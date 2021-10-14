using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thorlabs.DC4100.Interop;
using Ivi.Visa.Interop;
using Thorlabs;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace Driver_LED
{
    public partial class Form1 : Form
    {
        // # CAMPOS #
        private TLDC4100 tlDC4100 = null;
        int a = 0;
        private string PATH = @"D:\Proyecto_CHIRP_LED\Interface_ThorLabs\logs\signal.txt";
        private string TIME_REGISTER = @"D:\Proyecto_CHIRP_LED\Interface_ThorLabs\logs\time_registro.txt";
        private string VALS_REGISTER = @"D:\Proyecto_CHIRP_LED\Interface_ThorLabs\logs\vals_registro.txt";
        private float[] Signal_Array;
        private int Sampling_Freq;
        private long start, end;
        private Stopwatch start_global = new Stopwatch();
        private Stopwatch start_w = new Stopwatch();
        


        // # Constructor #
        public Form1() 
            //Constructor 
        {
            InitializeComponent();
            start_global.Start();
            start_w.Start();
            if (false)
            { 
            
            this.toolStripStatusLabel1.Text = "Thorlabs DC4100 Series instrument driver sample application.";
            this.statusStrip1.Refresh();

            //search for connected instruments - Avisa que está escaneando
            Console.WriteLine("Scanning for DC4100 Series instruments ...");
            this.toolStripStatusLabel1.Text = "Scanning for DC4100 Series instruments ...";
            this.statusStrip1.Refresh();

            // Efectúa el escanéo real. 
            string[] dc4100List = DC4100_ResourceManager.FindRscDC4100();

            // Si no encuentra nada, avisa que no pasó nah. 
            if (dc4100List.Length == 0)
            {
                Console.WriteLine("No instrument found.");
                this.toolStripStatusLabel1.Text = "No instrument found.";
                this.statusStrip1.Refresh();
                return;
            }

            // De lo contrario, avisa qeu sí encotrnó y que se conectó al primer dispositivo. 
            Console.WriteLine("Opening session to '{0}' ...", dc4100List[0].ToString());
            this.toolStripStatusLabel1.Text = string.Format("Opening session to '{0}' ...", dc4100List[0].ToString());
            this.statusStrip1.Refresh();

            //Aquí efectúa la conexión real. 
            tlDC4100 = new TLDC4100(dc4100List[0], false, false);

            // Set all LEDs off - Por seguridad
            for (int i = 0; i < 4; i++)
            {
                tlDC4100.setLedOnOff(i, false);
            }

            // Set the modus to Constant Current
            tlDC4100.setOperationMode(0);

            // Set Selection Mode to Multi Select
            tlDC4100.setSelectionMode(0);

            // Set all currents and switch the LEDs on
            for (int i = 0; i < 4; i++)
            {
                //tlDC4100.setConstCurrent(i, i * 0.01f); //Setea la corriente en intervalos de 10 en 10 mA
                //tlDC4100.setLedOnOff(i, true);
            }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Por si la señal no se ha cargado. 
            if (Signal_Array == null)
            {
                Console.WriteLine("Señal no Cargada");
                timer1.Enabled = false;
                return;
            }

            // Lectura punto
            label1.Text = a.ToString();
            float point = Signal_Array[a];
            Console.WriteLine($"Point: {point}");

            //Evaluación seguridad

            /*
            float limit;
            tlDC4100.getLimitCurrent(0, out limit);
            if (point > limit)
            {
                tlDC4100.setLedOnOff(0, false);
                timer1.Enabled = false;
                Console.WriteLine("Señal Interrumpida");
                Console.WriteLine("Punto sobre el rango de seguridad");
                return;
            }
            */
            // Envío de Punto 
            start = Stopwatch.GetTimestamp();
            long ts_c = (start - end);
            tlDC4100.setConstCurrent(0, point);
            end = Stopwatch.GetTimestamp();
            long ts = (end - start);
            Console.WriteLine("Elapsed Time is {0} ms, ciclo: {1}", ts/10000.0, ts_c/10000.0);

            // Siguiente Elemento
            a++;

            // Fin del arreglo
            if (a > (Signal_Array.Length -1))
            {
                timer1.Enabled = false;
                a = 0;
                tlDC4100.setLedOnOff(0, false);
                Console.WriteLine("Señal Finalizada");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void signal_reader(object sender, EventArgs e)
        {
            timer1.Enabled = false;
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
                foreach(var point in Signal_Array)
                {
                    Console.WriteLine($"Val: {point}");
                }
                button4.Enabled = true;
                button7.Enabled = true;
            }
            catch (IOException err)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(err.Message);
            }

        }

        private void signal_sender(object sender, EventArgs e)
        {
            //Preparación Timer
            Decimal Hz = numericUpDown1.Value;
            Decimal Interval = (1000 / Hz); //Hz -> msec
            Sampling_Freq = Decimal.ToInt32(Interval);
            //Console.WriteLine($"timer_Interval: {Sampling_Freq}, Int: {Interval}");
            //timer1.Interval = Sampling_Freq;
            timer1.Interval = 580;
            this.toolStripStatusLabel1.Text = timer1.Interval.ToString();
            this.statusStrip1.Refresh();


            //Preparación LED
            DateTime start = DateTime.Now;
            tlDC4100.setLedOnOff(0, true);
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            Console.WriteLine("ENCENDIDO Time is {0} ms", ts.TotalMilliseconds);

            //Inicio Señal
            timer1.Enabled = true;
        }

        private void Freq_Check(object sender, EventArgs e)
        {          
            //Cálculo del Intervalo 
            long Freq = Decimal.ToInt64(numericUpDown1.Value);
            //double Interval = Math.Round((1000 / Freq), 4, MidpointRounding.AwayFromZero);
            long Interval = Stopwatch.Frequency / Freq; 

            //Cálculo de la cantidad de Iteraciones. 
            double duration = Decimal.ToDouble(numericUpDown2.Value);
            long iteraciones = Convert.ToInt64((duration * Freq));
            long i = 0;

            
            //Preparación Registro
            var times = new double[iteraciones];

            //Preparación Timers
            double time_point = Interval;
            start_global.Restart();
            start_w.Restart();

            while(true)
            {
                if(start_w.ElapsedTicks >= time_point)
                {
                    times[i] = start_w.Elapsed.TotalMilliseconds;
                    time_point += Interval;
                    i++; 
                }

                if(i >= iteraciones)
                {
                    start_global.Stop();
                    break;
                }
            }
            
            

            //Registro: 
            var tsv = new StringBuilder(); 
            foreach(var time in times)
            {
                //Console.WriteLine(value);
                tsv.AppendLine(time.ToString()); 
            }
            File.WriteAllText(TIME_REGISTER, tsv.ToString());

            Console.WriteLine("i count: {0}", i);
            Console.WriteLine("Elapsed Time is {0}| {1} | {2} | {3}", start_global.Elapsed, start_global.ElapsedMilliseconds, start_global.ElapsedTicks, Stopwatch.Frequency);
            double err = 1000 - (Freq * Interval);
            Console.WriteLine("{0} | {1} | {2}", Interval, i, err);
        }

        private void Sig_Check(object sender, EventArgs e)
        {
            //Cálculo del Intervalo 
            long Freq = Decimal.ToInt64(numericUpDown1.Value);
            long Interval = Stopwatch.Frequency / Freq;

            //Cálculo de la cantidad de Iteraciones. 
            double duration = Decimal.ToDouble(numericUpDown2.Value);
            long iteraciones = Convert.ToInt64((duration * Freq));
            long i = 0;


            //Preparación Registro Tiempo
            var times = new double[iteraciones];
            var values = new float[iteraciones]; //Iteraciones debería ser de = largo que la señal. 

            //Preparación Timers
            double time_point = Interval;
            start_global.Restart();
            start_w.Restart();

            while (true)
            {
                if (start_w.ElapsedTicks >= time_point)
                {
                    values[i] = Signal_Array[i];
                    times[i] = start_w.Elapsed.TotalMilliseconds;
                    time_point += Interval;
                    i++;
                }

                if (i >= iteraciones)
                {
                    start_global.Stop();
                    break;
                }
            }



            //Registro Tiempo: 
            var time_sb = new StringBuilder();
            foreach (var time in times)
            {
                //Console.WriteLine(value);
                time_sb.AppendLine(time.ToString());
            }
            File.WriteAllText(TIME_REGISTER, time_sb.ToString());

            //Registro Señal: 
            var vals_sb = new StringBuilder();
            foreach (var value in values)
            {
                //Console.WriteLine(value);
                vals_sb.AppendLine(value.ToString());
            }
            File.WriteAllText(VALS_REGISTER, vals_sb.ToString());

            Console.WriteLine("i count: {0} | len array:{1}", i, Signal_Array.Count());
            Console.WriteLine("Elapsed Time is {0}[s]| {1}[ms] | {2}[ticks] | {3}[tick_freq]", start_global.Elapsed, start_global.ElapsedMilliseconds, start_global.ElapsedTicks, Stopwatch.Frequency);
            double err = 1000 - (Freq * Interval);
            Console.WriteLine("Interval {0}[ms] | interv_err: {1} [ms]", Interval, err);
        }

        private void stop_signal(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            a = 0;
            Console.WriteLine("Señal Detenida y Reseteada");

        }

        //private static String GetTimestamp(DateTime value)  { }

    }
} 
