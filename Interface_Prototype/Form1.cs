using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;


namespace Interface_Prototype
{
    public partial class Form1 : Form
    {
        //Calibration Arrays
        private double[][] Calib1_Array = new double[2][];
        private double[][] Calib2_Array = new double[2][];

        private bool OutCh1_taskrunning = false;
        private bool OutCh2_taskrunning = false;
        private Task DAC_Task;


        public Form1()
        {
            InitializeComponent();

            // Captura de Canales y preselección del primero. 
            OutCh1_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));
            OutCh2_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));
            InCh1_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));
            InCh2_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));

            if (OutCh1_comboBox.Items.Count > 0) { OutCh1_comboBox.SelectedIndex = 0; }
            if (OutCh2_comboBox.Items.Count > 0) { OutCh2_comboBox.SelectedIndex = 1; }
            if (InCh1_comboBox.Items.Count > 0) { InCh1_comboBox.SelectedIndex = 7; } 
            if (InCh2_comboBox.Items.Count > 0) { InCh2_comboBox.SelectedIndex = 1; }//change to 0 in production


        }

        // ####################################### Particular Use Funcions

        private void OutCh1_Write_button_Click(object sender, EventArgs e)
        {
            string[] channels = { OutCh1_comboBox.Text };
            double[] max_val = { Convert.ToDouble(OutCh1Max_numericUpDown.Value) };
            double[] min_val = { Convert.ToDouble(OutCh1Min_numericUpDown.Value) };
            double[] hz = { Convert.ToDouble(OutCh1Hz_numericUpDown.Value) };

            List<List<double>> data = new List<List<double>>();
            data.Add(OutCh1_Signal);

            List<double[][]> Calib_Arr = new List<double[][]>();
            Calib_Arr.Add(Calib1_Array);

            if (!(Calib1_Array[0] != null))
            {
                MessageBox.Show("Missing Calibration Profile");
            }
            else
            {
                Send_Signal(
                data: data,
                Calib_Profile: Calib_Arr,
                taskrunning: ref OutCh1_taskrunning,
                ao_channel: channels,
                max_val: max_val,
                min_val: min_val,
                hz: hz
                );
            }

        }

        private void OutCh2_Write_button_Click(object sender, EventArgs e)
        {

            string[] channels = { OutCh2_comboBox.Text };
            double[] max_val = { Convert.ToDouble(OutCh2Max_numericUpDown.Value) };
            double[] min_val = { Convert.ToDouble(OutCh2Min_numericUpDown.Value) };
            double[] hz = { Convert.ToDouble(OutCh2Hz_numericUpDown.Value) };

            List<List<double>> data = new List<List<double>>();
            data.Add(OutCh2_Signal);

            List<double[][]> Calib_Arr = new List<double[][]>();
            Calib_Arr.Add(Calib2_Array);

            if (!(Calib2_Array[0] != null))
            {
                MessageBox.Show("Missing Calibration Profile");
            }
            else
            {
                Send_Signal(
                data: data,
                Calib_Profile: Calib_Arr,
                taskrunning: ref OutCh2_taskrunning,
                ao_channel: channels,
                max_val: max_val,
                min_val: min_val,
                hz: hz
                );
            }            
        }

        private void OutChBoth_Write_button_Click(object sender, EventArgs e)
        {


            string[] channels = { 
                OutCh1_comboBox.Text,
                OutCh2_comboBox.Text
            };
            double[] max_val = { 
                Convert.ToDouble(OutCh1Max_numericUpDown.Value),
                Convert.ToDouble(OutCh2Max_numericUpDown.Value)
            };
            double[] min_val = { 
                Convert.ToDouble(OutCh1Min_numericUpDown.Value),
                Convert.ToDouble(OutCh2Min_numericUpDown.Value)
            };
            double[] hz = { 
                Convert.ToDouble(OutCh1Hz_numericUpDown.Value),
                Convert.ToDouble(OutCh2Hz_numericUpDown.Value)
            };

            List<List<double>> data = new List<List<double>>();
            data.Add(OutCh1_Signal);
            data.Add(OutCh2_Signal);

            List<double[][]> Calib_Arr = new List<double[][]>();
            Calib_Arr.Add(Calib1_Array);
            Calib_Arr.Add(Calib2_Array);

            //Console.WriteLine(Calib2_Array.Length);

            if ((!(Calib1_Array[0] != null)) || (!(Calib2_Array[0] != null)))
            {
                MessageBox.Show("Missing Calibration Profile");
            }
            else
            {
                Send_Signal(
                data: data,
                Calib_Profile: Calib_Arr,
                taskrunning: ref OutCh2_taskrunning,
                ao_channel: channels,
                max_val: max_val,
                min_val: min_val,
                hz: hz
                );
            }
        }

        // ####################################### General Use Funcions


        private void NewCalib_button_Click(object sender, EventArgs e)
        {

            string [] channels = 
            {
                InCh1_comboBox.Text,
                InCh2_comboBox.Text,
                OutCh1_comboBox.Text,
                OutCh2_comboBox.Text   
            };

            double [][] values = new double [4][];

            values[0] = new[] { 
                Convert.ToDouble(InCh1Min_numericUpDown.Value), 
                Convert.ToDouble(InCh1Max_numericUpDown.Value), 
                Convert.ToDouble(InCh1Hz_numericUpDown.Value) 
            };
            values[1] = new[] { 
                Convert.ToDouble(InCh2Min_numericUpDown.Value), 
                Convert.ToDouble(InCh2Max_numericUpDown.Value), 
                Convert.ToDouble(InCh2Hz_numericUpDown.Value) 
            };
            values[2] = new[] { 
                Convert.ToDouble(OutCh1Min_numericUpDown.Value), 
                Convert.ToDouble(OutCh1Max_numericUpDown.Value), 
                Convert.ToDouble(OutCh1Hz_numericUpDown.Value) 
            };
            values[3] = new[] { 
                Convert.ToDouble(OutCh2Min_numericUpDown.Value), 
                Convert.ToDouble(OutCh2Max_numericUpDown.Value),
                Convert.ToDouble(OutCh2Hz_numericUpDown.Value) 
            };


            Form_Calib Calib_Form = new Form_Calib(channels , values);

            if( Calib_Form.ShowDialog() == DialogResult.OK)
            {
                if (Calib_Form.Calib1_state == "completed") 
                { 
                    Calib1_Array = Calib_Form.Calib1_Array;
                    OutCh1_MaxLabel.Text = Convert.ToString(Math.Round(Calib1_Array[1].Max(), 4));
                }

                if (Calib_Form.Calib2_state == "completed") 
                { 
                    Calib2_Array = Calib_Form.Calib2_Array;
                    OutCh2_MaxLabel.Text = Convert.ToString(Math.Round(Calib2_Array[1].Max(), 4));
                }
                

                if ( (Calib_Form.Calib1_Association != "") ^ (Calib_Form.Calib2_Association != "")  )
                {
                    CalibLoaded_label.Text = "P: " + Calib_Form.Calib1_Association + Calib_Form.Calib2_Association;
                    CalibLoaded_label.BackColor = Color.PaleGreen; 
                }
                else
                {
                    CalibLoaded_label.Text = "P: " + Calib_Form.Calib1_Association + "  " + Calib_Form.Calib2_Association;
                    CalibLoaded_label.BackColor = Color.PaleGreen;
                }
            }
            else
            {
                Calib_Form.Calib1_taskRunning = false;
                Calib_Form.Calib2_taskRunning = false;
                //if (Calib_Form.Calib1_taskRunning) { Calib_Form.StopTask(1); }
                //if (Calib_Form.Calib2_taskRunning) { Calib_Form.StopTask(2); }

            }
            
        }

        private void UpdateChart( List<double> time, List<double> data, ref System.Windows.Forms.DataVisualization.Charting.Chart Chart )
        {
            //Console.WriteLine("{0} {1}", time.Count/Chart.Width, Chart.Width);

            int down_size = Chart.Width;
            int step = time.Count / (Chart.Width*15);

            step = step + Convert.ToInt32((step < 1));

            //Console.WriteLine("ds:{0} st:{1}", down_size, step);

            //double[] downsampl_time = new double[down_size];
            //double [] downsampl_data = new double[down_size];
            List<double> downsampl_time = new List<double>();
            List<double> downsampl_data = new List<double>();

            Console.WriteLine("t.co{0} step{1}", time.Count, step);

            for (int i = 0; i < time.Count; i+= step)
            {             
                //downsampl_data[k] = data[i];
                //downsampl_time[k] = time[i];
                downsampl_time.Add(time[i]);
                downsampl_data.Add(data[i]); 
            }

            //Console.WriteLine("{0} {1}", downsampl_data.Count, downsampl_time.Count);


            Chart.Series[0].Points.Clear();
            Chart.Series[0].Points.DataBindXY(downsampl_time, downsampl_data);
        }

        private void Send_Signal(List<List<double>> data, List<double[][]> Calib_Profile, ref bool taskrunning, string[] ao_channel, double[] max_val, double[] min_val, double[] hz)
        {
            OutCh1_Write_button.Enabled = false;
            OutCh2_Write_button.Enabled = false;
            OutChBoth_Write_button.Enabled = false;

            taskrunning = true;




            double[,] data_array;

            if (data.Count == 2)
            {
                data_array = new double[2, data[0].Count];

                for (int i = 0; i < data[0].Count; i++)
                {

                    data_array[0, i] = Transform_Func(data[0][i], Calib_Profile[0]);
                    data_array[1, i] = Transform_Func(data[1][i], Calib_Profile[1]);
                }
            }
            else
            {
                data_array = new double[1, data[0].Count];

                for (int i = 0; i < data[0].Count; i++)
                {
                    data_array[0, i] = Transform_Func(data[0][i], Calib_Profile[0]);
                }
            }

            
            try
            {

                DAC_Task = new Task();

                for (int i = 0; i < ao_channel.Length; i++)
                {
                    DAC_Task.AOChannels.CreateVoltageChannel(
                        ao_channel[i],
                        "",
                        min_val[i],
                        max_val[i],
                        AOVoltageUnits.Volts
                        );
                    AOChannel b = DAC_Task.AOChannels.All;
                    Console.WriteLine(b);

                }

                DAC_Task.Timing.ConfigureSampleClock(
                    "",
                    Convert.ToDouble(hz[0]),
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, 
                    data[0].Count
                    );

                DAC_Task.Control(TaskAction.Verify);

                AnalogMultiChannelWriter writer = new AnalogMultiChannelWriter(DAC_Task.Stream);



                DAC_Task.Done += new TaskDoneEventHandler(DAC_Task_Done);
                writer.WriteMultiSample(false, data_array);
                DAC_Task.Start();
                    

            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);                
                
                if (DAC_Task != null)
                {
                    DAC_Task.Dispose();
                }

                if (OutCh1_taskrunning) { OutCh1_taskrunning = false; };
                if (OutCh2_taskrunning) { OutCh2_taskrunning = false; };
                
                OutCh1_Write_button.Enabled = true;
            }


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

            if (OutCh1_taskrunning) { OutCh1_taskrunning = false; };
            OutCh1_Write_button.Enabled = true;
            OutCh2_Write_button.Enabled = true;
            OutChBoth_Write_button.Enabled = true;
        }

        private double Transform_Func(double data, double [][] Calib)
        {
            int Indx = Array.BinarySearch(Calib[1], data); //Busco la posición de la data que quiero conseguir, en las mediciones. 

            Console.WriteLine(Indx);
            if (Indx >= 0)
            {
                double ret_val = Calib[0][Indx];
                //Console.WriteLine("{0},{1}",data, ret_val);
                return ret_val;

            }
            else
            {
                int I = ~Indx;
                Console.WriteLine(I);

                if ((Calib[1][I] - data) < (data - Calib[1][I-1]))
                {
                    double ret_val = Calib[0][I];
                    //Console.WriteLine("{0},{1}", data, ret_val);
                    return ret_val;
                }
                else
                {
                    double ret_val = Calib[0][I-1];
                    //Console.WriteLine("{0},{1}", data, ret_val);
                    return ret_val;
                }
                
            }           

        }

        
    }
}
