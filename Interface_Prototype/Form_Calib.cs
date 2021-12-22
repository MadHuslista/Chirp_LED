using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.DAQmx;
using MathNet.Numerics;


namespace Interface_Prototype
{
    public partial class Form_Calib : Form
    {

        //Channels Data
        private Dictionary<string, Dictionary<string, double>> Ch_Dict;         //Channels config info Dict
        
        //Calibration Range 
        private double start_val, stop_val, step;
        private int samples_per_step;

        //Calibration Arrays
        private List<List<double>> Calib1_Data = new List<List<double>>();
        private List<List<double>> Calib2_Data = new List<List<double>>();
        private double[][] Calib1_Array = new double[2][];
        private double[][] Calib2_Array = new double[2][];
        private IEnumerator Calib1_RefEnum, Calib2_RefEnum;

        //DAQ Objects
        private Task Calib1_inputTask, Calib1_outputTask;
        private Task Calib2_inputTask, Calib2_outputTask;

        private AnalogSingleChannelWriter Calib1_writer, Calib2_writer;
        private AnalogSingleChannelReader Calib1_reader, Calib2_reader;

        private AsyncCallback Calib1_InputCallback, Calib2_InputCallback;

        private bool Calib1_taskRunning = false;
        private bool Calib2_taskRunning = false;     

        public Form_Calib(string [] channels, double [][] values) 
        {
            InitializeComponent();
            
            //Dictionary Population
            Ch_Dict = new Dictionary<string, Dictionary<string, double>>();
            string[] val_keys = { "Min", "Max", "Hz" };
            for (int i = 0; i  < channels.Length; i++)
            { 
                Ch_Dict.Add(channels[i], new Dictionary<string, double>()); //Cojo el canal
                for (int j = 0; j < values[i].Length; j++)
                {
                    Ch_Dict[channels[i]].Add(val_keys[j], values[i][j]);    //Agrego cada valor de los valores según su key correspondiente. 
                    //Console.WriteLine("{0}, {1}, {2}", channels[i], val_keys[j], values[i][j]);   //Revisión de Correspondencia 
                }
            }
            

            //Calibration Ranges
            start_val = 0;
            stop_val = 10;
            step = 0.001;
            samples_per_step = 25;
            

            // Captura de Canales y preselección del primero. 
            Calib1In_comboBox.Items.AddRange(new object[] { channels[0], channels[1] });
            Calib2In_comboBox.Items.AddRange(new object[] { channels[0], channels[1] });
            Calib1Out_comboBox.Items.AddRange(new object[] { channels[2], channels[3] });
            Calib2Out_comboBox.Items.AddRange(new object[] { channels[2], channels[3] });

            if (Calib1In_comboBox.Items.Count > 0) Calib1In_comboBox.SelectedIndex = 0;
            if (Calib2In_comboBox.Items.Count > 0) Calib2In_comboBox.SelectedIndex = 1;
            if (Calib1Out_comboBox.Items.Count > 0) Calib1Out_comboBox.SelectedIndex = 0;
            if (Calib2Out_comboBox.Items.Count > 0) Calib2Out_comboBox.SelectedIndex = 1;

            //Inicialización de Arreglos
            Calib1_Data.Add(new List<double>());
            Calib1_Data.Add(new List<double>());
            Calib2_Data.Add(new List<double>());
            Calib2_Data.Add(new List<double>());
        }


