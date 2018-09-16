﻿using System;
using System.Collections.Generic;

namespace QTS.Core.Graphics
{
    /// <summary>
    /// Представляет общие свойства и методы интерактивной диаграммы.
    /// </summary>
    public abstract class InteractiveDiagram : IGraph
    {
        int currentVisibleIndex = 0;
        event Action viewUpdated;
        bool showPreviousLines;

        /// <summary>
        /// Количество линий на диаграмме.
        /// </summary>
        public int LinesCount { get; private set; }

        protected InteractiveDiagram()
        {
            showPreviousLines = true;
        }

        //                      ,o.----.-----.oo.
        //             d8P=.:::.;;;::.`88b.
        //            /'.'. \:/ . `---:_;-|-
        //           ,-'.....v:..........:_.'-.
        //          /,-;.....|:..........`-;:\ \
        //         |' /'....:;::.........|\:.|'|
        //          '|| ..::/ `:::......|\::.| '
        //         |::\|.:/--. \|\|\-\.;\:::.`.
        //        .':::\\|'.-=\ `  \=-.,;:..:.|
        //        ||::::/\'``9;    'P/,"|)__ .|
        //        |'.:::(`| `- ;    --' //  \_'
        //       .'.. :::`|    /       .(     )
        //       ||..:|.::`    `__    ,(  ,  +
        //       |'..:`.|..`.  `='     |-;    `.
        //      .'..:..|.|..:`.    .'. \| `. /O/.
        //     .|..:...`..\:::' --  |.:...:.`.~  `.
        //    /'..::_.--'--/' :.    `.\;;;;::|\    `.
        //  -' |.:;'..\    |._      /    ,'--:_`.    `.
        //    /|./ |...`.   \    -.'   ,'...../- :.    `.
        //   | |.' |/....`-. \ _.'_..'.....,'   ..`:     `.
        //     \|  |'.....|_`.\'-' /..... /. . '   `:      `.
        //      '  |......-''/+|\ \....../.      .','.      .
        //     /  .'......\-'/,| \/.....| .     /.'|.\:     |
        //    ,   |........|//.' |......' .    '   | .`     '
        //    |   `-.__ . .|`||  |__ .-|  .        |==.__._/
        //    |  ` |:::::::| || |::::::'  ;        `::::|
        //    '   \ |::::::| `' |:::..|  .    _.-   |:::'
        //   /     \ |..:::|  .'|::..|  ___,-'`-.   |..|
        //   `-.    \/:....|.'  /...:|-'  /      `. | /
        //       \   `:;;;;;\_||:./;|    |         `-/
        //        )--==|O     `-`---\---'
        //       |.:. /:~`-=-.___()__;|
        //      .'.  /::/':::-------' .\
        //     .'.  /:::| :::::: |:::..:|
        //     |.  /::. /...::::.|:::...|
        //    .'  ;::. /........ |......:\
        //    |  |:.:.....:...........:.:`
        //   .' ,;.:..'..::....:.....:.:\:\
        //   |  |:.:.._..:.....:.........\`
        //  .' ,;..:\._......__:_......::.`\
        //  |  |:::.:/::......:........:....\
        //  |  |:.::||::......:.__,-........`
        //  | ,::..:||::......:. \::....:....\
        //  | |:.:..'|::........ ||:.....:... \
        // /  |::::|:|::..:...../||:..........`
        //|. ,|:...|.|::..:..... ||:.......... \
        //|| ,;::.:|.|::..:..... |:|:......:... \
        //`.\|1:../:.|::..:..... |:|:.......:... \
        // | |9:..\:.|::.:...... |.|:............
        //  \|9::.|:.|::.:...... |..|:.......:....`
        //   |7:.:|1.|::.:...... |..|:............ \
        //   |:::.|6.|::.:...... |:.|:........:.... \
        //   |1:..|5.|::........ |:..|:..............\
        //    0:::|m.|::........ |:..|:........:..... |
        //    \:::|i:|::........ |:..|::........... ,-'
        //     \26|n:|:::....... |::.:|:........: .'
        //          `-GST::......|:.:.|:.......::|
        //             |`-:_______|---|________,-'
        //             |. . .`. . . .|
        //             `   . .|. . . |
        //              \   . | . . .|
        //               |.  .|.     |
        //               |    \   .  /
        //               `. . |      |
        //                |   |     .|
        //                `.  | .    |
        //                 |__|      |
        //                 |==|     .'
        //                 `.~|==.__|
        //                  | |~~===|
        //                  | |     |
        //                   \|    .'
        //                    |    |
        //                    |    |\
        //                    |   .' |
        //                    |   |  `.
        //                   .'   | .'|
        //                   |   .'_,/:\
        //                   |   |<:::::`.
        //                _,-|   |':::;-)|
        //              ,+__/'   |;-=+_;-'
        //             /;;;||    |-'
        //             `---|:\   |
        //                 |\:\,--\
        //                  \`:,---`
        //                   |:|   |\
        //                   |\:`._')`
        //                    \:::./ |
        //                     \:;..\|
        //                      `._;='
        //Влом коментить...
        //стащил с инета Аску)

        void OnMouseDown(object lineKey) => SetVisibleLinesCount((int)lineKey);

        public void AddInteractiveAnnotation(double y, double x, string annotation, bool atTop, object annotationKey) => CreateInteractiveAnnotation(y, x, annotation, atTop, annotationKey, OnMouseDown);

        public void BeginLine()
        {
            LinesCount++;
            CreateLine();
        }

        public void BeginInteractiveLine(object lineKey)
        {
            BeginLine();
            MakeLineInteractive(lineKey, OnMouseDown);
        }

        public abstract string Title { get; set; }

        protected abstract void CreateLine();

        protected abstract void MakeLineInteractive(object lineKey, Action<object> onMouseDown);

        protected abstract void CreateInteractiveAnnotation(double y, double x, string annotation, bool atTop, object annotationKey, Action<object> onMouseDown);

        public abstract void AddPoint(double y, double x);

        public abstract void AddLineMetadata(string metadata);

        public abstract void AddAnnotation(double y, double x, string annotation, bool atTop);

        public abstract void UpdateView(int visibleLineIndex);

        public abstract void CompleteLine(bool randomColor = true);

        public abstract void CreateLineByPoints(IEnumerable<double> yValues, double startX);

        public abstract void ExportToBitmap(bool betterHieghts, string path);

        void SetVisibleLinesCount(int lineIndex)
        {
            currentVisibleIndex = lineIndex;
            UpdateView(currentVisibleIndex);

            viewUpdated?.Invoke();
        }

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
            SetVisibleLinesCount(LinesCount - 1);
        }

        public void StepForward()
        {
            if (currentVisibleIndex == LinesCount - 1)
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
