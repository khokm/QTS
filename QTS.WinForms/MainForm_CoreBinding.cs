using System.Windows.Forms;
using QTS.Core;
using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    /*
     * Реализация главной формы как пользовательского интерфейса для обратной связи
     */

    partial class MainForm : ICallbackUi
    {
        public void InvalidateDiagramView() => plot1.Invalidate();

        public IGraph CreateGraph()
        {
            return new OxyPlotGraph();
        }

        public TimeDiagram CreateNewDiagram(int channelCount, int queueCapacity)
        {
            return new OxyPlotDiagram(channelCount, queueCapacity);
        }

        public void SetDiagramView(TimeDiagram diagram)
        {
            plot1.Model = ((OxyPlotDiagram)diagram).plotModel;
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

        public string GetImagePathFolder(string description)
        {
            using (var fbd = new FolderBrowserDialog() { Description = description })
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return fbd.SelectedPath;
            }

            return "";
        }

        public void ShowTextWindow(string title, string text)
        {
            new TextWindow(text).Show();
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
