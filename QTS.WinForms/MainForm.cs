using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using OxyPlot;

using QTS.Core;
using QTS.Core.Diagram;

using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    public partial class MainForm : Form
    {
        QtsController controller;
        bool rndValueChanged = false;

        public MainForm()
        {
            InitializeComponent();

            ShowPreviousLines_ComboBox.SelectedIndex = 0;
            ShowGraphs_ComboBox.SelectedIndex = 0;

            plot1.Model = new PlotModel()
            {
                IsLegendVisible = false,
            };

            controller = new QtsController(this, new OxyPlotFactory());

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

            System.Globalization.CultureInfo myCulture =
     (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            myCulture.NumberFormat.NaNSymbol = "Не определено";

            System.Threading.Thread.CurrentThread.CurrentCulture = myCulture;
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

        private void SaveConfig()
        {
            try
            {
                var config = ParseDiagramParameters();

                using (SaveFileDialog dialog = new SaveFileDialog() { FileName = "config.xml" })
                {
                    if (dialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.FileName))
                        return;
                    XmlSerializer formatter = new XmlSerializer(typeof(ParametersContainer));
                    using (Stream fStream = new FileStream(dialog.FileName,
                       FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        formatter.Serialize(fStream, config);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                ShowError("Ошибка", "Не удалось сохранить конфигурацию");
            }
        }

        private void LoadConfig()
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog() { FileName = "config.xml" })
                {
                    if (dialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.FileName))
                        return;

                    XmlSerializer formatter = new XmlSerializer(typeof(ParametersContainer));
                    using (Stream fStream = new FileStream(dialog.FileName,
                       FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        var config = (ParametersContainer)formatter.Deserialize(fStream);

                        threadIntencity_Numeric.Value = config.ThreadIntencity;
                        parkPlace_Numeric.Value = config.QueueCapacity;
                        minRnd_Numeric.Value = (decimal)config.MinRandomValue;
                        timeLimit_CheckBox.Checked = config.HasTimeLimit;
                        timeLimit_Numeric.Value = (decimal)config.TimeLimit;
                        clientLimit_CheckBox.Checked = config.HasClientLimit;
                        clientLimit_Numeric.Value = config.ClientLimit;
                        preferFirstChannel_CheckBox.Checked = config.PreferFirstChannel;

                        channelIntencites.Items.Clear();
                        foreach (var item in config.ChannelsIntencites)
                            AddChannelIntencity(item);
                    }
                }
            }
            catch
            {
                ShowError("Ошибка", "Не удалось загрузить конфигурацию");
            }
        }

        private void AddChannelIntencity(int value)
        {
            channelIntencites.Items.Add(string.Format("{0}. {1}", channelIntencites.Items.Count + 1, value));
            deleteChannelIntencity_Button.Enabled = true;
        }

        #region Обработчики кнопок правой панели
        bool showNotify = true;
        private void addChannelIntencity_Button_Click(object sender, EventArgs e)
        {
            using (EnterValueForm valueForm = new EnterValueForm(1))
            {
                valueForm.ShowDialog();

                if (valueForm.DialogResult == DialogResult.OK)
                {
                    AddChannelIntencity(valueForm.Value);
                    if (showNotify)
                    {
                        ShowWarning("Подсказка", "Для редактирования пропускной способности канала используйте двойной клик мышью.");
                        showNotify = false;
                    }
                }
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
            using (EnterValueForm valueForm = new EnterValueForm(int.Parse(tmpValue.Substring(3))))
            {
                valueForm.ShowDialog();

                if (valueForm.DialogResult == DialogResult.OK)
                    channelIntencites.Items[channelIntencites.SelectedIndex] = string.Format("{0}. {1}", index + 1, valueForm.Value);
            }
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
        private void toStart_Button_Click(object sender, EventArgs e) => InteractiveDiagram?.GoToStart();

        private void stepBack_Button_Click(object sender, EventArgs e) => InteractiveDiagram?.StepBack();

        private void stepForward_Button_Click(object sender, EventArgs e) => InteractiveDiagram?.StepForward();

        private void toEnd_Button_Click(object sender, EventArgs e) => InteractiveDiagram?.GoToEnd();

        private void ShowPreviousLines_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InteractiveDiagram != null)
                InteractiveDiagram.ShowPreviousLines = ShowPreviousLines_ComboBox.SelectedIndex == 0;
        }

        private void ShowGraphs_ComboBox_SelectedIndexChanged(object sender, EventArgs e) => InteractiveDiagram?.SetLayer(ShowGraphs_ComboBox.SelectedIndex);
        #endregion

        #region Обработчики кнопок меню
        private void загрузитьКонфигурациюToolStripMenuItem_Click(object sender, EventArgs e) => LoadConfig();

        private void сохранитьКонфигурациюКакToolStripMenuItem_Click(object sender, EventArgs e) => SaveConfig();

        private void анализДиаграммыToolStripMenuItem_Click(object sender, EventArgs e) => controller.MakeDiagramAnalyze();

        private void построитьГрафикToolStripMenuItem_Click(object sender, EventArgs e) => controller.MakeDiagram();

        private void синтезСМОToolStripMenuItem_Click(object sender, EventArgs e) => controller.MakeSynthesis();

        private void улучшениеГрафикаToolStripMenuItem_Click(object sender, EventArgs e) => controller.MakeGraphImprovement();

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutControlsForm())
                about.ShowDialog();
        }
        #endregion

        #region Обработчик нажатия клавиш
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
                case Keys.Q:
                    ShowPreviousLines_ComboBox.SelectedIndex = -ShowPreviousLines_ComboBox.SelectedIndex + 1;
                    break;
                case Keys.W:
                    ShowGraphs_ComboBox.SelectedIndex = -ShowGraphs_ComboBox.SelectedIndex + 1;
                    break;
            }
        }
        #endregion
    }
}