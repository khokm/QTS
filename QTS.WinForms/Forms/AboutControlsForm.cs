using System;
using System.Windows.Forms;

namespace QTS.WinForms
{
    /// <summary>
    /// Окно помощи по управлению.
    /// </summary>
    public partial class AboutControlsForm : Form
    {
        /// <summary>
        /// Создает новое окно помощи по управлению.
        /// </summary>
        public AboutControlsForm()
        {
            InitializeComponent();
            ActiveControl = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
