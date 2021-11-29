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

namespace Calibration_Test
{
    public partial class Form1 : Form
    {

        private string CLB_PATH = @"D:\Proyecto_CHIRP_LED\Calibration_Test\logs";

        private List<List<double>> Calibration_Data = new List<List<double>>();
        private double [][] Calibration_Array = new double [2][] ;
        private Task inputTask;
        private Task outputTask;
        AnalogSingleChannelWriter writer;
        AnalogSingleChannelReader reader;
        private AsyncCallback inputCallback;

        private System.Collections.IEnumerator Ref_Enum;
        private int samples_per_step; 
        

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

        private void calibration_button_Click(object sender, EventArgs e)
        {
            double start_val = 0; //0V
            double stop_val = 10; // 10V
            double step = 0.01; //1mV

            double sample_rate = 20000;
            samples_per_step = 50; // 100;

            double [] Ref_Array = Generate.LinearRange(start_val, step, stop_val);
            Ref_Enum = Ref_Array.GetEnumerator();

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
                SampleQuantityMode.ContinuousSamples,
                samples_per_step
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

        private void InputRead(IAsyncResult ar)
        {
            double[] data = reader.EndReadMultiSample(ar);

            double data_mean = data.Sum() / data.Length;

            Calibration_Data[0].Add(data_mean);
            Calibration_Data[1].Add(Convert.ToDouble(Ref_Enum.Current));
            
            
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
                save_calib();
            }

        }

        private void save_calib()
        {
            var calib_sb = new StringBuilder(); 
            
            for (int i = 0; i < Calibration_Array[0].Length; i++)
            {
                calib_sb.Append(Calibration_Array[0][i].ToString());
                calib_sb.Append(",");
                calib_sb.AppendLine(Calibration_Array[1][i].ToString());
            }
            File.WriteAllText(CLB_PATH, calib_sb.ToString());
        }
    }
}
