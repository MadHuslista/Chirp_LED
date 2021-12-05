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

            //Captura de Canales y preselección del primero 
            PhCh_InCh1_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External)); 
            if (PhCh_InCh1_comboBox.Items.Count > 0) { PhCh_InCh1_comboBox.SelectedIndex = 7; }

            PhCh_InCh2_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External));
            if (PhCh_InCh2_comboBox.Items.Count > 0) { PhCh_InCh2_comboBox.SelectedIndex = 0; }

            PhCh_OutCh1_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));
            if (PhCh_OutCh1_comboBox.Items.Count > 0) { PhCh_OutCh1_comboBox.SelectedIndex = 0; }

            PhCh_OutCh2_comboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External));
            if (PhCh_OutCh2_comboBox.Items.Count > 0) { PhCh_OutCh2_comboBox.SelectedIndex = 1; }

        }
        
    }
}