        // ####################################### General Use Funcions
        private void DoCalib(
            //Channels Values
            string[] channels,
            Dictionary<string, double> input_values,
            Dictionary<string, double> output_values,

            //DAC Connection Ojects
            ref Task inputTask,
            ref Task outputTask,
            ref AnalogSingleChannelWriter writer,
            ref AnalogSingleChannelReader reader,
            ref AsyncCallback InputCallback,
            Action<IAsyncResult> Callback_Function,
            int calib,

            //Data Management
            ref IEnumerator Ref_Enum,
            ref List<List<double>> Calib_Data, 

            //Form Management
            ref ProgressBar Progress_Bar,
            ref Button Same_CalibButt,
            ref Button Opos_CalibButt,
            ref Button Same_SaveCalib,
            ref Button Opos_SaveCalib
            )
        {
            //Asignación de Canales y Task
            string input_channel = channels[0];
            string output_channel = channels[1];

            //double[] input_values = values[0];
            //double[] output_values = values[1];

            //Creación del Arreglo de Calibración 
            double[] Ref_Array = Generate.LinearRange(start_val, step, stop_val);
            Ref_Enum = Ref_Array.GetEnumerator();
            Progress_Bar.Maximum = Ref_Array.Length + 10;
            Progress_Bar.Value = 0;


            //Console.WriteLine("Ref_Enum: {0}", object.ReferenceEquals(Ref_Enum, Calib1_RefEnum));
            try
            {
                //Creacion de los Task
                inputTask = new Task();
                outputTask = new Task();

                //Config de los canales
                inputTask.AIChannels.CreateVoltageChannel(
                    input_channel,
                    "",
                    AITerminalConfiguration.Rse,
                    input_values["Min"],                //MinVal
                    input_values["Max"],                //MaxVal
                    AIVoltageUnits.Volts
                    );

                inputTask.Timing.ConfigureSampleClock(
                    "",
                    input_values["Hz"],                //Sample Rate
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples
                    );

                outputTask.AOChannels.CreateVoltageChannel(
                    output_channel,
                    "",
                    output_values["Min"],
                    output_values["Max"],
                    AOVoltageUnits.Volts
                    );

                inputTask.Control(TaskAction.Verify);
                outputTask.Control(TaskAction.Verify);



                writer = new AnalogSingleChannelWriter(outputTask.Stream);
                reader = new AnalogSingleChannelReader(inputTask.Stream);

                Ref_Enum.MoveNext(); //Inicio el numerador en el primer elemento
                Calib_Data[0].Clear();
                Calib_Data[1].Clear();

                writer.WriteSingleSample(true, Convert.ToDouble(Ref_Enum.Current)); //Escribe el primero dato. 

                InputCallback = new AsyncCallback(Callback_Function);                   //Asocia la función CallBack definiendo el delegado (el AsyncCallback es el delegado) 
                reader.SynchronizeCallbacks = true;                                     
                reader.BeginReadMultiSample(samples_per_step, InputCallback, inputTask);    //Se la entrega al reader. 

                //Se aísla la ejecución de la calibración. 
                Same_CalibButt.Enabled = false;
                Opos_CalibButt.Enabled = false;
                Same_SaveCalib.Enabled = false;
                Opos_SaveCalib.Enabled = false;

                //Console.WriteLine(object.ReferenceEquals(Ref_Enum, Calib1_RefEnum));
                //Console.WriteLine(object.ReferenceEquals(InputCallback, Calib1_InputCallback));

            }

            catch (Exception ex)
            {
                StopTask(calib);
                MessageBox.Show(ex.Message);
            }


        }

