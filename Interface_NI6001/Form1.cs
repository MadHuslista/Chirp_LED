using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using NationalInstruments.DAQmx;


namespace Interface_Net4._5
{
    public partial class Form1 : Form
    {
        private string PATH = @"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\signal.txt";
        private string TIME_REGISTER = @"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\time_registro.txt";
        private string PRETIME_REGISTER = @"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\pretime_registro.txt";
        private string VALS_REGISTER = @"D:\Proyecto_CHIRP_LED\Interface_NI6001\logs\vals_registro.txt";

        private double[] Signal_Array;
        
        private Stopwatch sig_clock = new Stopwatch();

        private bool taskRunning = false; // Seguimiento del Task 
        Task DAC_Task = null;

        public Form1()
        {
            InitializeComponent();

            //Captura de Canales y preselección del primero. 
            comboBox1.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));
            if (comboBox1.Items.Count > 0) { comboBox1.SelectedIndex = 0; }

            //Iniciar el reloj aquí, ahorra el tiempo de iniciación del mismo. Luego basta con reiniciarlo (lo que demora menos). 
            sig_clock.Start();
        }

        private void read_signal(object sender, EventArgs e)
        {
            try
            {
                List<double> Signal_data = new List<double>();
                foreach (string point in File.ReadLines(PATH, Encoding.UTF8))
                {
                    //Console.WriteLine($"Str: {point}");
                    Signal_data.Add(double.Parse(point, CultureInfo.InvariantCulture));
                }
                Signal_Array = Signal_data.ToArray();
                Console.WriteLine("Señal Correctamente Cargada - Array len:{0}", Signal_Array.Count());
                button2.Enabled = true;

                foreach (var point in Signal_Array) { Console.WriteLine($"Val: {point}");}
                
            }
            catch (IOException err)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(err.Message);
            }
        }

        private void send_signal(object sender, EventArgs e)
        {

            button2.Enabled = false;
            taskRunning = true;

            //Obtención de Frecuencia de Muestreo y otros datos
            var channel = comboBox1.Text;
            double minval = Convert.ToDouble(textBox1.Text);
            double maxval = Convert.ToDouble(textBox2.Text);
            double sample_rate = Decimal.ToDouble(numericUpDown1.Value);

            // Creación del Task y objetos necesarios para la comunicación con el DAC

            try
            {
                DAC_Task = new Task(); // Crea la 'Tarea' (un entorno de trabajo) (paso 1) 
                DAC_Task.AOChannels.CreateVoltageChannel(channel, "aoChannel", minval, maxval, AOVoltageUnits.Volts); // Crea y Configura el canal (paso 1) 

                //Verificación de Rigor
                DAC_Task.Control(TaskAction.Verify);

                //Configuración del Timing del Task 
                DAC_Task.Timing.ConfigureSampleClock(
                    "",                                 //Para configurar que la fuente sea interna, se usa ""
                    sample_rate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples,
                    Signal_Array.Length);

                //Escritura de la Data
                AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(DAC_Task.Stream); // Crea el Writer (paso 2) 

                //Config del evento que avisa que ya se escribió todo. 
                DAC_Task.Done += new TaskDoneEventHandler(DAC_Task_Done);

                //Envío efectivo de la Data
                writer.WriteMultiSample(false, Signal_Array);
                DAC_Task.Start();
            }

            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);

                if (DAC_Task != null)
                {
                    DAC_Task.Dispose();
                }
                taskRunning = false;
                button2.Enabled = true;
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = taskRunning;
        }

        private void DAC_Task_Done(object sender, TaskDoneEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            if (DAC_Task != null)
            {
                DAC_Task.Dispose();
            }

            taskRunning = false;
            button2.Enabled = true;
        }

        private void stop_signal(object sender, EventArgs e)
        {

        }


        // Límpia cualqueir recurso que haya quedado usándose. 
   

    }

}
