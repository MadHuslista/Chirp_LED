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
            if (InCh1_comboBox.Items.Count > 0) { InCh1_comboBox.SelectedIndex = 0; }
            if (InCh2_comboBox.Items.Count > 0) { InCh2_comboBox.SelectedIndex = 1; }
        }
    }
}
