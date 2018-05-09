using System;
using System.IO;
using System.Windows.Forms;

namespace QTS.WinForms
{
    /// <summary>
    /// Простое текстовое окно с возможностью сохранения содержимого в файл.
    /// </summary>
    public partial class TextWindow : Form
    {
        public TextWindow(string diagramAnalyze)
        {
            InitializeComponent();
            textBox1.Text = diagramAnalyze;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == DialogResult.OK)
                using (var writer = new StreamWriter(save.OpenFile()))
                    writer.WriteLine(textBox1.Text);

        }
    }
}
