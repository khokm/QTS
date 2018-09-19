namespace QTS.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьКонфигурациюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьКонфигурациюКакToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.анализToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.построитьГрафикToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.анализДиаграммыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.синтезСМОToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.улучшениеГрафикаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.управлениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RightPanel = new System.Windows.Forms.Panel();
            this.preferFirstChannel_CheckBox = new System.Windows.Forms.CheckBox();
            this.clientLimit_CheckBox = new System.Windows.Forms.CheckBox();
            this.timeLimit_CheckBox = new System.Windows.Forms.CheckBox();
            this.clientLimit_Numeric = new System.Windows.Forms.NumericUpDown();
            this.timeLimit_Numeric = new System.Windows.Forms.NumericUpDown();
            this.minRnd_Numeric = new System.Windows.Forms.NumericUpDown();
            this.threadIntencity_Numeric = new System.Windows.Forms.NumericUpDown();
            this.parkPlace_Numeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.deleteChannelIntencity_Button = new System.Windows.Forms.Button();
            this.addChannelIntencity_Button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.channelIntencites = new System.Windows.Forms.ListBox();
            this.plot1 = new OxyPlot.WindowsForms.PlotView();
            this.PlotPanel = new System.Windows.Forms.Panel();
            this.statusText = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toStart_Button = new System.Windows.Forms.ToolStripButton();
            this.stepBack_Button = new System.Windows.Forms.ToolStripButton();
            this.stepForward_Button = new System.Windows.Forms.ToolStripButton();
            this.toEnd_Button = new System.Windows.Forms.ToolStripButton();
            this.ShowPreviousLines_ComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.ShowGraphs_ComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.описаниеФункцийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1.SuspendLayout();
            this.RightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientLimit_Numeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeLimit_Numeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRnd_Numeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadIntencity_Numeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parkPlace_Numeric)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.PlotPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.анализToolStripMenuItem,
            this.помощьToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1123, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.загрузитьКонфигурациюToolStripMenuItem,
            this.сохранитьКонфигурациюКакToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // загрузитьКонфигурациюToolStripMenuItem
            // 
            this.загрузитьКонфигурациюToolStripMenuItem.Name = "загрузитьКонфигурациюToolStripMenuItem";
            this.загрузитьКонфигурациюToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.загрузитьКонфигурациюToolStripMenuItem.Text = "Загрузить конфигурацию...";
            this.загрузитьКонфигурациюToolStripMenuItem.Click += new System.EventHandler(this.загрузитьКонфигурациюToolStripMenuItem_Click);
            // 
            // сохранитьКонфигурациюКакToolStripMenuItem
            // 
            this.сохранитьКонфигурациюКакToolStripMenuItem.Name = "сохранитьКонфигурациюКакToolStripMenuItem";
            this.сохранитьКонфигурациюКакToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.сохранитьКонфигурациюКакToolStripMenuItem.Text = "Сохранить конфигурацию в файл...";
            this.сохранитьКонфигурациюКакToolStripMenuItem.Click += new System.EventHandler(this.сохранитьКонфигурациюКакToolStripMenuItem_Click);
            // 
            // анализToolStripMenuItem
            // 
            this.анализToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.построитьГрафикToolStripMenuItem,
            this.анализДиаграммыToolStripMenuItem,
            this.синтезСМОToolStripMenuItem,
            this.улучшениеГрафикаToolStripMenuItem});
            this.анализToolStripMenuItem.Name = "анализToolStripMenuItem";
            this.анализToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.анализToolStripMenuItem.Text = "Действия";
            // 
            // построитьГрафикToolStripMenuItem
            // 
            this.построитьГрафикToolStripMenuItem.Name = "построитьГрафикToolStripMenuItem";
            this.построитьГрафикToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.построитьГрафикToolStripMenuItem.Text = "Построить временную диаграмму";
            this.построитьГрафикToolStripMenuItem.Click += new System.EventHandler(this.построитьГрафикToolStripMenuItem_Click);
            // 
            // анализДиаграммыToolStripMenuItem
            // 
            this.анализДиаграммыToolStripMenuItem.Name = "анализДиаграммыToolStripMenuItem";
            this.анализДиаграммыToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.анализДиаграммыToolStripMenuItem.Text = "Анализ диаграммы...";
            this.анализДиаграммыToolStripMenuItem.Click += new System.EventHandler(this.анализДиаграммыToolStripMenuItem_Click);
            // 
            // синтезСМОToolStripMenuItem
            // 
            this.синтезСМОToolStripMenuItem.Name = "синтезСМОToolStripMenuItem";
            this.синтезСМОToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.синтезСМОToolStripMenuItem.Text = "Синтез СМО...";
            this.синтезСМОToolStripMenuItem.Click += new System.EventHandler(this.синтезСМОToolStripMenuItem_Click);
            // 
            // улучшениеГрафикаToolStripMenuItem
            // 
            this.улучшениеГрафикаToolStripMenuItem.Name = "улучшениеГрафикаToolStripMenuItem";
            this.улучшениеГрафикаToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.улучшениеГрафикаToolStripMenuItem.Text = "Улучшение графика...";
            this.улучшениеГрафикаToolStripMenuItem.Click += new System.EventHandler(this.улучшениеГрафикаToolStripMenuItem_Click);
            // 
            // помощьToolStripMenuItem
            // 
            this.помощьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.управлениеToolStripMenuItem,
            this.описаниеФункцийToolStripMenuItem});
            this.помощьToolStripMenuItem.Name = "помощьToolStripMenuItem";
            this.помощьToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.помощьToolStripMenuItem.Text = "Помощь";
            // 
            // управлениеToolStripMenuItem
            // 
            this.управлениеToolStripMenuItem.Name = "управлениеToolStripMenuItem";
            this.управлениеToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.управлениеToolStripMenuItem.Text = "Помощь по управлению";
            this.управлениеToolStripMenuItem.Click += new System.EventHandler(this.управлениеToolStripMenuItem_Click);
            // 
            // RightPanel
            // 
            this.RightPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RightPanel.Controls.Add(this.preferFirstChannel_CheckBox);
            this.RightPanel.Controls.Add(this.clientLimit_CheckBox);
            this.RightPanel.Controls.Add(this.timeLimit_CheckBox);
            this.RightPanel.Controls.Add(this.clientLimit_Numeric);
            this.RightPanel.Controls.Add(this.timeLimit_Numeric);
            this.RightPanel.Controls.Add(this.minRnd_Numeric);
            this.RightPanel.Controls.Add(this.threadIntencity_Numeric);
            this.RightPanel.Controls.Add(this.parkPlace_Numeric);
            this.RightPanel.Controls.Add(this.label3);
            this.RightPanel.Controls.Add(this.label2);
            this.RightPanel.Controls.Add(this.label1);
            this.RightPanel.Controls.Add(this.deleteChannelIntencity_Button);
            this.RightPanel.Controls.Add(this.addChannelIntencity_Button);
            this.RightPanel.Controls.Add(this.groupBox1);
            this.RightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightPanel.Location = new System.Drawing.Point(882, 24);
            this.RightPanel.Name = "RightPanel";
            this.RightPanel.Padding = new System.Windows.Forms.Padding(0, 300, 0, 60);
            this.RightPanel.Size = new System.Drawing.Size(241, 606);
            this.RightPanel.TabIndex = 2;
            // 
            // preferFirstChannel_CheckBox
            // 
            this.preferFirstChannel_CheckBox.AutoSize = true;
            this.preferFirstChannel_CheckBox.Checked = true;
            this.preferFirstChannel_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.preferFirstChannel_CheckBox.Location = new System.Drawing.Point(3, 246);
            this.preferFirstChannel_CheckBox.Name = "preferFirstChannel_CheckBox";
            this.preferFirstChannel_CheckBox.Size = new System.Drawing.Size(171, 17);
            this.preferFirstChannel_CheckBox.TabIndex = 20;
            this.preferFirstChannel_CheckBox.Text = "Предпочитать первый канал";
            this.preferFirstChannel_CheckBox.UseVisualStyleBackColor = true;
            // 
            // clientLimit_CheckBox
            // 
            this.clientLimit_CheckBox.AutoSize = true;
            this.clientLimit_CheckBox.Location = new System.Drawing.Point(4, 206);
            this.clientLimit_CheckBox.Name = "clientLimit_CheckBox";
            this.clientLimit_CheckBox.Size = new System.Drawing.Size(157, 17);
            this.clientLimit_CheckBox.TabIndex = 19;
            this.clientLimit_CheckBox.Text = "Ограничение по заявкам:";
            this.clientLimit_CheckBox.UseVisualStyleBackColor = true;
            this.clientLimit_CheckBox.CheckedChanged += new System.EventHandler(this.clientLimit_CheckBox_CheckedChanged);
            // 
            // timeLimit_CheckBox
            // 
            this.timeLimit_CheckBox.AutoSize = true;
            this.timeLimit_CheckBox.Location = new System.Drawing.Point(4, 161);
            this.timeLimit_CheckBox.Name = "timeLimit_CheckBox";
            this.timeLimit_CheckBox.Size = new System.Drawing.Size(119, 30);
            this.timeLimit_CheckBox.TabIndex = 18;
            this.timeLimit_CheckBox.Text = "Ограничение по \r\nвремени (в часах):";
            this.timeLimit_CheckBox.UseVisualStyleBackColor = true;
            this.timeLimit_CheckBox.CheckedChanged += new System.EventHandler(this.timeLimit_CheckBox_CheckedChanged);
            // 
            // clientLimit_Numeric
            // 
            this.clientLimit_Numeric.Enabled = false;
            this.clientLimit_Numeric.Location = new System.Drawing.Point(164, 206);
            this.clientLimit_Numeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.clientLimit_Numeric.Name = "clientLimit_Numeric";
            this.clientLimit_Numeric.Size = new System.Drawing.Size(65, 20);
            this.clientLimit_Numeric.TabIndex = 17;
            // 
            // timeLimit_Numeric
            // 
            this.timeLimit_Numeric.DecimalPlaces = 3;
            this.timeLimit_Numeric.Enabled = false;
            this.timeLimit_Numeric.Location = new System.Drawing.Point(164, 167);
            this.timeLimit_Numeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.timeLimit_Numeric.Name = "timeLimit_Numeric";
            this.timeLimit_Numeric.Size = new System.Drawing.Size(65, 20);
            this.timeLimit_Numeric.TabIndex = 15;
            // 
            // minRnd_Numeric
            // 
            this.minRnd_Numeric.DecimalPlaces = 3;
            this.minRnd_Numeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.minRnd_Numeric.Location = new System.Drawing.Point(164, 118);
            this.minRnd_Numeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.minRnd_Numeric.Name = "minRnd_Numeric";
            this.minRnd_Numeric.Size = new System.Drawing.Size(65, 20);
            this.minRnd_Numeric.TabIndex = 13;
            this.minRnd_Numeric.ValueChanged += new System.EventHandler(this.minRnd_Numeric_ValueChanged);
            // 
            // threadIntencity_Numeric
            // 
            this.threadIntencity_Numeric.Location = new System.Drawing.Point(164, 20);
            this.threadIntencity_Numeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.threadIntencity_Numeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threadIntencity_Numeric.Name = "threadIntencity_Numeric";
            this.threadIntencity_Numeric.Size = new System.Drawing.Size(65, 20);
            this.threadIntencity_Numeric.TabIndex = 12;
            this.threadIntencity_Numeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // parkPlace_Numeric
            // 
            this.parkPlace_Numeric.Location = new System.Drawing.Point(164, 67);
            this.parkPlace_Numeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.parkPlace_Numeric.Name = "parkPlace_Numeric";
            this.parkPlace_Numeric.Size = new System.Drawing.Size(65, 20);
            this.parkPlace_Numeric.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "Минимальный интервал\r\nмежду заявками:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Интенсивность потока (шт/ч):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Количество стояночных\r\nмест:";
            // 
            // deleteChannelIntencity_Button
            // 
            this.deleteChannelIntencity_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteChannelIntencity_Button.Enabled = false;
            this.deleteChannelIntencity_Button.Location = new System.Drawing.Point(124, 552);
            this.deleteChannelIntencity_Button.Name = "deleteChannelIntencity_Button";
            this.deleteChannelIntencity_Button.Size = new System.Drawing.Size(105, 23);
            this.deleteChannelIntencity_Button.TabIndex = 4;
            this.deleteChannelIntencity_Button.Text = "Удалить";
            this.deleteChannelIntencity_Button.UseVisualStyleBackColor = true;
            this.deleteChannelIntencity_Button.Click += new System.EventHandler(this.deleteChannelIntencity_Button_Click);
            // 
            // addChannelIntencity_Button
            // 
            this.addChannelIntencity_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addChannelIntencity_Button.Location = new System.Drawing.Point(15, 552);
            this.addChannelIntencity_Button.Name = "addChannelIntencity_Button";
            this.addChannelIntencity_Button.Size = new System.Drawing.Size(103, 23);
            this.addChannelIntencity_Button.TabIndex = 3;
            this.addChannelIntencity_Button.Text = "Добавить";
            this.addChannelIntencity_Button.UseVisualStyleBackColor = true;
            this.addChannelIntencity_Button.Click += new System.EventHandler(this.addChannelIntencity_Button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.channelIntencites);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 300);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.groupBox1.Size = new System.Drawing.Size(241, 246);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Пропускные способности каналов (шт/ч)";
            // 
            // channelIntencites
            // 
            this.channelIntencites.Dock = System.Windows.Forms.DockStyle.Fill;
            this.channelIntencites.FormattingEnabled = true;
            this.channelIntencites.Location = new System.Drawing.Point(3, 13);
            this.channelIntencites.Name = "channelIntencites";
            this.channelIntencites.Size = new System.Drawing.Size(235, 230);
            this.channelIntencites.TabIndex = 3;
            this.channelIntencites.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.channelIntencites_MouseDoubleClick);
            // 
            // plot1
            // 
            this.plot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plot1.Location = new System.Drawing.Point(0, 20);
            this.plot1.Name = "plot1";
            this.plot1.PanCursor = System.Windows.Forms.Cursors.NoMove2D;
            this.plot1.Size = new System.Drawing.Size(888, 586);
            this.plot1.TabIndex = 0;
            this.plot1.Text = "plot1";
            this.plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            this.plot1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.plot1_PreviewKeyDown);
            // 
            // PlotPanel
            // 
            this.PlotPanel.Controls.Add(this.statusText);
            this.PlotPanel.Controls.Add(this.plot1);
            this.PlotPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlotPanel.Location = new System.Drawing.Point(0, 24);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Padding = new System.Windows.Forms.Padding(0, 20, 235, 0);
            this.PlotPanel.Size = new System.Drawing.Size(1123, 606);
            this.PlotPanel.TabIndex = 1;
            // 
            // statusText
            // 
            this.statusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.statusText.Location = new System.Drawing.Point(0, 20);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(888, 586);
            this.statusText.TabIndex = 4;
            this.statusText.Text = "statusText";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statusText.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toStart_Button,
            this.stepBack_Button,
            this.stepForward_Button,
            this.toEnd_Button,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.ShowPreviousLines_ComboBox,
            this.toolStripLabel2,
            this.ShowGraphs_ComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(882, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toStart_Button
            // 
            this.toStart_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toStart_Button.Image = ((System.Drawing.Image)(resources.GetObject("toStart_Button.Image")));
            this.toStart_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toStart_Button.Name = "toStart_Button";
            this.toStart_Button.Size = new System.Drawing.Size(56, 22);
            this.toStart_Button.Text = "В начало";
            this.toStart_Button.Click += new System.EventHandler(this.toStart_Button_Click);
            // 
            // stepBack_Button
            // 
            this.stepBack_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stepBack_Button.Image = ((System.Drawing.Image)(resources.GetObject("stepBack_Button.Image")));
            this.stepBack_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepBack_Button.Name = "stepBack_Button";
            this.stepBack_Button.Size = new System.Drawing.Size(65, 22);
            this.stepBack_Button.Text = "Шаг назад";
            this.stepBack_Button.Click += new System.EventHandler(this.stepBack_Button_Click);
            // 
            // stepForward_Button
            // 
            this.stepForward_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stepForward_Button.Image = ((System.Drawing.Image)(resources.GetObject("stepForward_Button.Image")));
            this.stepForward_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepForward_Button.Name = "stepForward_Button";
            this.stepForward_Button.Size = new System.Drawing.Size(72, 22);
            this.stepForward_Button.Text = "Шаг вперед";
            this.stepForward_Button.Click += new System.EventHandler(this.stepForward_Button_Click);
            // 
            // toEnd_Button
            // 
            this.toEnd_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toEnd_Button.Image = ((System.Drawing.Image)(resources.GetObject("toEnd_Button.Image")));
            this.toEnd_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toEnd_Button.Name = "toEnd_Button";
            this.toEnd_Button.Size = new System.Drawing.Size(50, 22);
            this.toEnd_Button.Text = "В конец";
            this.toEnd_Button.Click += new System.EventHandler(this.toEnd_Button_Click);
            // 
            // ShowPreviousLines_ComboBox
            // 
            this.ShowPreviousLines_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ShowPreviousLines_ComboBox.DropDownWidth = 75;
            this.ShowPreviousLines_ComboBox.Items.AddRange(new object[] {
            "Все",
            "Одну"});
            this.ShowPreviousLines_ComboBox.MergeIndex = 0;
            this.ShowPreviousLines_ComboBox.Name = "ShowPreviousLines_ComboBox";
            this.ShowPreviousLines_ComboBox.Size = new System.Drawing.Size(75, 25);
            this.ShowPreviousLines_ComboBox.SelectedIndexChanged += new System.EventHandler(this.ShowPreviousLines_ComboBox_SelectedIndexChanged);
            // 
            // ShowGraphs_ComboBox
            // 
            this.ShowGraphs_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ShowGraphs_ComboBox.Items.AddRange(new object[] {
            "Семейство графиков",
            "Сумму графиков"});
            this.ShowGraphs_ComboBox.MergeIndex = 0;
            this.ShowGraphs_ComboBox.Name = "ShowGraphs_ComboBox";
            this.ShowGraphs_ComboBox.Size = new System.Drawing.Size(130, 25);
            this.ShowGraphs_ComboBox.SelectedIndexChanged += new System.EventHandler(this.ShowGraphs_ComboBox_SelectedIndexChanged);
            // 
            // описаниеФункцийToolStripMenuItem
            // 
            this.описаниеФункцийToolStripMenuItem.Name = "описаниеФункцийToolStripMenuItem";
            this.описаниеФункцийToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.описаниеФункцийToolStripMenuItem.Text = "Описание функций программы";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(125, 22);
            this.toolStripLabel1.Text = "Показывать линии (Q):";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(94, 22);
            this.toolStripLabel2.Text = "Показывать (W):";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 630);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.RightPanel);
            this.Controls.Add(this.PlotPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.Text = "Системы массового обслуживания";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.RightPanel.ResumeLayout(false);
            this.RightPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientLimit_Numeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeLimit_Numeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRnd_Numeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadIntencity_Numeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parkPlace_Numeric)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.PlotPanel.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel RightPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addChannelIntencity_Button;
        private System.Windows.Forms.Button deleteChannelIntencity_Button;
        private System.Windows.Forms.ListBox channelIntencites;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem построитьГрафикToolStripMenuItem;
        private OxyPlot.WindowsForms.PlotView plot1;
        private System.Windows.Forms.Panel PlotPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toStart_Button;
        private System.Windows.Forms.ToolStripButton stepForward_Button;
        private System.Windows.Forms.ToolStripButton stepBack_Button;
        private System.Windows.Forms.NumericUpDown threadIntencity_Numeric;
        private System.Windows.Forms.NumericUpDown parkPlace_Numeric;
        private System.Windows.Forms.NumericUpDown minRnd_Numeric;
        private System.Windows.Forms.NumericUpDown timeLimit_Numeric;
        private System.Windows.Forms.NumericUpDown clientLimit_Numeric;
        private System.Windows.Forms.CheckBox clientLimit_CheckBox;
        private System.Windows.Forms.CheckBox timeLimit_CheckBox;
        private System.Windows.Forms.ToolStripMenuItem анализToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem анализДиаграммыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem синтезСМОToolStripMenuItem;
        private System.Windows.Forms.CheckBox preferFirstChannel_CheckBox;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem управлениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem загрузитьКонфигурациюToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьКонфигурациюКакToolStripMenuItem;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.ToolStripMenuItem улучшениеГрафикаToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toEnd_Button;
        private System.Windows.Forms.ToolStripComboBox ShowPreviousLines_ComboBox;
        private System.Windows.Forms.ToolStripComboBox ShowGraphs_ComboBox;
        private System.Windows.Forms.ToolStripMenuItem описаниеФункцийToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    }
}