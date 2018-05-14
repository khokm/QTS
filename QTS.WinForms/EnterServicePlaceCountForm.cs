using System;
using System.Windows.Forms;

namespace QTS.WinForms
{
    /// <summary>
    /// Окно для ввода параметров синтеза СМО.
    /// </summary>
    public partial class EnterServicePlaceCountForm : Form
    {
        public EnterServicePlaceCountForm()
        {
            InitializeComponent();
        }

        public int MinumumQueuePlaces { get; private set; }
        public int MaximumQueuePlaces { get; private set; }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            int min = (int)FromNumeric.Value;
            int max = (int)ToNumeric.Value;

            if (max <= min || min < 0 || max < 0)
            {
                MessageBox.Show("Некорректные значения градиента КМО", "Синтез СМО", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MinumumQueuePlaces = min;
            MaximumQueuePlaces = max;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}