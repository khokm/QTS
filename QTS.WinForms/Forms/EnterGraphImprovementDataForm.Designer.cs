namespace QTS.WinForms
{
    partial class EnterGraphImprovementDataForm
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
            this.StartButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ExperimentCountNumeric = new System.Windows.Forms.NumericUpDown();
            this.Graphs_comboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.ExperimentCountNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(56, 103);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(194, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Построить семейство графиков";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "График:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Количество повторов экспериментов:";
            // 
            // ExperimentCountNumeric
            // 
            this.ExperimentCountNumeric.Location = new System.Drawing.Point(231, 57);
            this.ExperimentCountNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ExperimentCountNumeric.Name = "ExperimentCountNumeric";
            this.ExperimentCountNumeric.Size = new System.Drawing.Size(57, 20);
            this.ExperimentCountNumeric.TabIndex = 4;
            // 
            // Graphs_comboBox
            // 
            this.Graphs_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Graphs_comboBox.DropDownWidth = 600;
            this.Graphs_comboBox.FormattingEnabled = true;
            this.Graphs_comboBox.Location = new System.Drawing.Point(66, 12);
            this.Graphs_comboBox.Name = "Graphs_comboBox";
            this.Graphs_comboBox.Size = new System.Drawing.Size(222, 21);
            this.Graphs_comboBox.TabIndex = 5;
            // 
            // EnterGraphImprovementDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 138);
            this.Controls.Add(this.Graphs_comboBox);
            this.Controls.Add(this.ExperimentCountNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterGraphImprovementDataForm";
            this.Text = "Улучшение показателя графика";
            ((System.ComponentModel.ISupportInitialize)(this.ExperimentCountNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ExperimentCountNumeric;
        private System.Windows.Forms.ComboBox Graphs_comboBox;
    }
}