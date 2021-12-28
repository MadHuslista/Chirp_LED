using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;

namespace Interface_Prototype
{
    public partial class Form1 : Form
    {
        List<double> OutCh1_Signal = new List<double>();
        List<double> OutCh1_Time = new List<double>();
        List<int> OutCh1_SegmentPos = new List<int>();

        List<double> OutCh2_Signal = new List<double>();
        List<double> OutCh2_Time = new List<double>();        

        private void OutCh1_AddCurrentSeg_button_Click(object sender, EventArgs e)
        {
            
            
            ref List<double> Signal = ref OutCh1_Signal;
            ref List<double> Time = ref OutCh1_Time;
            ref List<int> SegmentPos = ref OutCh1_SegmentPos; 
            double base_amp_value = Convert.ToDouble(OCh1Adap_baseAmp_numericUpDown.Value);

            //Form Management
            ref System.Windows.Forms.DataVisualization.Charting.Chart Chart = ref OutCh1_chart;
            

            SegmentPos.Add(Signal.Count);
            //Console.WriteLine("AddCurr{0}",Signal.Count);

            

            if (OutCh1_Adapt_radioButton.Checked) {
                

                AdaptSig(
                    signal:         ref Signal, 
                    time:           ref Time,
                    base_amp:       base_amp_value,
                    adapt_time:     Convert.ToDouble(OCh1Adap_Time_numericUpDown.Value),
                    hz:             Convert.ToDouble(OutCh1Hz_numericUpDown.Value) 
                    );                

                

            }
            else if (OutCh1_OnOff_radioButton.Checked) {
                
                OnOffSig(
                    signal:     ref Signal,
                    time:       ref Time, 
                    on_amp:     Convert.ToDouble(OCh1OnOff_Amp_numericUpDown.Value), 
                    on_time:    Convert.ToDouble(OCh1OnOff_OnDur_numericUpDown.Value),
                    off_time:   Convert.ToDouble(OCh1OnOff_OffDur_numericUpDown.Value),
                    hz:         Convert.ToDouble(OutCh1Hz_numericUpDown.Value)
                    );

                

            }
            else if (OutCh1_Freq_radioButton.Checked) {
                
                FreqSig(
                    signal              : ref Signal,
                    time                : ref Time,
                    freq_duration       : Convert.ToDouble(OCh1Freq_FreqDur_numericUpDown.Value),
                    freq_amp            : Convert.ToDouble(OCh1Freq_FreqAmp_numericUpDown.Value),
                    base_amp            : base_amp_value,
                    start_phase         : Convert.ToDouble(OCh1Freq_Phase_numericUpDown.Value),  
                    final_freq          : Convert.ToDouble(OCh1Freq_FinalFreq_numericUpDown.Value),
                    hz                  : Convert.ToDouble(OutCh1Hz_numericUpDown.Value)
                    );

                
            }
            else if (OutCh1_Amp_radioButton.Checked) {
                
                AmpSig(
                    signal              : ref Signal, 
                    time                : ref Time, 
                    amp_duration        : Convert.ToDouble(OCh1Amp_AmpDur_numericUpDown.Value), 
                    base_freq           : Convert.ToDouble(OCh1Amp_BaseFreq_numericUpDown.Value), 
                    max_amp             : Convert.ToDouble(OCh1Amp_MaxAmp_numericUpDown.Value), 
                    base_amp            : base_amp_value,
                    hz                  : Convert.ToDouble(OutCh1Hz_numericUpDown.Value)
                    );
                
            }

            UpdateChart(Time, Signal, ref Chart);
            
        }

        private void OutCh1_RemoveSeg_button_Click(object sender, EventArgs e)
        {

            ref List<double> sig = ref OutCh1_Signal;
            ref List<double> t = ref OutCh1_Time;
            ref List<int> segpos = ref OutCh1_SegmentPos;
            ref System.Windows.Forms.DataVisualization.Charting.Chart Chart = ref OutCh1_chart;


            RemoveSegment(
                Signal          : ref sig,
                Time            : ref t,
                SegmentPos      : ref segpos
                );


            UpdateChart(
                time            : t, 
                data            : sig, 
                Chart           : ref Chart);

        }



