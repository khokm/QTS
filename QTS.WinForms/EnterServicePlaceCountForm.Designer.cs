namespace QTS.WinForms
{
    partial class EnterServicePlaceCountForm
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
            this.FromNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ToNumeric = new System.Windows.Forms.NumericUpDown();
            this.ApplyButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.FromNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // FromNumeric
            // 
            this.FromNumeric.Location = new System.Drawing.Point(55, 11);
            this.FromNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.FromNumeric.Name = "FromNumeric";
            this.FromNumeric.Size = new System.Drawing.Size(57, 20);
            this.FromNumeric.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "От:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "До:";
            // 
            // ToNumeric
            // 
            this.ToNumeric.Location = new System.Drawing.Point(179, 12);
            this.ToNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ToNumeric.Name = "ToNumeric";
            this.ToNumeric.Size = new System.Drawing.Size(57, 20);
            this.ToNumeric.TabIndex = 2;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(29, 42);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(207, 23);
            this.ApplyButton.TabIndex = 4;
            this.ApplyButton.Text = "Построить графики";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // EnterServicePlaceCountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 67);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ToNumeric);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FromNumeric);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterServicePlaceCountForm";
            this.Text = "Количество мест в очереди";
            ((System.ComponentModel.ISupportInitialize)(this.FromNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown FromNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ToNumeric;
        private System.Windows.Forms.Button ApplyButton;
    }
}