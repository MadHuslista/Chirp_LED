using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;
using NationalInstruments;
using MathNet.Numerics;
using System.Globalization;


namespace Calibration_Test
{
    public partial class Form1 : Form
    {

        private string CLB_PATH = @"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\calib_arr.txt";
        //private string SIG_PATH = @"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\signal.txt";
        //private string CLB_PATH1 = @"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\calib_arrr.txt";
        private string SIG_PATH = @"D:\Proyecto_CHIRP_LED\Calibration_Test\logs\probe_signal.txt";

        private List<List<double>> Calibration_Data = new List<List<double>>();
        private double [][] Calibration_Array = new double [2][] ;
        private Task inputTask;
        private Task outputTask;
        private Task DAC_Task;
        private bool taskRunning = false;

        AnalogSingleChannelWriter writer;
        AnalogSingleChannelReader reader;
        private AsyncCallback inputCallback;

        private System.Collections.IEnumerator Ref_Enum;
        private int samples_per_step;

        private double[] Signal_Array;


        public Form1()
        {
            InitializeComponent();

            // Initialize UI
            inputChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));
            outputChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));

            if (inputChannelComboBox.Items.Count > 0)   inputChannelComboBox.SelectedIndex = 7; //Cambiar a index 0 antes de sacar a producción
            if (outputChannelComboBox.Items.Count > 0) outputChannelComboBox.SelectedIndex = 0;

            Calibration_Data.Add(new List<double>());
            Calibration_Data.Add(new List<double>());
            

        }

       
        private void StopTask()
        {
            inputTask.Stop();
            outputTask.Stop();
            inputTask.Dispose();
            outputTask.Dispose();
        }

        private void calibration_button_Click(object sender, EventArgs e)
        {
            double start_val = -1; //0V
            double stop_val = 10; // 10V
            double step = 0.001; //1mV

            double sample_rate = 20000;
            samples_per_step = 20; // 100;

            double [] Ref_Array = Generate.LinearRange(start_val, step, stop_val);
            Ref_Enum = Ref_Array.GetEnumerator();

            try
            {
                //Creación de los Task
                inputTask = new Task();
                outputTask = new Task();

                //Config de los canales 
                inputTask.AIChannels.CreateVoltageChannel(
                    inputChannelComboBox.Text,
                    "",
                    AITerminalConfiguration.Rse,
                    Convert.ToDouble(inputMinValNumeric.Value),
                    Convert.ToDouble(inputMaxValNumeric.Value),
                    AIVoltageUnits.Volts
                    );

                inputTask.Timing.ConfigureSampleClock(
                    "",
                    sample_rate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples
                    //samples_per_step
                    );

                outputTask.AOChannels.CreateVoltageChannel(
                    outputChannelComboBox.Text,
                    "",
                    Convert.ToDouble(outputMinValNumeric.Value),
                    Convert.ToDouble(outputMaxValNumeric.Value),
                    AOVoltageUnits.Volts
                    );

                inputTask.Control(TaskAction.Verify);
                outputTask.Control(TaskAction.Verify);

                writer = new AnalogSingleChannelWriter(outputTask.Stream);
                reader = new AnalogSingleChannelReader(inputTask.Stream);

                Ref_Enum.MoveNext();
                writer.WriteSingleSample(true, Convert.ToDouble(Ref_Enum.Current));


                inputCallback = new AsyncCallback(InputRead);
                reader.SynchronizeCallbacks = true;
                reader.BeginReadMultiSample(samples_per_step, inputCallback, inputTask);
                calibration_button.Enabled = false;
            }
            catch (Exception ex)
            {
                StopTask();
                MessageBox.Show(ex.Message);
            }


        }

        private void InputRead(IAsyncResult ar)
        {
            try
            {


                double[] data = reader.EndReadMultiSample(ar);

                double data_mean = data.Sum() / data.Length;

                Calibration_Data[0].Add(Convert.ToDouble(Ref_Enum.Current));
                Calibration_Data[1].Add(data_mean);


                if (Ref_Enum.MoveNext())
                {
                    writer.WriteSingleSample(true, Convert.ToDouble(Ref_Enum.Current));
                    reader.BeginReadMultiSample(samples_per_step, inputCallback, inputTask);
                }
                else
                {
                    calibration_button.Enabled = true;
                    Console.WriteLine("Done");
                    Calibration_Array[0] = Calibration_Data[0].ToArray();
                    Calibration_Array[1] = Calibration_Data[1].ToArray();

                    Array.Sort(Calibration_Array[1], Calibration_Array[0]);
                    save_calib(CLB_PATH);
                    StopTask();
                }
            }
            catch (Exception ex)
            {
                StopTask();
                MessageBox.Show(ex.Message);
            }
        }

        private void save_calib(String PATH)
        {
            var calib_sb = new StringBuilder(); 
            
            for (int i = 0; i < Calibration_Array[0].Length; i++)
            {
                calib_sb.Append(Calibration_Array[0][i].ToString());
                calib_sb.Append(",");
                calib_sb.AppendLine(Calibration_Array[1][i].ToString());
            }
            File.WriteAllText(PATH, calib_sb.ToString());
        }


        private double transform_func(double data)
        {
            //double data = Convert.ToDouble(numericUpDown1.Value);
            int Indx = Array.BinarySearch(Calibration_Array[1], data);
            if (Indx > 0)
            {
                double ret_val = Calibration_Array[0][Indx];
                //Console.WriteLine("{0},{1}",data, ret_val);
                return ret_val;

            }
            else
            {
                int I = ~Indx;
                
                if ((Calibration_Array[1][I] - data) < (data - Calibration_Array[1][I-1]))
                {
                    double ret_val = Calibration_Array[0][I];
                    //Console.WriteLine("{0},{1}", data, ret_val);
                    return ret_val;
                }
                else
                {
                    double ret_val = Calibration_Array[0][I-1];
                    //Console.WriteLine("{0},{1}", data, ret_val);
                    return ret_val;
                }
                
            }           
            
        }

        private void read_signal_Click(object sender, EventArgs e)
        {
            try
            {
                List<double> Signal_data = new List<double>();
                foreach (string point in File.ReadLines(SIG_PATH, Encoding.UTF8))
                {
                    //Console.WriteLine($"Str: {point}");
                    double temp = double.Parse(point, CultureInfo.InvariantCulture);
                    double calib_temp = transform_func(temp);
                    Signal_data.Add(calib_temp);
                }
                Signal_Array = Signal_data.ToArray();
                Console.WriteLine("Señal Correctamente Cargada - Array len:{0}", Signal_Array.Count());
                send_signal.Enabled = true;

                foreach (var point in Signal_Array) { Console.WriteLine($"Val: {point}");}

            }
            catch (IOException err)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(err.Message);
            }

        }

        private void send_signal_Click(object sender, EventArgs e)
        {
            send_signal.Enabled = false;
            taskRunning = true;

            try
            {
                DAC_Task = new Task();
                DAC_Task.AOChannels.CreateVoltageChannel(
                    outputChannelComboBox.Text,
                    "",
                    Convert.ToDouble(outputMinValNumeric.Value),
                    Convert.ToDouble(outputMaxValNumeric.Value),
                    AOVoltageUnits.Volts
                    );
                DAC_Task.Timing.ConfigureSampleClock(
                    "",
                    Convert.ToDouble(numericUpDown1.Value),
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples,
                    Signal_Array.Length
                    );

                AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(DAC_Task.Stream);

                DAC_Task.Done += new TaskDoneEventHandler(DAC_Task_Done);

                writer.WriteMultiSample(false, Signal_Array);
                DAC_Task.Start();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                if (DAC_Task != null)
                {
                    DAC_Task.Dispose();
                }

                taskRunning = false;
                send_signal.Enabled = true;
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
            send_signal.Enabled = true;
        }
    }
}
