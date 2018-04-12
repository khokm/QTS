namespace QTS.Core.Graphics
{
    interface IController
    {
        void GoToStart();
        void GoToEnd();
        void StepForward();
        void StepBack();
        bool showPreviousLines { get; set; }

        event System.Action OnVisualUpdated;
    }
}
