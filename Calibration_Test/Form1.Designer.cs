
namespace Calibration_Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputChanGroupBox = new System.Windows.Forms.GroupBox();
            this.inputChannelComboBox = new System.Windows.Forms.ComboBox();
            this.inputMinValNumeric = new System.Windows.Forms.NumericUpDown();
            this.inputChanLabel = new System.Windows.Forms.Label();
            this.inputMaxValLabel = new System.Windows.Forms.Label();
            this.inputMinValLabel = new System.Windows.Forms.Label();
            this.inputMaxValNumeric = new System.Windows.Forms.NumericUpDown();
            this.outputChanGroupBox = new System.Windows.Forms.GroupBox();
            this.outputChannelComboBox = new System.Windows.Forms.ComboBox();
            this.outputMinValNumeric = new System.Windows.Forms.NumericUpDown();
            this.outputMaxValLabel = new System.Windows.Forms.Label();
            this.outputMinValLabel = new System.Windows.Forms.Label();
            this.outputMaxValNumeric = new System.Windows.Forms.NumericUpDown();
            this.outputChanLabel = new System.Windows.Forms.Label();
            this.calibration_button = new System.Windows.Forms.Button();
            this.send_signal = new System.Windows.Forms.Button();
            this.read_signal = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.inputChanGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputMinValNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputMaxValNumeric)).BeginInit();
            this.outputChanGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputMinValNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputMaxValNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // inputChanGroupBox
            // 
            this.inputChanGroupBox.Controls.Add(this.inputChannelComboBox);
            this.inputChanGroupBox.Controls.Add(this.inputMinValNumeric);
            this.inputChanGroupBox.Controls.Add(this.inputChanLabel);
            this.inputChanGroupBox.Controls.Add(this.inputMaxValLabel);
            this.inputChanGroupBox.Controls.Add(this.inputMinValLabel);
            this.inputChanGroupBox.Controls.Add(this.inputMaxValNumeric);
            this.inputChanGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputChanGroupBox.Location = new System.Drawing.Point(25, 27);
            this.inputChanGroupBox.Name = "inputChanGroupBox";
            this.inputChanGroupBox.Size = new System.Drawing.Size(328, 112);
            this.inputChanGroupBox.TabIndex = 2;
            this.inputChanGroupBox.TabStop = false;
            this.inputChanGroupBox.Text = "Channel Parameters - Input";
            // 
            // inputChannelComboBox
            // 
            this.inputChannelComboBox.Location = new System.Drawing.Point(152, 24);
            this.inputChannelComboBox.Name = "inputChannelComboBox";
            this.inputChannelComboBox.Size = new System.Drawing.Size(168, 21);
            this.inputChannelComboBox.TabIndex = 1;
            this.inputChannelComboBox.Text = "Dev1/ai0";
            // 
            // inputMinValNumeric
            // 
            this.inputMinValNumeric.DecimalPlaces = 2;
            this.inputMinValNumeric.Location = new System.Drawing.Point(152, 80);
            this.inputMinValNumeric.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.inputMinValNumeric.Name = "inputMinValNumeric";
            this.inputMinValNumeric.Size = new System.Drawing.Size(168, 20);
            this.inputMinValNumeric.TabIndex = 5;
            this.inputMinValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            // 
            // inputChanLabel
            // 
            this.inputChanLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputChanLabel.Location = new System.Drawing.Point(16, 26);
            this.inputChanLabel.Name = "inputChanLabel";
            this.inputChanLabel.Size = new System.Drawing.Size(96, 16);
            this.inputChanLabel.TabIndex = 0;
            this.inputChanLabel.Text = "Input Channel:";
            // 
            // inputMaxValLabel
            // 
            this.inputMaxValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputMaxValLabel.Location = new System.Drawing.Point(16, 54);
            this.inputMaxValLabel.Name = "inputMaxValLabel";
            this.inputMaxValLabel.Size = new System.Drawing.Size(96, 16);
            this.inputMaxValLabel.TabIndex = 2;
            this.inputMaxValLabel.Text = "Maximum Value:";
            // 
            // inputMinValLabel
            // 
            this.inputMinValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputMinValLabel.Location = new System.Drawing.Point(16, 82);
            this.inputMinValLabel.Name = "inputMinValLabel";
            this.inputMinValLabel.Size = new System.Drawing.Size(96, 16);
            this.inputMinValLabel.TabIndex = 4;
            this.inputMinValLabel.Text = "Minimum Value:";
            // 
            // inputMaxValNumeric
            // 
            this.inputMaxValNumeric.DecimalPlaces = 2;
            this.inputMaxValNumeric.Location = new System.Drawing.Point(152, 52);
            this.inputMaxValNumeric.Name = "inputMaxValNumeric";
            this.inputMaxValNumeric.Size = new System.Drawing.Size(168, 20);
            this.inputMaxValNumeric.TabIndex = 3;
            this.inputMaxValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // outputChanGroupBox
            // 
            this.outputChanGroupBox.Controls.Add(this.outputChannelComboBox);
            this.outputChanGroupBox.Controls.Add(this.outputMinValNumeric);
            this.outputChanGroupBox.Controls.Add(this.outputMaxValLabel);
            this.outputChanGroupBox.Controls.Add(this.outputMinValLabel);
            this.outputChanGroupBox.Controls.Add(this.outputMaxValNumeric);
            this.outputChanGroupBox.Controls.Add(this.outputChanLabel);
            this.outputChanGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputChanGroupBox.Location = new System.Drawing.Point(25, 147);
            this.outputChanGroupBox.Name = "outputChanGroupBox";
            this.outputChanGroupBox.Size = new System.Drawing.Size(328, 112);
            this.outputChanGroupBox.TabIndex = 3;
            this.outputChanGroupBox.TabStop = false;
            this.outputChanGroupBox.Text = "Channel Parameters - Output";
            // 
            // outputChannelComboBox
            // 
            this.outputChannelComboBox.Location = new System.Drawing.Point(152, 24);
            this.outputChannelComboBox.Name = "outputChannelComboBox";
            this.outputChannelComboBox.Size = new System.Drawing.Size(168, 21);
            this.outputChannelComboBox.TabIndex = 1;
            this.outputChannelComboBox.Text = "Dev1/ao0";
            // 
            // outputMinValNumeric
            // 
            this.outputMinValNumeric.DecimalPlaces = 2;
            this.outputMinValNumeric.Location = new System.Drawing.Point(152, 80);
            this.outputMinValNumeric.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.outputMinValNumeric.Name = "outputMinValNumeric";
            this.outputMinValNumeric.Size = new System.Drawing.Size(168, 20);
            this.outputMinValNumeric.TabIndex = 5;
            this.outputMinValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            // 
            // outputMaxValLabel
            // 
            this.outputMaxValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputMaxValLabel.Location = new System.Drawing.Point(16, 54);
            this.outputMaxValLabel.Name = "outputMaxValLabel";
            this.outputMaxValLabel.Size = new System.Drawing.Size(96, 16);
            this.outputMaxValLabel.TabIndex = 2;
            this.outputMaxValLabel.Text = "Maximum Value:";
            // 
            // outputMinValLabel
            // 
            this.outputMinValLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputMinValLabel.Location = new System.Drawing.Point(16, 82);
            this.outputMinValLabel.Name = "outputMinValLabel";
            this.outputMinValLabel.Size = new System.Drawing.Size(96, 16);
            this.outputMinValLabel.TabIndex = 4;
            this.outputMinValLabel.Text = "Minimum Value:";
            // 
            // outputMaxValNumeric
            // 
            this.outputMaxValNumeric.DecimalPlaces = 2;
            this.outputMaxValNumeric.Location = new System.Drawing.Point(152, 52);
            this.outputMaxValNumeric.Name = "outputMaxValNumeric";
            this.outputMaxValNumeric.Size = new System.Drawing.Size(168, 20);
            this.outputMaxValNumeric.TabIndex = 3;
            this.outputMaxValNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // outputChanLabel
            // 
            this.outputChanLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputChanLabel.Location = new System.Drawing.Point(16, 26);
            this.outputChanLabel.Name = "outputChanLabel";
            this.outputChanLabel.Size = new System.Drawing.Size(96, 16);
            this.outputChanLabel.TabIndex = 0;
            this.outputChanLabel.Text = "Output Channel:";
            // 
            // calibration_button
            // 
            this.calibration_button.Location = new System.Drawing.Point(157, 288);
            this.calibration_button.Name = "calibration_button";
            this.calibration_button.Size = new System.Drawing.Size(75, 23);
            this.calibration_button.TabIndex = 4;
            this.calibration_button.Text = "Calibrar";
            this.calibration_button.UseVisualStyleBackColor = true;
            this.calibration_button.Click += new System.EventHandler(this.calibration_button_Click);
            // 
            // send_signal
            // 
            this.send_signal.Enabled = false;
            this.send_signal.Location = new System.Drawing.Point(417, 108);
            this.send_signal.Name = "send_signal";
            this.send_signal.Size = new System.Drawing.Size(103, 30);
            this.send_signal.TabIndex = 9;
            this.send_signal.Text = "Enviar Señal";
            this.send_signal.UseVisualStyleBackColor = true;
            this.send_signal.Click += new System.EventHandler(this.send_signal_Click);
            // 
            // read_signal
            // 
            this.read_signal.Location = new System.Drawing.Point(417, 72);
            this.read_signal.Name = "read_signal";
            this.read_signal.Size = new System.Drawing.Size(103, 30);
            this.read_signal.TabIndex = 8;
            this.read_signal.Text = "Cargar \'signal.txt\'";
            this.read_signal.UseVisualStyleBackColor = true;
            this.read_signal.Click += new System.EventHandler(this.read_signal_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(414, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Frecuencia de Muestreo [Hz]";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(417, 43);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 339);
            this.Controls.Add(this.send_signal);
            this.Controls.Add(this.read_signal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.calibration_button);
            this.Controls.Add(this.inputChanGroupBox);
            this.Controls.Add(this.outputChanGroupBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.inputChanGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inputMinValNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputMaxValNumeric)).EndInit();
            this.outputChanGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outputMinValNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputMaxValNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox inputChanGroupBox;
        private System.Windows.Forms.ComboBox inputChannelComboBox;
        private System.Windows.Forms.NumericUpDown inputMinValNumeric;
        private System.Windows.Forms.Label inputChanLabel;
        private System.Windows.Forms.Label inputMaxValLabel;
        private System.Windows.Forms.Label inputMinValLabel;
        private System.Windows.Forms.NumericUpDown inputMaxValNumeric;
        private System.Windows.Forms.GroupBox outputChanGroupBox;
        private System.Windows.Forms.ComboBox outputChannelComboBox;
        private System.Windows.Forms.NumericUpDown outputMinValNumeric;
        private System.Windows.Forms.Label outputMaxValLabel;
        private System.Windows.Forms.Label outputMinValLabel;
        private System.Windows.Forms.NumericUpDown outputMaxValNumeric;
        private System.Windows.Forms.Label outputChanLabel;
        private System.Windows.Forms.Button calibration_button;
        private System.Windows.Forms.Button send_signal;
        private System.Windows.Forms.Button read_signal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}

