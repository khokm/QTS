using System;
using System.Windows.Forms;

namespace QTS.WinForms
{
    public partial class EnterServicePlaceCountForm : Form
    {
        public EnterServicePlaceCountForm()
        {
            InitializeComponent();
        }

        public int minumumPlaces { get; private set; }
        public int maximumPlaces { get; private set; }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            int min = (int)FromNumeric.Value;
            int max = (int)ToNumeric.Value;
            int count = max - min;

            if (count <= 0 || min < 0 || max < 0)
            {
                MessageBox.Show("Некорректные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            minumumPlaces = min;
            maximumPlaces = max;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}