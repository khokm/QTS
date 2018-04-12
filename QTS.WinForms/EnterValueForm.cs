using System;
using System.Windows.Forms;

namespace QTS.WinForms
{
    public partial class EnterValueForm : Form
    {
        public int value { get { return (int)numericUpDown1.Value; } }
        public EnterValueForm(int value)
        {
            InitializeComponent();
            numericUpDown1.Value = value;

            numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
