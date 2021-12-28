using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.DAQmx;


namespace Interface_Prototype
{
    public partial class Form1 : Form
    {
        //Calibration Arrays
        private double[][] Calib1_Array = new double[2][];
        private double[][] Calib2_Array = new double[2][];

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
                }

                if (Calib_Form.Calib2_state == "completed") 
                { 
                    Calib2_Array = Calib_Form.Calib2_Array; 
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

    }
}
