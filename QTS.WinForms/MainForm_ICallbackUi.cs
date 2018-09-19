using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using QTS.Core;
using QTS.Core.Graphics;
using QTS.Core.Diagram;

using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    /*
     * Реализация главной формы как пользовательского интерфейса для обратной связи
     */

    partial class MainForm : ICallbackUi
    {
        InteractiveDiagram diagram;

        public InteractiveDiagram InteractiveDiagram
        {
            get
            {
                return diagram;
            }

            set
            {
                ShowPreviousLines_ComboBox.SelectedIndex = 0;
                ShowGraphs_ComboBox.SelectedIndex = 0;

                plot1.Model = (value as OxyPlotGraph)?.PlotModel;
                diagram = value;
            }
        }

        public void InvalidateDiagramView() => plot1.InvalidatePlot(false);

        public ParametersContainer GetDiagramParameters() => ParseDiagramParameters();

        public bool YesNoDialog(string title, string message) => MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

        public void StartExplorer(string path) => Process.Start(path);

        public void ShowTextWindow(string title, string text) => new TextWindow(text).Show();

        public void ShowError(string title, string message) => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public void ShowWarning(string title, string message) => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public void CreateTextFile(string path, string text) => File.WriteAllText(path, text);

        public QueuePlaceGradientData GetQueuePlaceGradientData()
        {
            using (var form = new EnterServicePlaceCountForm() { StartPosition = FormStartPosition.CenterParent })
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return null;

                return form.gradientData;
            }
        }

        public GraphImprovementParamsData GetGraphImprovementParams(string[] names)
        {
            using (var form = new EnterGraphImprovementDataForm(names) { StartPosition = FormStartPosition.CenterParent })
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return null;

                return form.ImprovementData;
            }

        }

        public string GetFolderPath(string description, string defaultFolder)
        {
            bool done = false;
            while (!done)
            {
                done = true;
                using (var fbd = new FolderBrowserDialog() { Description = description, SelectedPath = defaultFolder })
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        try
                        {
                            string name = fbd.SelectedPath + "/" + new Random().Next() + ".test";
                            File.Create(name).Close();
                            File.Delete(name);
                            return fbd.SelectedPath;
                        }
                        catch
                        {
                            ShowError("Выбор папки", "Недостаточно прав для записи в папку");
                            done = false;
                        }
                    }
                }
            }

            return "";
        }

        public void LockInterface()
        {
            if (Enabled == false)
                return;

            SuspendLayout();
            Enabled = false;
            Application.DoEvents();
        }

        public void ShowText(string description)
        {
            statusText.Text = description;

            if (!statusText.Visible)
                statusText.Visible = true;

            statusText.Refresh();
        }

        public void HideText() => statusText.Visible = false;

        public void UnlockInterface()
        {
            if (Enabled == true)
                return;

            Enabled = true;
            ResumeLayout();
        }
    }
}
