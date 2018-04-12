using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OxyPlot;
using QTS.Core;
using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    public partial class MainForm : Form, IForm
    {
        QtsController controller;
        bool rndValueChanged = false;

        public MainForm()
        {
            InitializeComponent();

            controller = new QtsController(this);

            var keyBinder = new PlotController();
            keyBinder.UnbindKeyDown(OxyKey.Up);
            keyBinder.UnbindKeyDown(OxyKey.Down);
            keyBinder.UnbindKeyDown(OxyKey.Left);
            keyBinder.UnbindKeyDown(OxyKey.Right);

            plot1.Controller = keyBinder;
        }

        private Parameters ParseParameters()
        {
            int threadIntencity = (int)threadIntencity_Numeric.Value;
            int parkCount = (int)parkPlace_Numeric.Value;

            double minRndValue = (double)minRnd_Numeric.Value;

            double maxTime = (double)timeLimit_Numeric.Value;
            int maxClients = (int)clientLimit_Numeric.Value;

            bool clientsLimit = clientLimit_CheckBox.Checked;
            bool timeLimit = timeLimit_CheckBox.Checked;

            if (!timeLimit && !clientsLimit)
            {
                MessageBox.Show("Введите хотя бы одно ограничение.", "Ошибка вычислений", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            bool preferFirstChannel = preferFirstChannel_CheckBox.Checked;

            List<int> channels = new List<int>();
            foreach (var item in channelIntencites.Items)
                channels.Add(int.Parse(item.ToString().Substring(3)));

            if (channels.Count == 0)
            {
                MessageBox.Show("Система не имеет мест обслуживания.", "Ошибка вычислений", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new Parameters(threadIntencity, parkCount, channels.ToArray(), maxTime, maxClients, minRndValue, preferFirstChannel, clientsLimit, timeLimit);
        }

        // Some Buttons
        //
        private void addChannelIntencity_Button_Click(object sender, EventArgs e)
        {
            EnterValueForm valueForm = new EnterValueForm(1);
            valueForm.ShowDialog();

            if (valueForm.DialogResult == DialogResult.OK)
            {
                channelIntencites.Items.Add(string.Format("{0}. {1}", channelIntencites.Items.Count + 1, valueForm.value));
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
                channelIntencites.Items[channelIntencites.SelectedIndex] = string.Format("{0}. {1}", index + 1, valueForm.value);
        }

        private void showPrevLines_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controller.ShowPreviousLines(showPrevLines_CheckBox.Checked);
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

        private void plot1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
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

        //Diagram Controllers
        //
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

        //Buttons
        //
        private void анализДиаграммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.ShowDiagramAnalyze();
        }

        private void построитьГрафикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parameters = ParseParameters();

            if (parameters == null)
                return;

            controller.CreateTimeDiagram(parameters);
        }

        private void синтезСМОToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parameters = ParseParameters();

            if (parameters == null)
                return;

            var form = new EnterServicePlaceCountForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            controller.CreateGraphs(parameters, form.minumumPlaces, form.maximumPlaces);
        }

        //IForm implementation
        //
        Action IForm.GetFormUpdateAction()
        {
            return plot1.Invalidate;
        }

        void IForm.SetModel(TimeDiagram diagram)
        {
            plot1.Model = ((OxyPlotDiagram)diagram).plotModel;
        }

        void IForm.RemoveModel()
        {
            plot1.Model = null;
            showPrevLines_CheckBox.Checked = true;
        }

        bool IForm.YesNoDialog(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        TimeDiagram IForm.CreateDiagram(int channelCount, int queueCapacity)
        {
            return new OxyPlotDiagram(channelCount, queueCapacity);
        }

        IGraph IForm.CreateGraph()
        {
            return new OxyPlotGraph();
        }

        string IForm.GetImagePathFolder(string description)
        {
            using (var fbd = new FolderBrowserDialog() { Description =  description})
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return fbd.SelectedPath;
            }

            return "";
        }

        void IForm.ShowTextWindow(string title, string text)
        {
            new TextWindow(text).Show();
        }

        void IForm.ShowError(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}