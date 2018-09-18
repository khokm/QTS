using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QTS.Core.Diagram;
using System.Drawing;
using System.Linq;

namespace QTS.WinForms
{
    public partial class EnterGraphImprovementDataForm : Form
    {
        public EnterGraphImprovementDataForm(string[] graphNames)
        {
            InitializeComponent();
            Graphs_comboBox.Items.AddRange(graphNames);
            Graphs_comboBox.SelectedIndex = 0;
            Font font = new Font("Microsoft Sans Serif", 9f);
            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(graphNames.OrderByDescending(s => s.Length).First(), font);
            Graphs_comboBox.DropDownWidth = (int)size.Width;
        }

        public GraphImprovementParamsData ImprovementData {get; private set;}

        private void StartButton_Click(object sender, EventArgs e)
        {
            int expCount = (int)ExperimentCountNumeric.Value;

            if (expCount <= 0)
            {
                MessageBox.Show("Количество повторов экспериментов должно быть больше 0.", "Улучшение графика", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImprovementData = new GraphImprovementParamsData(Graphs_comboBox.SelectedIndex, expCount);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