        private void CalibInputRead(

            //DAC Connection Ojects
            ref AnalogSingleChannelReader reader,
            ref AnalogSingleChannelWriter writer,
            ref Task inputTask,
            ref AsyncCallback InputCallback,
            ref bool taskRunning,
            int calib,

            //Data Management
            ref IEnumerator Ref_Enum,
            ref List<List<double>> Calib_Data,
            ref double[][] Calib_Array,
            IAsyncResult ar, 

            //Form Management
            ref ProgressBar Progress_Bar,
            ref System.Windows.Forms.DataVisualization.Charting.Chart Chart,
            ref Button calibA_button,
            ref Button calibB_button,
            ref Button saveA_button,
            ref Button saveB_button
            )
        {
            try
            {
                double[] data = reader.EndReadMultiSample(ar);
                double data_mean = data.Sum() / data.Length;

                Calib_Data[0].Add(Convert.ToDouble(Ref_Enum.Current));  //guardo el Valor de Referencia 
                Calib_Data[1].Add(data_mean);                           //Guardo el correspondiente medido 

                if ((Ref_Enum.MoveNext()) && (taskRunning == true))
                {
                    writer.WriteSingleSample(true, Convert.ToDouble(Ref_Enum.Current));
                    reader.BeginReadMultiSample(samples_per_step, InputCallback, inputTask);
                    Progress_Bar.Value += 1; 
                }
                else if (taskRunning == false)
                {
                    StopTask(calib);
                    Ref_Enum.Reset();
                    Progress_Bar.Value = 0;
                    calibA_button.Enabled = true;
                    calibB_button.Enabled = true;
                }
                else
                {
                    StopTask(calib);

                    Calib_Array[0] = Calib_Data[0].ToArray();
                    Calib_Array[1] = Calib_Data[1].ToArray();

                    Array.Sort(Calib_Array[1], Calib_Array[0]);
                    Ref_Enum.Reset();
                    Progress_Bar.Value += 5;

                    Chart.Series[0].Points.Clear();
                    Chart.Series[0].Points.DataBindXY(Calib_Array[0], Calib_Array[1]);

                    Progress_Bar.Value += 5;
                    calibA_button.Enabled = true;
                    calibB_button.Enabled = true;
                    saveA_button.Enabled = true;
                    saveB_button.Enabled = true;
                }                
            }
            catch (Exception ex)
            {
                StopTask(calib);
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveCalib(ref System.Windows.Forms.SaveFileDialog SaveDialog, ref double[][] Calib_Array, ref ProgressBar Progress_Bar)
        {
            SaveDialog.InitialDirectory = Directory.GetCurrentDirectory();
            SaveDialog.RestoreDirectory = true;
            SaveDialog.Title = "Browse Text Files";
            SaveDialog.DefaultExt = "txt";
            SaveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveDialog.FilterIndex = 2;
            SaveDialog.CheckFileExists = true;
            SaveDialog.CheckPathExists = true;

            if (!(Calib_Array[0] != null)) //Está en doble negativo, porque si el arreglo aún no está instanciado; eso no significa que sea == null. 
            {
                MessageBox.Show("El Perfil de Calibración no tiene datos");
            }
            else if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                String PATH = SaveDialog.FileName;
                var calib_sb = new StringBuilder();

                for (int i = 0; i < Calib_Array[0].Length; i++)
                {
                    calib_sb.Append(Calib_Array[0][i].ToString());
                    calib_sb.Append(",");
                    calib_sb.AppendLine(Calib_Array[1][i].ToString());
                }

                File.WriteAllText(PATH, calib_sb.ToString());
                Progress_Bar.Value = 0;
            }


        }

        private void StopTask(int calib)
        {
            if (calib == 1)
            {


                Calib1_inputTask.Stop();
                Calib1_outputTask.Stop();
                Calib1_inputTask.Dispose();
                Calib1_outputTask.Dispose();
            }
            else if (calib == 2)
            {
                Calib2_inputTask.Stop();
                Calib2_outputTask.Stop();
                Calib2_inputTask.Dispose();
                Calib2_outputTask.Dispose();
            }
        }

        // ####################################### Particular Use Funcions


        private void DoCalib1_button_Click(object sender, EventArgs e)
        {
            String In_Ch = Calib1In_comboBox.Text;
            String Out_Ch = Calib1Out_comboBox.Text;
            DoCalib2_button.Enabled = false;
            Calib1_taskRunning = true;

            DoCalib(
                //Channels Values
                channels: new[] { In_Ch, Out_Ch },
                input_values: Ch_Dict[In_Ch],
                output_values: Ch_Dict[Out_Ch],

                //DAC Connection Ojects
                inputTask: ref Calib1_inputTask,
                outputTask: ref Calib1_outputTask,
                writer: ref Calib1_writer,
                reader: ref Calib1_reader,
                InputCallback: ref Calib1_InputCallback,
                Callback_Function: Calib1_Temp_InputRead,
                calib: 1,

                //Array Management
                Ref_Enum: ref Calib1_RefEnum,
                Calib_Data: ref Calib1_Data,

                //Form Management
                Progress_Bar: ref Calib1_progressBar,
                Same_CalibButt: ref DoCalib1_button,
                Opos_CalibButt: ref DoCalib2_button,
                Same_SaveCalib: ref SaveCalib1_button,
                Opos_SaveCalib: ref SaveCalib2_button
            );

        }

        private void DoCalib2_button_Click(object sender, EventArgs e)
        {
            String In_Ch = Calib2In_comboBox.Text;
            String Out_Ch = Calib2Out_comboBox.Text;
            DoCalib1_button.Enabled = false;
            Calib2_taskRunning = true;

            DoCalib(
                //Channels Values
                channels: new[] { In_Ch, Out_Ch },
                input_values: Ch_Dict[In_Ch],
                output_values: Ch_Dict[Out_Ch],

                //DAC Connection Ojects
                inputTask: ref Calib2_inputTask,
                outputTask: ref Calib2_outputTask,
                writer: ref Calib2_writer,
                reader: ref Calib2_reader,
                InputCallback: ref Calib2_InputCallback,
                Callback_Function: Calib2_Temp_InputRead,
                calib: 2,

                //Array Management
                Ref_Enum: ref Calib2_RefEnum,
                Calib_Data: ref Calib2_Data,

                //Form Management
                Progress_Bar: ref Calib2_progressBar,
                Same_CalibButt: ref DoCalib2_button,
                Opos_CalibButt: ref DoCalib1_button,
                Same_SaveCalib: ref SaveCalib2_button,
                Opos_SaveCalib: ref SaveCalib1_button
            );



        }


        private void Calib1_Temp_InputRead(IAsyncResult Calib1_AsyncResult)
        {
            CalibInputRead(

                //DAC Connection Ojects
                reader          : ref Calib1_reader,
                writer          : ref Calib1_writer, 
                inputTask       : ref Calib1_inputTask,
                InputCallback   : ref Calib1_InputCallback,
                taskRunning     : ref Calib1_taskRunning,
                calib           : 1,

                //Data Management
                Ref_Enum        : ref Calib1_RefEnum,
                Calib_Data      : ref Calib1_Data,
                Calib_Array     : ref Calib1_Array,
                ar              : Calib1_AsyncResult,

                //Form Management
                Progress_Bar    : ref Calib1_progressBar,
                Chart           : ref Calib1_chart,
                calibA_button   : ref DoCalib1_button,
                calibB_button   : ref DoCalib2_button,
                saveA_button    : ref SaveCalib1_button,
                saveB_button    : ref SaveCalib2_button
                );
        }

        private void Calib2_Temp_InputRead(IAsyncResult Calib2_AsyncResult)
        {
            CalibInputRead(

                //DAC Connection Ojects
                reader: ref Calib2_reader,
                writer: ref Calib2_writer,
                inputTask: ref Calib2_inputTask,
                InputCallback: ref Calib2_InputCallback,
                taskRunning: ref Calib2_taskRunning,
                calib: 2,

                //Data Management
                Ref_Enum: ref Calib2_RefEnum,
                Calib_Data: ref Calib2_Data,
                Calib_Array: ref Calib2_Array,
                ar: Calib2_AsyncResult,

                //Form Management
                Progress_Bar: ref Calib2_progressBar,
                Chart: ref Calib2_chart,
                calibA_button: ref DoCalib2_button,
                calibB_button: ref DoCalib1_button,
                saveA_button: ref SaveCalib2_button,
                saveB_button: ref SaveCalib1_button
                );
        }
                

        private void SaveCalib1_button_Click(object sender, EventArgs e)
        {
            SaveCalib(
                ref Calib1_saveFileDialog, 
                ref Calib1_Array,
                ref Calib1_progressBar
                );

        }

        private void SaveCalib2_button_Click(object sender, EventArgs e)
        {
            SaveCalib(
                ref Calib2_saveFileDialog, 
                ref Calib2_Array,
                ref Calib2_progressBar
                );
        }

        
        private void StopCalib1_button_Click(object sender, EventArgs e)
        {
            Calib1_taskRunning = false;
            
        }

        private void StopCalib2_button_Click(object sender, EventArgs e)
        {
            Calib2_taskRunning = false;
        }
    }
}
