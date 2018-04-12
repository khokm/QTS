using System.Drawing;

namespace QTS.Core
{
    public interface IGraph
    {
        void SetTitle(string name);
        void AddLine(string name);
        void CompleteLine();
        void AddPoint(double y, double x);
        Bitmap ExportToBitmap();
    }
}
