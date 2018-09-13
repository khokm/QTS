using System.Windows.Forms;
using QTS.Core;
using QTS.OxyPlotGraphics;
using System.Diagnostics;
using System;

namespace QTS.WinForms
{
    /*
     * Реализация главной формы как пользовательского интерфейса для обратной связи
     */

    partial class MainForm : ICallbackUi
    {
        public void InvalidateDiagramView() => plot1.InvalidatePlot(false);

        public ParametersContainer GetDiagramParameters() => ParseDiagramParameters();

        public void SetDiagramView(TimeDiagram diagram)
        {
            plot1.Model = (diagram as OxyPlotDiagram)?.plotModel;
        }

        public void RemoveDiagramView()
        {
            plot1.Model = null;
            showPrevLines_CheckBox.Checked = true;
        }

        public bool YesNoDialog(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        public string GetFolderPath(string description, string defaultFolder)
        {
            using (var fbd = new FolderBrowserDialog() { Description = description, SelectedPath = defaultFolder })
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return fbd.SelectedPath;
            }

            return "";
        }

        public void StartExplorer(string path)
        {
            Process.Start(path);
        }

        public void ShowTextWindow(string title, string text)
        {
            new TextWindow(text).Show();
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public QueuePlaceGradientData GetQueuePlaceGradientData()
        {
            using (var form = new EnterServicePlaceCountForm())
            {

                if (form.ShowDialog() != DialogResult.OK)
                    return null;

                return form.gradientData;
            }
        }
    }
}
