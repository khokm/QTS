using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QTS.Core
{
    public abstract class InteractiveDiagram
    {
        int currentVisibleIndex = 0;
        event Action viewUpdated;

        public int PathCount { get; set; }

        protected InteractiveDiagram()
        {
            showPreviousLines = true;
        }

        /// <summary>
        /// Добавляет точку на текущую линию.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public abstract void AddPathPoint(double y, double x);

        /// <summary>
        /// Вызывается при создании нового пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// 
        protected abstract void StartPath(double y, double x);

        public void OnPathStarted(double y, double x)
        {
            PathCount++;
            StartPath(y, x);
        }

        public abstract void AddPathMetadata(string metadata);

        /// <summary>
        /// Вызывается по окончании создания пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public abstract void OnPathFinished(double y, double x, int clientNumber);

        /// <summary>
        /// Вызывается по окончании создания диаграммы
        /// </summary>
        public abstract void PostProcessDiagram();

        /// <summary>
        /// Вызывается при изменении отображения диаграммы
        /// </summary>
        /// <param name="visibleLineIndex"></param>
        public abstract void UpdateView(int visibleLineIndex);

        /// <summary>
        /// Устанавливает количество отображаемых линий
        /// </summary>
        /// <param name="lineIndex">Индекс последней отображаемой линии</param>
        protected void SetVisibleLinesCount(int lineIndex)
        {
            currentVisibleIndex = lineIndex;
            UpdateView(currentVisibleIndex);

            viewUpdated?.Invoke();
        }

        bool showPreviousLines;
        public bool ShowPreviousLines
        {
            get
            {
                return showPreviousLines;
            }
            set
            {
                showPreviousLines = value;
                SetVisibleLinesCount(currentVisibleIndex);
            }
        }

        public void GoToStart()
        {
            SetVisibleLinesCount(0);
        }

        public void GoToEnd()
        {
            SetVisibleLinesCount(PathCount - 1);
        }

        public void StepForward()
        {
            if (currentVisibleIndex == PathCount - 1)
                return;

            SetVisibleLinesCount(currentVisibleIndex + 1);
        }

        public void StepBack()
        {
            if (currentVisibleIndex == 0)
                return;

            SetVisibleLinesCount(currentVisibleIndex - 1);
        }

        public event Action ViewUpdated
        {
            add
            {
                viewUpdated += value;
            }

            remove
            {
                viewUpdated -= value;
            }
        }
    }
}
