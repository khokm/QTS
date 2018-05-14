using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OxyPlot;
using QTS.Core;
using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    public partial class MainForm : Form
    {
        QtsController<OxyPlotDiagram> controller;
        bool rndValueChanged = false;

        public MainForm()
        {
            InitializeComponent();

            controller = new QtsController<OxyPlotDiagram>(this, new OxyPlotFactory());

            /*
             * OxyPlot по умолчанию уже использует
             * клавиши стрелок для движения по диаграмме,
             * так что отцепим их
             */
            var keyBinder = new PlotController();
            keyBinder.UnbindKeyDown(OxyKey.Up);
            keyBinder.UnbindKeyDown(OxyKey.Down);
            keyBinder.UnbindKeyDown(OxyKey.Left);
            keyBinder.UnbindKeyDown(OxyKey.Right);

            plot1.Controller = keyBinder;
            //tests
            //
            // var pars = new ParametersContainer(10, 2, 0, false, -5, true, 1, false, new[] { 5, 3, 100 });
            //var pars = new ParametersContainer(10, 2, 0, true, 10, false, 2, false, new int[] { });
            //controller.MakeDiagram(pars);
            //controller.MakeSynthesis(pars, 1, 0);
        }

        /// <summary>
        /// Считывает данные с правой панели.
        /// </summary>
        /// <returns>Набор параметров для построения диаграммы.</returns>
        private ParametersContainer ParseDiagramParameters()
        {
            int threadIntencity = (int)threadIntencity_Numeric.Value;
            int queuePlaceCount = (int)parkPlace_Numeric.Value;
            double minRndValue = (double)minRnd_Numeric.Value;
            bool timeLimit = timeLimit_CheckBox.Checked;
            double maxTime = (double)timeLimit_Numeric.Value;
            bool clientsLimit = clientLimit_CheckBox.Checked;
            int maxClients = (int)clientLimit_Numeric.Value;
            bool preferFirstChannel = preferFirstChannel_CheckBox.Checked;

            List<int> channels = new List<int>();
            foreach (var item in channelIntencites.Items)
                channels.Add(int.Parse(item.ToString().Substring(3)));

            return new ParametersContainer(threadIntencity, queuePlaceCount, minRndValue, timeLimit, maxTime, clientsLimit, maxClients, preferFirstChannel, channels.ToArray());
        }

        #region Обработчики кнопок правой панели
        private void addChannelIntencity_Button_Click(object sender, EventArgs e)
        {
            EnterValueForm valueForm = new EnterValueForm(1);
            valueForm.ShowDialog();

            if (valueForm.DialogResult == DialogResult.OK)
            {
                channelIntencites.Items.Add(string.Format("{0}. {1}", channelIntencites.Items.Count + 1, valueForm.Value));
                deleteChannelIntencity_Button.Enabled = true;
            }
        }

        private void deleteChannelIntencity_Button_Click(object sender, EventArgs e)
        {
            if (channelIntencites.SelectedIndex != -1)
                channelIntencites.Items.RemoveAt(channelIntencites.SelectedIndex);

            if (channelIntencites.Items.Count == 0)
                deleteChannelIntencity_Button.Enabled = false;
            else
            {
                //rename channels
                var items = channelIntencites.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    string text = items[i].ToString().Substring(3);
                    items[i] = string.Format("{0}. {1}", i + 1, text);
                }
            }
        }

        private void channelIntencites_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = channelIntencites.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches)
                return;

            var tmpValue = channelIntencites.Items[channelIntencites.SelectedIndex].ToString();
            EnterValueForm valueForm = new EnterValueForm(int.Parse(tmpValue.Substring(3)));
            valueForm.ShowDialog();

            if (valueForm.DialogResult == DialogResult.OK)
                channelIntencites.Items[channelIntencites.SelectedIndex] = string.Format("{0}. {1}", index + 1, valueForm.Value);
        }

        private void timeLimit_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            timeLimit_Numeric.Enabled = timeLimit_CheckBox.Checked;
        }

        private void clientLimit_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            clientLimit_Numeric.Enabled = clientLimit_CheckBox.Checked;
        }

        private void minRnd_Numeric_ValueChanged(object sender, EventArgs e)
        {
            if (!rndValueChanged)
            {
                MessageBox.Show("Изменение минимального значения ГСЧ влияет на точность модели.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rndValueChanged = true;
            }
        }
        #endregion

        #region Обработчики кнопок верхней панели (управление отображением диаграммы)
        private void toStart_Button_Click(object sender, EventArgs e)
        {
            controller.GoToDiagramStart();
        }

        private void stepBack_Button_Click(object sender, EventArgs e)
        {
            controller.GoToDiagramPrev();
        }

        private void stepForward_Button_Click(object sender, EventArgs e)
        {
            controller.GoToDiagramNext();
        }

        private void toEnd_Button_Click(object sender, EventArgs e)
        {
            controller.GoToDiagramEnd();
        }

        private void showPrevLines_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controller.ShowPreviousLines(showPrevLines_CheckBox.Checked);
        }
        #endregion

        #region Обработчики кнопок меню
        private void анализДиаграммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.MakeDiagramAnalyze();
        }

        private void построитьГрафикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.MakeDiagram(ParseDiagramParameters());
        }

        private void синтезСМОToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parameters = ParseDiagramParameters();

            if (!controller.CheckParametersValid(parameters))
                return;

            var form = new EnterServicePlaceCountForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            controller.MakeSynthesis(parameters, form.MinumumQueuePlaces, form.MaximumQueuePlaces);
        }

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutControlsForm().ShowDialog();
        }
        #endregion

        #region Обработчик нажатия клавиш стрелок
        private void plot1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    toEnd_Button.PerformClick();
                    break;
                case Keys.Down:
                    toStart_Button.PerformClick();
                    break;
                case Keys.Left:
                    stepBack_Button.PerformClick();
                    break;
                case Keys.Right:
                    stepForward_Button.PerformClick();
                    break;
            }
        }
        #endregion
    }
}