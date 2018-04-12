using QTS.Core.Diagram;
using QTS.Core.Graphics;
using QTS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTS.Core
{
    public abstract class TimeDiagram : IDiagram, IAnalyzable, IController
    {
        struct Line
        {
            public double start, end;

            public Line(double start, double end)
            {
                this.start = start;
                this.end = end;
            }
        }

        double m_systemStartTime;
        double m_systemStopTime;
        bool m_diagramCompleted;

        double m_serviceTime;
        double m_queueWaitingTime;
        int m_clientsServed;
        int m_clientsLost;
        int m_queueClientCount;
        int m_clientsCount;

        int m_channelCount;
        int m_queueCapacity;

        List<Line>[] m_channelLines;
        List<Line>[] m_queueLines;
        double[] m_queueBusyTime;

        Line[][] m_inverseChannelLines;

        int m_currentVisibleIndex;

        bool m_showPreviousLines;

        int currentVisibleIndex
        {
            get
            {
                return m_currentVisibleIndex;
            }
        }

        protected int topY
        {
            get
            {
                return 2 + channelCount + queueCapacity;
            }
        }

        public event Action OnVisualUpdated;

        public TimeDiagram(int channelCount, int queueCapacity)
        {
            m_channelCount = channelCount;
            m_queueCapacity = queueCapacity;

            m_channelLines = new List<Line>[channelCount];
            for (int i = 0; i < channelCount; i++)
                m_channelLines[i] = new List<Line>();

            m_queueLines = new List<Line>[queueCapacity];
            for (int i = 0; i < queueCapacity; i++)
                m_queueLines[i] = new List<Line>();

            m_queueBusyTime = new double[queueCapacity];
            m_inverseChannelLines = new Line[channelCount][];

            m_showPreviousLines = true;
        }

        protected abstract void AddVisualPoint(double y, double x);

        protected abstract void OnLineStarted(double y, double x);

        public void PushStartPoint(double arrivalTime)
        {
            if (clientsCount == 0)
                m_systemStartTime = arrivalTime;

            int y = topY;

            OnLineStarted(y, arrivalTime);
            AddVisualPoint(y, arrivalTime);
        }

        public void PushChannelLine(int channelIndex, double start, double end)
        {
            m_channelLines[channelIndex].Add(new Line(start, end));

            m_serviceTime += end - start;

            int y = topY - 1 - channelIndex;
            AddVisualPoint(y, start);
            AddVisualPoint(y, end);
        }

        public void AddQueueClient()
        {
            m_queueClientCount++;
        }

        public void PushParkLine(int parkIndex, double start, double end)
        {
            m_queueLines[parkIndex].Add(new Line(start, end));

            double currentParkTime = end - start;
            m_queueBusyTime[parkIndex] += currentParkTime;

            if (parkIndex != 0)
                m_queueBusyTime[parkIndex - 1] -= currentParkTime;

            m_queueWaitingTime += currentParkTime;

            int y = topY - 1 - channelCount - parkIndex;
            AddVisualPoint(y, start);
            AddVisualPoint(y, end);
        }

        public void PushServedPoint(double completeTime)
        {
            m_clientsServed++;
            int y = 1;
            AddVisualPoint(y, completeTime);
            Breakline(y, completeTime);
        }

        public void PushBreakPoint(double breakTime)
        {
            m_clientsLost++;
            int y = 0;
            AddVisualPoint(y, breakTime);
            Breakline(y, breakTime);
        }

        void Breakline(double y, double x)
        {
            if (systemStopTime < x)
                m_systemStopTime = x;

            m_clientsCount++;
            OnLineFinished(y, x);
        }

        protected abstract void OnLineFinished(double y, double x);

        protected abstract void FinishVisualDiagram();

        public void CompleteDiagram()
        {
            if (m_diagramCompleted)
                throw new Exception("Диаграмма уже построена");

            m_diagramCompleted = true;
            CaluclateInverseLines();
            FinishVisualDiagram();
        }

        public void CaluclateInverseLines()
        {
            for (int i = 0; i < channelCount; i++)
            {
                var lines = m_channelLines[i];

                if (lines.Count == 0)
                {
                    m_inverseChannelLines[i] = new Line[] 
                    {
                        new Line(systemStartTime, systemStopTime)
                    };
                    continue;
                }

                List<Line> result = new List<Line>();

                result.Add(new Line(systemStartTime, lines[0].start));

                for (int k = 0; k < lines.Count - 1; k++)
                    result.Add(new Line(lines[k].end, lines[k + 1].start));

                result.Add
                    (
                    new Line(lines[lines.Count - 1].end, systemStopTime)
                    );

                m_inverseChannelLines[i] = result.ToArray();
            }
        }

        public bool diagramCompleted
        {
            get
            {
                return m_diagramCompleted;
            }
        }

        public int channelCount
        {
            get
            {
                return m_channelCount;
            }
        }

        public int queueCapacity
        {
            get
            {
                return m_queueCapacity;
            }
        }

        public int clientsCount
        {
            get
            {
                return m_clientsCount;
            }
        }

        public int clientsServed
        {
            get
            {
                return m_clientsServed;
            }
        }

        public int clientsLost
        {
            get
            {
                return m_clientsLost;
            }
        }

        public double systemStartTime
        {
            get
            {
                return m_systemStartTime;
            }
        }

        public double systemStopTime
        {
            get
            {
                return m_systemStopTime;
            }
        }

        public double systemWorkTime
        {
            get
            {
                return systemStopTime - systemStartTime;
            }
        }

        public double[] queueBusyTime
        {
            get
            {
                return m_queueBusyTime;
            }
        }

        public double queueWaitingTime
        {
            get
            {
                return m_queueWaitingTime;
            }
        }

        public double serviceTime
        {
            get
            {
                return m_serviceTime;
            }
        }

        public int queueClientCount
        {
            get
            {
                return m_queueClientCount;
            }
        }

        Line[] LineIntersect
            (
            IEnumerable<Line> lines1, 
            IEnumerable<Line> lines2
            )
        {
            List<Line> result = new List<Line>();

            foreach (Line a in lines1)
            {
                foreach (Line b in lines2)
                {
                    double start = Math.Max(a.start, b.start);
                    double end = Math.Min(a.end, b.end);

                    if (end - start <= 0)
                        continue;

                    result.Add(new Line(start, end));
                }
            }

            return result.ToArray();
        }

        public double GetChannelIntersectionLength(int count)
        {
            double intersectionLength = 0;

            foreach (var usingChannels in 
                Permuter.Permute(channelCount, count))
            {
                var intersection = new Line[]
                {
                    new Line(0, double.MaxValue)
                };

                for (int i = 0; i < count; i++)
                    intersection = LineIntersect
                        (
                        intersection, m_channelLines[usingChannels[i]]
                        );

                for (int i = 0; i < channelCount; i++)
                    if (!usingChannels.Contains(i))
                        intersection = LineIntersect
                            (
                            intersection, m_inverseChannelLines[i]
                            );

                foreach (var line in intersection)
                    intersectionLength += line.end - line.start;

            }

            return intersectionLength;
        }

        public int GetClientCountAtTime(double point, double step)
        {
            int count = 0;

            foreach (var line in m_queueLines)
            {
                var result = LineIntersect
                    (
                    line, 
                    new Line[] 
                    {
                        new Line(point - step / 2, point + step / 2)
                    }
                    );

                if (result.Length != 0)
                    count++;
            }

            foreach (var line in m_channelLines)
            {
                var result = LineIntersect
                    (
                    line, 
                    new Line[] 
                    {
                        new Line(point - step / 2, point + step / 2)
                    }
                    );

                if (result.Length != 0)
                    count++;
            }

            return count;
        }

        //IController
        public bool showPreviousLines
        {
            get
            {
                return m_showPreviousLines;
            }
            set
            {
                m_showPreviousLines = value;
                SetDiagramVisibility(m_currentVisibleIndex);
            }
        }

        protected abstract void UpdateVisualContent(int visibleLineIndex);

        protected void SetDiagramVisibility(int lineIndex)
        {
            m_currentVisibleIndex = lineIndex;
            UpdateVisualContent(currentVisibleIndex);

            if (OnVisualUpdated != null)
                OnVisualUpdated();
        }

        public void GoToStart()
        {
            SetDiagramVisibility(0);
        }

        public void GoToEnd()
        {
            SetDiagramVisibility(clientsCount - 1);
        }

        public void StepForward()
        {
            if (currentVisibleIndex == clientsCount - 1)
                return;

            SetDiagramVisibility(currentVisibleIndex + 1);
        }

        public void StepBack()
        {
            if (currentVisibleIndex == 0)
                return;

            SetDiagramVisibility(currentVisibleIndex - 1);
        }
    }
}
