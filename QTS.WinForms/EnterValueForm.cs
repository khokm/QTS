using System;
using System.Windows.Forms;

namespace QTS.WinForms
{
    /// <summary>
    /// Простое окно для ввода одного числа.
    /// </summary>
    public partial class EnterValueForm : Form
    {
        public int Value => (int)numericUpDown1.Value;

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
                Close();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}