        //########### FUNCIONES DE USO GENERAL

        private void RemoveSegment(ref List<double> Signal, ref List<double> Time, ref List<int> SegmentPos)
        {
            
            if (Signal.Count > 0)
            {
                int last_pos = SegmentPos[SegmentPos.Count - 1];

                Signal.RemoveRange(last_pos, Signal.Count - last_pos);
                Time.RemoveRange(last_pos, Time.Count - last_pos);

                SegmentPos.RemoveAt(SegmentPos.Count - 1);

            }

        }

        private void AdaptSig(ref List<double> signal, ref List<double> time, double base_amp, double adapt_time, double hz)
        {
            int shape = Convert.ToInt32(adapt_time * hz);           
            
            double[] ones = Enumerable.Repeat(base_amp, shape).ToArray();
            
            double[] t = GenTime(ones.Length,ref time, adapt_time, hz);

            signal.AddRange(ones);
            time.AddRange(t); 
            
        }

        private void OnOffSig(ref List<double> signal, ref List<double> time, double on_amp, double on_time, double off_time, double hz)
        {
            int on_shape = Convert.ToInt32(on_time * hz);
            int off_shape = Convert.ToInt32(off_time * hz);

            double[] on_temp = Enumerable.Repeat(on_amp, on_shape).ToArray();
            double[] off_temp = Enumerable.Repeat(0.0 , off_shape).ToArray();

            double[] onoff = on_temp.Concat(off_temp).ToArray();
            double[] t = GenTime(onoff.Length, ref time, on_time + off_time, hz);

            signal.AddRange(onoff);
            time.AddRange(t);

        }

        private void FreqSig(ref List<double> signal, ref List<double> time, double freq_duration, double freq_amp, double base_amp, double start_phase, double final_freq, double hz)
        {
            double freq_acc = final_freq / freq_duration;
            double freq_shape = freq_duration * hz;
            start_phase = start_phase * Constants.Pi;

            double[] temp_t = Generate.LinearSpaced(Convert.ToInt32(freq_shape), 0, freq_duration - (1 / hz));
            List<double> freq_points = new List<double>();

            foreach (double t_point in temp_t)
            {
                double radian = start_phase + Constants.Pi * freq_acc * t_point * t_point;
                double f_point = Trig.Sin(radian) * freq_amp + base_amp;
                freq_points.Add(f_point); 
            }
           
            double [] t = GenTime(freq_points.Count, ref time, freq_duration, hz);

            signal.AddRange(freq_points);
            time.AddRange(t); 


        }

        private void AmpSig(ref List<double> signal, ref List<double> time, double amp_duration, double base_freq, double max_amp, double base_amp, double hz)       
        {

            double freq_shape = amp_duration * hz;

            double[] amp = Generate.Sinusoidal(Convert.ToInt32(freq_shape), hz, base_freq, 1);
            double[] increase = Generate.LinearSpaced(Convert.ToInt32(freq_shape), 0, max_amp);
            List<double> amp_signal = new List<double>();

            for (int i = 0; i < freq_shape; i++)
            {
                double amp_point = amp[i] * increase[i] + base_amp;
                amp_signal.Add(amp_point);
            }

            double[] t = GenTime(amp.Length, ref time, amp_duration, hz);

            signal.AddRange(amp_signal);
            time.AddRange(t); 
        }

        private double [] GenTime(int size, ref List<double> time, double duration, double hz)
        {
            double last_time;
            if (time.Count == 0) { last_time = 0.0; }
            else { last_time = time[time.Count - 1]; }

            double[] t = Generate.LinearSpaced(size, last_time, last_time + duration - (1 / hz));
            return t; 

        }


        
    }
}
