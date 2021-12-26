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

            object a = Calib_Form.ShowDialog();
            Console.WriteLine(a);
        }

        private void LoadCalib_button_Click(object sender, EventArgs e)
        {

        }
    }
}
