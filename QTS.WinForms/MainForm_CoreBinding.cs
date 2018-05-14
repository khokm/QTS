using System.Windows.Forms;
using QTS.Core;
using QTS.OxyPlotGraphics;

namespace QTS.WinForms
{
    /*
     * Реализация главной формы как пользовательского интерфейса для обратной связи
     */

    partial class MainForm : ICallbackUi<OxyPlotDiagram>
    {
        public void InvalidateDiagramView() => plot1.InvalidatePlot(false);

        public void SetDiagramView(OxyPlotDiagram diagram)
        {
            plot1.Model = diagram.plotModel;
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

        public string GetFolderPath(string description)
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

        public void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
