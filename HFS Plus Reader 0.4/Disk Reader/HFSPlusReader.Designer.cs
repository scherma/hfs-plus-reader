namespace Disk_Reader
{
    partial class HFSPlusReader
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HFSPlusReader));
            this.directoryTree = new System.Windows.Forms.TreeView();
            this.mainMenuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hashImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hashFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hashAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hashSelectedFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ItemsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesListRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hashFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.gridScroll = new System.Windows.Forms.VScrollBar();
            this.sectorGrid = new System.Windows.Forms.PictureBox();
            this.MapRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.showTotalBlocks = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.showCurrentBlock = new System.Windows.Forms.Label();
            this.goToBlock = new System.Windows.Forms.Button();
            this.blockNumBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.prevBlock = new System.Windows.Forms.Button();
            this.nextBlock = new System.Windows.Forms.Button();
            this.hexText = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SectorIcons = new System.Windows.Forms.ImageList(this.components);
            this.mainMenuBar.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.filesListRightClick.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sectorGrid)).BeginInit();
            this.MapRightClick.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // directoryTree
            // 
            this.directoryTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryTree.Location = new System.Drawing.Point(0, 0);
            this.directoryTree.Name = "directoryTree";
            this.directoryTree.Size = new System.Drawing.Size(302, 433);
            this.directoryTree.TabIndex = 2;
            this.directoryTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTree_BeforeExpand);
            this.directoryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directoryTree_AfterSelect);
            // 
            // mainMenuBar
            // 
            this.mainMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.analyseToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuBar.Location = new System.Drawing.Point(0, 0);
            this.mainMenuBar.Name = "mainMenuBar";
            this.mainMenuBar.Size = new System.Drawing.Size(1284, 24);
            this.mainMenuBar.TabIndex = 4;
            this.mainMenuBar.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openToolStripMenuItem.Text = "Open Disk Image";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // analyseToolStripMenuItem
            // 
            this.analyseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hashImageToolStripMenuItem,
            this.hashFilesToolStripMenuItem});
            this.analyseToolStripMenuItem.Name = "analyseToolStripMenuItem";
            this.analyseToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.analyseToolStripMenuItem.Text = "Analyse";
            // 
            // hashImageToolStripMenuItem
            // 
            this.hashImageToolStripMenuItem.Enabled = false;
            this.hashImageToolStripMenuItem.Name = "hashImageToolStripMenuItem";
            this.hashImageToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hashImageToolStripMenuItem.Text = "Hash image";
            this.hashImageToolStripMenuItem.Click += new System.EventHandler(this.hashImageToolStripMenuItem_Click);
            // 
            // hashFilesToolStripMenuItem
            // 
            this.hashFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hashAllFilesToolStripMenuItem,
            this.hashSelectedFilesToolStripMenuItem});
            this.hashFilesToolStripMenuItem.Name = "hashFilesToolStripMenuItem";
            this.hashFilesToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hashFilesToolStripMenuItem.Text = "Hash files";
            // 
            // hashAllFilesToolStripMenuItem
            // 
            this.hashAllFilesToolStripMenuItem.Enabled = false;
            this.hashAllFilesToolStripMenuItem.Name = "hashAllFilesToolStripMenuItem";
            this.hashAllFilesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.hashAllFilesToolStripMenuItem.Text = "Hash all files";
            this.hashAllFilesToolStripMenuItem.Click += new System.EventHandler(this.hashAllFilesToolStripMenuItem_Click);
            // 
            // hashSelectedFilesToolStripMenuItem
            // 
            this.hashSelectedFilesToolStripMenuItem.Enabled = false;
            this.hashSelectedFilesToolStripMenuItem.Name = "hashSelectedFilesToolStripMenuItem";
            this.hashSelectedFilesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.hashSelectedFilesToolStripMenuItem.Text = "Hash selected files";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(948, 430);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ItemsList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(940, 404);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Contents List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ItemsList
            // 
            this.ItemsList.AllowColumnReorder = true;
            this.ItemsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader20});
            this.ItemsList.ContextMenuStrip = this.filesListRightClick;
            this.ItemsList.FullRowSelect = true;
            this.ItemsList.Location = new System.Drawing.Point(0, 0);
            this.ItemsList.Name = "ItemsList";
            this.ItemsList.Size = new System.Drawing.Size(940, 404);
            this.ItemsList.TabIndex = 4;
            this.ItemsList.UseCompatibleStateImageBehavior = false;
            this.ItemsList.View = System.Windows.Forms.View.Details;
            this.ItemsList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.onRowClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Catalog Node ID";
            this.columnHeader2.Width = 95;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Creation Date";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Last Modified";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Attributes Last Modified";
            this.columnHeader5.Width = 130;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Last Accessed";
            this.columnHeader6.Width = 130;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Last Backed Up";
            this.columnHeader7.Width = 130;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Permissions";
            this.columnHeader8.Width = 105;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Data Fork Size (Bytes)";
            this.columnHeader9.Width = 135;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Resource Fork Size (Bytes)";
            this.columnHeader10.Width = 135;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Data Fork Start Sector";
            this.columnHeader11.Width = 120;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Resource Fork Start Sector";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Data Fragments (Count)";
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Resource Fragments (count)";
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Data Fork MD5";
            this.columnHeader15.Width = 220;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Data Fork SHA1";
            this.columnHeader16.Width = 220;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Resource Fork MD5";
            this.columnHeader17.Width = 220;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Resource Fork SHA1";
            this.columnHeader18.Width = 220;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Is Deleted";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "Full Path";
            this.columnHeader20.Width = 500;
            // 
            // filesListRightClick
            // 
            this.filesListRightClick.Enabled = false;
            this.filesListRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hashFileToolStripMenuItem,
            this.exportFileToolStripMenuItem});
            this.filesListRightClick.Name = "filesListRightClick";
            this.filesListRightClick.Size = new System.Drawing.Size(127, 48);
            // 
            // hashFileToolStripMenuItem
            // 
            this.hashFileToolStripMenuItem.Enabled = false;
            this.hashFileToolStripMenuItem.Name = "hashFileToolStripMenuItem";
            this.hashFileToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.hashFileToolStripMenuItem.Text = "Hash file";
            this.hashFileToolStripMenuItem.Click += new System.EventHandler(this.hashFileToolStripMenuItem_Click);
            // 
            // exportFileToolStripMenuItem
            // 
            this.exportFileToolStripMenuItem.Enabled = false;
            this.exportFileToolStripMenuItem.Name = "exportFileToolStripMenuItem";
            this.exportFileToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.exportFileToolStripMenuItem.Text = "Export file";
            this.exportFileToolStripMenuItem.Click += new System.EventHandler(this.exportFileToolStripMenuItem_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.gridScroll);
            this.tabPage2.Controls.Add(this.sectorGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(940, 404);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Map";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9F);
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(145, 404);
            this.textBox1.TabIndex = 2;
            // 
            // gridScroll
            // 
            this.gridScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridScroll.Location = new System.Drawing.Point(923, 0);
            this.gridScroll.Name = "gridScroll";
            this.gridScroll.Size = new System.Drawing.Size(17, 404);
            this.gridScroll.TabIndex = 1;
            this.gridScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.gridScroll_Scroll);
            // 
            // sectorGrid
            // 
            this.sectorGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sectorGrid.ContextMenuStrip = this.MapRightClick;
            this.sectorGrid.ErrorImage = null;
            this.sectorGrid.Location = new System.Drawing.Point(151, 3);
            this.sectorGrid.Name = "sectorGrid";
            this.sectorGrid.Size = new System.Drawing.Size(769, 398);
            this.sectorGrid.TabIndex = 0;
            this.sectorGrid.TabStop = false;
            this.sectorGrid.SizeChanged += new System.EventHandler(this.sectorGrid_SizeChanged);
            this.sectorGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.sectorGrid_Paint);
            this.sectorGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.sectorGrid_MouseClick);
            this.sectorGrid.Resize += new System.EventHandler(this.sectorGrid_Resize);
            // 
            // MapRightClick
            // 
            this.MapRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToToolStripMenuItem});
            this.MapRightClick.Name = "MapRightClick";
            this.MapRightClick.Size = new System.Drawing.Size(107, 26);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.goToToolStripMenuItem.Text = "Go To";
            this.goToToolStripMenuItem.Click += new System.EventHandler(this.goToToolStripMenuItem_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(0, 2);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1260, 301);
            this.tabControl2.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.propertyGrid1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1252, 275);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Information";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(6, 6);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(1243, 263);
            this.propertyGrid1.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.comboBox1);
            this.tabPage4.Controls.Add(this.showTotalBlocks);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.showCurrentBlock);
            this.tabPage4.Controls.Add(this.goToBlock);
            this.tabPage4.Controls.Add(this.blockNumBox);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.prevBlock);
            this.tabPage4.Controls.Add(this.nextBlock);
            this.tabPage4.Controls.Add(this.hexText);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1252, 275);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Hex";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(424, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // showTotalBlocks
            // 
            this.showTotalBlocks.AutoSize = true;
            this.showTotalBlocks.Location = new System.Drawing.Point(149, 9);
            this.showTotalBlocks.MinimumSize = new System.Drawing.Size(45, 13);
            this.showTotalBlocks.Name = "showTotalBlocks";
            this.showTotalBlocks.Size = new System.Drawing.Size(45, 13);
            this.showTotalBlocks.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(131, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "of";
            // 
            // showCurrentBlock
            // 
            this.showCurrentBlock.AutoSize = true;
            this.showCurrentBlock.Location = new System.Drawing.Point(86, 10);
            this.showCurrentBlock.MinimumSize = new System.Drawing.Size(40, 13);
            this.showCurrentBlock.Name = "showCurrentBlock";
            this.showCurrentBlock.Size = new System.Drawing.Size(40, 13);
            this.showCurrentBlock.TabIndex = 6;
            // 
            // goToBlock
            // 
            this.goToBlock.Enabled = false;
            this.goToBlock.Location = new System.Drawing.Point(327, 6);
            this.goToBlock.Name = "goToBlock";
            this.goToBlock.Size = new System.Drawing.Size(41, 20);
            this.goToBlock.TabIndex = 5;
            this.goToBlock.Text = "Go to";
            this.goToBlock.UseVisualStyleBackColor = true;
            this.goToBlock.Click += new System.EventHandler(this.goToBlock_Click);
            // 
            // blockNumBox
            // 
            this.blockNumBox.Enabled = false;
            this.blockNumBox.Location = new System.Drawing.Point(271, 6);
            this.blockNumBox.Name = "blockNumBox";
            this.blockNumBox.Size = new System.Drawing.Size(50, 20);
            this.blockNumBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Viewing block";
            // 
            // prevBlock
            // 
            this.prevBlock.Enabled = false;
            this.prevBlock.Location = new System.Drawing.Point(211, 6);
            this.prevBlock.Name = "prevBlock";
            this.prevBlock.Size = new System.Drawing.Size(24, 20);
            this.prevBlock.TabIndex = 2;
            this.prevBlock.Text = "-";
            this.prevBlock.UseVisualStyleBackColor = true;
            this.prevBlock.Click += new System.EventHandler(this.prevBlock_Click);
            // 
            // nextBlock
            // 
            this.nextBlock.Enabled = false;
            this.nextBlock.Location = new System.Drawing.Point(241, 6);
            this.nextBlock.Name = "nextBlock";
            this.nextBlock.Size = new System.Drawing.Size(24, 20);
            this.nextBlock.TabIndex = 1;
            this.nextBlock.Text = "+";
            this.nextBlock.UseVisualStyleBackColor = true;
            this.nextBlock.Click += new System.EventHandler(this.nextBlock_Click);
            // 
            // hexText
            // 
            this.hexText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.hexText.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexText.Location = new System.Drawing.Point(0, 32);
            this.hexText.MinimumSize = new System.Drawing.Size(1100, 100);
            this.hexText.Multiline = true;
            this.hexText.Name = "hexText";
            this.hexText.ReadOnly = true;
            this.hexText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.hexText.Size = new System.Drawing.Size(1252, 194);
            this.hexText.TabIndex = 0;
            this.hexText.WordWrap = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Size = new System.Drawing.Size(1260, 743);
            this.splitContainer1.SplitterDistance = 436;
            this.splitContainer1.TabIndex = 7;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.directoryTree);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(1260, 436);
            this.splitContainer2.SplitterDistance = 305;
            this.splitContainer2.TabIndex = 0;
            // 
            // SectorIcons
            // 
            this.SectorIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SectorIcons.ImageStream")));
            this.SectorIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.SectorIcons.Images.SetKeyName(0, "gpt.bmp");
            this.SectorIcons.Images.SetKeyName(1, "mbr.bmp");
            this.SectorIcons.Images.SetKeyName(2, "partition_slack.bmp");
            this.SectorIcons.Images.SetKeyName(3, "volume_file.bmp");
            this.SectorIcons.Images.SetKeyName(4, "volume_header.bmp");
            this.SectorIcons.Images.SetKeyName(5, "volume_unallocated.bmp");
            // 
            // HFSPlusReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 782);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainMenuBar);
            this.MainMenuStrip = this.mainMenuBar;
            this.Name = "HFSPlusReader";
            this.Text = "HFS+ Reader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.mainMenuBar.ResumeLayout(false);
            this.mainMenuBar.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.filesListRightClick.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sectorGrid)).EndInit();
            this.MapRightClick.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView directoryTree;
        private System.Windows.Forms.MenuStrip mainMenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TextBox hexText;
        private System.Windows.Forms.Button nextBlock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button prevBlock;
        private System.Windows.Forms.Button goToBlock;
        private System.Windows.Forms.TextBox blockNumBox;
        private System.Windows.Forms.Label showTotalBlocks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label showCurrentBlock;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hashImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hashFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hashAllFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hashSelectedFilesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip filesListRightClick;
        private System.Windows.Forms.ToolStripMenuItem hashFileToolStripMenuItem;
        private System.Windows.Forms.ListView ItemsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ToolStripMenuItem exportFileToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ImageList SectorIcons;
        private System.Windows.Forms.PictureBox sectorGrid;
        private System.Windows.Forms.VScrollBar gridScroll;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ContextMenuStrip MapRightClick;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
    }
}

