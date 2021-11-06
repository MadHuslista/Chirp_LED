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

        private float[] Signal_Array;
        
        private Stopwatch sig_clock = new Stopwatch();

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
                List<float> Signal_data = new List<float>();
                foreach (string point in File.ReadLines(PATH, Encoding.UTF8))
                {
                    //Console.WriteLine($"Str: {point}");
                    Signal_data.Add(float.Parse(point, CultureInfo.InvariantCulture));
                }
                Signal_Array = Signal_data.ToArray();
                Console.WriteLine("Señal Correctamente Cargada - Array len:{0}", Signal_Array.Count());
                button2.Enabled = true;

                //foreach (var point in Signal_Array) { Console.WriteLine($"Val: {point}");}
                
            }
            catch (IOException err)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(err.Message);
            }
        }

        private void send_signal(object sender, EventArgs e)
        {
            

            //Cálculo del Intervalo 
            long Freq = Decimal.ToInt64(numericUpDown1.Value);
            long Interval = Stopwatch.Frequency / Freq;

            //Cálculo de la cantidad de Iteraciones. 
            double duration = Decimal.ToDouble(numericUpDown2.Value);
            long iteraciones = Convert.ToInt64((duration * Freq));
            //long iteraciones = Signal_Array.Count();
            long i = 0;


            //Preparación Registro Tiempo
            var times = new double[iteraciones];
            var pretimes = new double[iteraciones];
            var values = new float[iteraciones]; //Iteraciones debería ser de = largo que la señal. 

            //Creación del Task y objetos necesarios para la comunicación con el DAC

            var channel = comboBox1.Text;
            double minval = Convert.ToDouble(textBox1.Text);
            double maxval = Convert.ToDouble(textBox2.Text);

            Task DAC_Task = new Task(); // Crea la 'Tarea' (un entorno de trabajo) (paso 1) 
            DAC_Task.AOChannels.CreateVoltageChannel(channel, "aoChannel", minval, maxval, AOVoltageUnits.Volts); // Crea y Configura el canal (paso 1) 

            AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(DAC_Task.Stream); // Crea el Writer (paso 2) 
            
            /*
            AnalogSingleChannelWriter writessr = new AnalogSingleChannelWriter(DAC_Task.Stream); // Crea el Writer (paso 2) 

            var test = new double[Signal_Array.Count()];
            var d = 0;
            foreach (var t in test) { test[d] = Convert.ToDouble(Signal_Array[d]); d++; }
            writessr.WriteMultiSample(true, test);
            */

            //Preparación Timers
            long time_point = Interval;
            sig_clock.Restart();
            

            //Envío efectivo de la señal.
            while (true)
            {
                if (sig_clock.ElapsedTicks >= time_point)
                {
                    //values[i] = Signal_Array[i];
                    pretimes[i] = sig_clock.Elapsed.TotalMilliseconds;
                    writer.WriteSingleSample(true, Signal_Array[i]);
                    times[i] = sig_clock.Elapsed.TotalMilliseconds;
                    time_point += Interval;
                    i++;
                }

                if (i >= iteraciones)
                {
                    sig_clock.Stop();
                    break;
                }
            }

            DAC_Task.Dispose();

            //Registro PreTiempo: 
            var pretime_sb = new StringBuilder();
            foreach (var pretime in pretimes)
            {
                //Console.WriteLine(value);
                pretime_sb.AppendLine(pretime.ToString());
            }
            File.WriteAllText(PRETIME_REGISTER, pretime_sb.ToString());

            //Registro Tiempo: 
            var time_sb = new StringBuilder();
            foreach (var time in times)
            {
                //Console.WriteLine(value);
                time_sb.AppendLine(time.ToString());
            }
            File.WriteAllText(TIME_REGISTER, time_sb.ToString());

            /*
            //Registro Señal: 
            var vals_sb = new StringBuilder();
            foreach (var value in values)
            {
                //Console.WriteLine(value);
                vals_sb.AppendLine(value.ToString());
            }
            File.WriteAllText(VALS_REGISTER, vals_sb.ToString());
            */

            Console.WriteLine("i count: {0} | len array:{1}", i, Signal_Array.Count());
            Console.WriteLine("Elapsed Time is {0}[s]| {1}[ms] | {2}[ticks] | {3}[tick_freq]", sig_clock.Elapsed, sig_clock.ElapsedMilliseconds, sig_clock.ElapsedTicks, Stopwatch.Frequency);
            double err = 1000 - (Freq * Interval);
            Console.WriteLine("Interval {0}[ms] | interv_err: {1} [ms]", Interval, err);
        }

        private void stop_signal(object sender, EventArgs e)
        {

        }


        // Límpia cualqueir recurso que haya quedado usándose. 
   

    }

}
