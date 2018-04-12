namespace QTS.Core
{
    public interface IForm
    {
        System.Action GetFormUpdateAction();
        void RemoveModel();
        void SetModel(TimeDiagram diagram);
        bool YesNoDialog(string title, string message);
        TimeDiagram CreateDiagram(int channelCount, int queueCapacity);
        IGraph CreateGraph();
        string GetImagePathFolder(string description);
        void ShowTextWindow(string title, string text);
        void ShowError(string title, string text);
    }
}
