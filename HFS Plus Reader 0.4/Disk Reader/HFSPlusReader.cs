using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Disk_Reader
{
    public partial class HFSPlusReader : Form
    {
        displayComponents dc = new displayComponents();
        bool _resizing = false;
        long gridStartRow = 0;
        MapGoTo _mapGoToForm;

        public HFSPlusReader()
        {
            InitializeComponent();
            hexText.Text = dc.contentDisplay;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectfile = new OpenFileDialog();
            selectfile.Filter = "DD Files|*.001|Apple DMG Files|*.dmg";
            selectfile.Title = "Select Image File";

            if (selectfile.ShowDialog() == DialogResult.OK)
            {
                dc.imagepath = selectfile.FileName;

                try
                {
                    dc.getImageContents(dc.imagepath);
                    directoryTree.Nodes.Clear();
                    directoryTree.Nodes.Add(dc.displayTree);

                    filesListRightClick.Enabled = true;
                    hashImageToolStripMenuItem.Enabled = true;
                    exportFileToolStripMenuItem.Enabled = true;

                    tabControl1.Enabled = true;
                    dc.setScrollSize(ref gridScroll, ref sectorGrid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(dc.help_about);
        }
        private void hashImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataOperations.hashValues hv = dc.addImageHash();
            string[] hashStrings = dataOperations.buildHashStrings(hv);

            absImageStream.imageProperties tag = (absImageStream.imageProperties)directoryTree.Nodes[0].Tag;

            tag.hashMD5 = hashStrings[0];
            tag.hashSHA1 = hashStrings[1];
            directoryTree.Nodes[0].Tag = tag;
            propertyGrid1.SelectedObject = tag;
        }
        private void onRowClick(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            comboBox1.Visible = false;
            if (e.Item.Tag is HFSPlusCatalogFolder)
            {
                HFSPlusCatalogFolder details = (HFSPlusCatalogFolder)e.Item.Tag;
                propertyGrid1.SelectedObject = details;
                resetHex();
            }
            else if (e.Item.Tag is HFSPlusCatalogFile)
            {
                resetHex();
                HFSPlusCatalogFile details = (HFSPlusCatalogFile)e.Item.Tag;
                propertyGrid1.SelectedObject = details;
                if (details.dataFork.forkDataValues.logicalSize > 0 || details.resourceFork.forkDataValues.logicalSize > 0)
                    comboBox1.Visible = true;
                if (details.dataFork.forkDataValues.logicalSize > 0)
                {
                    dc.showForkData(details, 0, forkStream.forkType.data);
                    dc.selectedFile = details;

                    hexText.Text = dc.contentDisplay;
                    showTotalBlocks.Text = details.dataFork.forkDataValues.totalBlocks.ToString();

                    if (details.dataFork.forkDataValues.totalBlocks > 1)
                    {
                        nextBlock.Enabled = true;
                        goToBlock.Enabled = true;
                        blockNumBox.Enabled = true;
                    }

                    dc.fileDataBlock = 1;
                    showCurrentBlock.Text = dc.fileDataBlock.ToString();
                    comboBox1.SelectedItem = dc.forkview[0];
                }
                else if (details.resourceFork.forkDataValues.logicalSize > 0)
                {
                    dc.showForkData(details, 0, forkStream.forkType.resource);
                    dc.selectedFile = details;

                    hexText.Text = dc.contentDisplay;
                    showTotalBlocks.Text = details.resourceFork.forkDataValues.totalBlocks.ToString();

                    if (details.resourceFork.forkDataValues.totalBlocks > 1)
                    {
                        nextBlock.Enabled = true;
                        goToBlock.Enabled = true;
                        blockNumBox.Enabled = true;
                    }

                    dc.fileDataBlock = 1;
                    showCurrentBlock.Text = dc.fileDataBlock.ToString();
                    comboBox1.SelectedItem = dc.forkview[1];
                }
                else
                {
                    hexText.Text = "";
                    showCurrentBlock.Text = "";
                }
                comboBox1.DataSource = dc.forkview;
            }
            else if (e.Item.Tag is HFSPlus.volumeHeader)
            {
                HFSPlus.volumeHeader details = (HFSPlus.volumeHeader)e.Item.Tag;
                propertyGrid1.SelectedObject = details;
                resetHex();
            }
            else if (e.Item.Tag is attributesLeafNode.HFSPlusAttrInlineData)
            {
                resetHex();

                attributesLeafNode.HFSPlusAttrInlineData attrDetails = (attributesLeafNode.HFSPlusAttrInlineData)e.Item.Tag;
                propertyGrid1.SelectedObject = attrDetails;

                if (attrDetails.otherData.Length > 0)
                {
                    dc.showInlineAttrData((attributesLeafNode.HFSPlusAttrInlineData)e.Item.Tag);
                    hexText.Text = dc.contentDisplay;
                }
            }
        }
        private void nextBlock_Click(object sender, EventArgs e)
        {
            dc.fileDataBlock++;
            showCurrentBlock.Text = dc.fileDataBlock.ToString();

            forkStream.forkType type = new forkStream.forkType();
            switch ((string)comboBox1.SelectedItem)
            {
                case "Resource fork":
                    type = forkStream.forkType.resource;
                    
                    dc.showForkData(dc.selectedFile, dc.fileDataBlock-1, forkStream.forkType.resource);
                    prevBlock.Enabled = true;
                    if (dc.selectedFile.resourceFork.forkDataValues.totalBlocks == dc.fileDataBlock)
                    {
                        nextBlock.Enabled = false;
                    }
                    break;
                default:
                    type = forkStream.forkType.data;
                                        
                    dc.showForkData(dc.selectedFile, dc.fileDataBlock-1, forkStream.forkType.data);
                    prevBlock.Enabled = true;
                    if (dc.selectedFile.dataFork.forkDataValues.totalBlocks == dc.fileDataBlock)
                    {
                        nextBlock.Enabled = false;
                    }
                    break;
            }
            hexText.Text = dc.contentDisplay;
        }
        private void prevBlock_Click(object sender, EventArgs e)
        {
            dc.fileDataBlock--;
            showCurrentBlock.Text = dc.fileDataBlock.ToString();
            forkStream.forkType type = new forkStream.forkType();
            switch ((string)comboBox1.SelectedItem)
            {
                case "Resource fork":
                    type = forkStream.forkType.resource;

                    dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.resource);
                    if (dc.selectedFile.resourceFork.forkDataValues.totalBlocks == dc.fileDataBlock)
                    {
                        nextBlock.Enabled = false;
                    }
                    break;
                default:
                    type = forkStream.forkType.data;

                    dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.data);
                    break;
            }
            nextBlock.Enabled = true;
            if (dc.fileDataBlock == 1)
            {
                prevBlock.Enabled = false;
            }
            hexText.Text = dc.contentDisplay;
        }
        private void goToBlock_Click(object sender, EventArgs e)
        {
            uint fileBlock = 1;
            try
            {
                fileBlock = Convert.ToUInt32(blockNumBox.Text);
            }
            catch (FormatException f)
            {
                MessageBox.Show("Input string is not a sequence of digits.\r\n" + f.Message);
            }
            catch (OverflowException f)
            {
                MessageBox.Show("Input must be greater than 0 and less than 2^32.\r\n" + f.Message);
            }
            finally
            {
                forkStream.forkType type = new forkStream.forkType();
                switch (comboBox1.SelectedItem.ToString())
                {
                    case "Resource fork": 
                        if (fileBlock > dc.selectedFile.resourceFork.forkDataValues.totalBlocks || fileBlock < 1)
                        {
                            MessageBox.Show("This block does not exist within the file");
                        }
                        else
                        {
                            dc.fileDataBlock = fileBlock;
                            showCurrentBlock.Text = dc.fileDataBlock.ToString();
                            showTotalBlocks.Text = dc.selectedFile.resourceFork.forkDataValues.totalBlocks.ToString();
                            dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.resource);

                            hexText.Text = dc.contentDisplay;

                            if (dc.fileDataBlock < dc.selectedFile.resourceFork.forkDataValues.totalBlocks)
                            {
                                nextBlock.Enabled = true;
                            }
                            else
                            {
                                nextBlock.Enabled = false;
                            }

                            if (dc.fileDataBlock > 1)
                            {
                                prevBlock.Enabled = true;
                            }
                            else
                            {
                                prevBlock.Enabled = false;
                            }

                            blockNumBox.Enabled = true;
                            goToBlock.Enabled = true;
                        }
                        break;
                    default:
                        if (fileBlock > dc.selectedFile.dataFork.forkDataValues.totalBlocks || fileBlock < 1)
                        {
                            MessageBox.Show("This block does not exist within the file");
                        }
                        else
                        {
                            dc.fileDataBlock = fileBlock;
                            showCurrentBlock.Text = dc.fileDataBlock.ToString();
                            showTotalBlocks.Text = dc.selectedFile.dataFork.forkDataValues.totalBlocks.ToString();
                            dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.data);

                            hexText.Text = dc.contentDisplay;

                            if (dc.fileDataBlock < dc.selectedFile.dataFork.forkDataValues.totalBlocks)
                            {
                                nextBlock.Enabled = true;
                            }
                            else
                            {
                                nextBlock.Enabled = false;
                            }

                            if (dc.fileDataBlock > 1)
                            {
                                prevBlock.Enabled = true;
                            }
                            else
                            {
                                prevBlock.Enabled = false;
                            }

                            blockNumBox.Enabled = true;
                            goToBlock.Enabled = true;
                        }
                        break;
                }
            }
        }
        private void resetHex()
        {
            nextBlock.Enabled = false;
            goToBlock.Enabled = false;
            blockNumBox.Enabled = false;
            showTotalBlocks.Text = "";
            showCurrentBlock.Text = "";
            blockNumBox.Text = "";
            hexText.Text = "";
        }
        private void hashAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Disabled in this version. Please use the right-click menu to hash files individually.");
            //DialogResult dr = MessageBox.Show("If this is a large image, this process could take a VERY long time.\r\n"
            //                               + "Do you wish to continue?", "Continue", MessageBoxButtons.YesNo);
            //if (dr == DialogResult.Yes)
            //{
            //    dc.hashAll();
            //}
        }
        private void hashFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ItemsList.SelectedItems[0].Tag is HFSPlusCatalogFile)
            {
                dataOperations.hashValues[] hashes = dc.hashFile((HFSPlusCatalogFile)ItemsList.SelectedItems[0].Tag);

                string[] data = dataOperations.buildHashStrings(hashes[0]);
                string[] resource = dataOperations.buildHashStrings(hashes[1]);

                string displayHashes = "Data fork hashes:\r\n" +
                                        "MD5  :: " + data[0] + "\r\n" +
                                        "SHA1 :: " + data[1] + "\r\n\r\n" +
                                        "Resource fork hashes:\r\n" +
                                        "MD5  :: " + resource[0] + "\r\n" +
                                        "SHA1 :: " + resource[1] + "\r\n";

                MessageBox.Show(displayHashes);
            }
        }
        private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ItemsList.SelectedItems[0].Tag is HFSPlusCatalogFile)
            {
                HFSPlusCatalogFile theFile = (HFSPlusCatalogFile)ItemsList.SelectedItems[0].Tag;

                SaveFileDialog sfd = new SaveFileDialog();

                sfd.FileName = ItemsList.SelectedItems[0].Text;
                sfd.Title = "Export file";

                sfd.ShowDialog();

                if (sfd.FileName != "")
                {
                    if (theFile.dataFork != null && theFile.dataFork.forkDataValues.logicalSize > 0)
                    {
                        dc.exportFile(theFile, forkStream.forkType.data, sfd.FileName);
                    }
                    
                    if (theFile.resourceFork != null && theFile.resourceFork.forkDataValues.logicalSize > 0)
                    {
                        dc.exportFile(theFile, forkStream.forkType.resource, sfd.FileName + ".rsrc");
                    }

                }
            }
            
        }
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            resetHex();
            switch ((string)comboBox1.SelectedItem)
            {
                case "Resource fork":
                    dc.fileDataBlock = 1;
                    showCurrentBlock.Text = dc.fileDataBlock.ToString();
                    showTotalBlocks.Text = dc.selectedFile.resourceFork.forkDataValues.totalBlocks.ToString();
                    dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.resource);

                    hexText.Text = dc.contentDisplay;

                    prevBlock.Enabled = false;
                    if (dc.selectedFile.resourceFork.forkDataValues.totalBlocks > 1)
                    {
                        nextBlock.Enabled = true;
                    }

                    blockNumBox.Enabled = true;
                    goToBlock.Enabled = true;
                    break;
                case "Data fork":
                    dc.fileDataBlock = 1;
                    showCurrentBlock.Text = dc.fileDataBlock.ToString();
                    showTotalBlocks.Text = dc.selectedFile.dataFork.forkDataValues.totalBlocks.ToString();
                    dc.showForkData(dc.selectedFile, dc.fileDataBlock - 1, forkStream.forkType.data);

                    hexText.Text = dc.contentDisplay;

                    prevBlock.Enabled = false;
                    if (dc.selectedFile.dataFork.forkDataValues.totalBlocks > 1)
                    {
                        nextBlock.Enabled = true;
                    }

                    blockNumBox.Enabled = true;
                    goToBlock.Enabled = true;
                    break;
                default:
                    break;
            }
        }
        private void directoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((int)e.Node.Handle != dc.lastSelectedNode)
            {
                dc.listViewRows.Clear();
                ItemsList.Items.Clear();

                if (e.Node.Tag is HFSPlusCatalogFolder)
                {
                    HFSPlusCatalogFolder details = (HFSPlusCatalogFolder)e.Node.Tag;
                    propertyGrid1.SelectedObject = details;
                }
                else if (e.Node.Tag is HFSPlus.volumeHeader)
                {
                    HFSPlus.volumeHeader details = (HFSPlus.volumeHeader)e.Node.Tag;
                    propertyGrid1.SelectedObject = details;
                }
                else if (e.Node.Tag is absImageStream.imageProperties)
                {
                    absImageStream.imageProperties details = (absImageStream.imageProperties)e.Node.Tag;
                    propertyGrid1.SelectedObject = details;
                }

                dc.generateListViewContent(e.Node);

                foreach (ListViewItem row in dc.listViewRows)
                {
                    ItemsList.Items.Add(row);
                }

                hashFileToolStripMenuItem.Enabled = true;

                dc.lastSelectedNode = (int)e.Node.Handle;
            }
        }
        private void sectorGrid_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            dc.drawMapView(g, sectorGrid, gridStartRow, SectorIcons);
            textBox1.Text = dc.sectorIDs;
        }
        private void sectorGrid_Resize(object sender, EventArgs e)
        {
            _resizing = true;
        }

        private void sectorGrid_SizeChanged(object sender, EventArgs e)
        {
            dc.setScrollSize(ref gridScroll, ref sectorGrid);
        }
        private void gridScroll_Scroll(object sender, ScrollEventArgs e)
        {
            this.gridStartRow = e.NewValue;
            sectorGrid.Invalidate();
            sectorGrid.Update();
        }
        private void sectorGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                resetHex();
                dc.selectedSector = (e.Location.X / dc.sectorSquareSize) +
                                    (sectorGrid.Width / dc.sectorSquareSize) *
                                    (e.Location.Y / dc.sectorSquareSize) +
                                    (gridScroll.Value * (sectorGrid.Width / dc.sectorSquareSize));
                dc.showSectorData(dc.selectedSector);
                hexText.Text = dc.contentDisplay;
                showCurrentBlock.Text = dc.selectedSector.ToString();
                showTotalBlocks.Text = (dc.i.Length / dc.i.sectorSize).ToString();
                sectorGrid.Invalidate();
                sectorGrid.Update();
            }
        }
        private void directoryTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is HFSPlusCatalogFolder)
            {
                // remove any dummy nodes
                e.Node.Nodes.Clear();

                // add the actual directories
                TreeNode children = dc.getSubDirectories(e.Node);
                foreach (TreeNode child in children.Nodes)
                {
                    if (child.Tag is HFSPlusCatalogFolder)
                    {
                        e.Node.Nodes.Add(child);
                    }
                }
            }
        }
        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mapGoToForm = new MapGoTo();
            _mapGoToForm.GoTo += MapGoTo;
            _mapGoToForm.ShowDialog();
        }
        protected virtual void MapGoTo(object sender, GoToEventArgs a)
        {
            if (a.Sector <= dc.i.Length / dc.i.sectorSize && a.Sector >= 0)
            {
                dc.selectedSector = a.Sector;
                int sectorsPerRow = sectorGrid.Width / dc.sectorSquareSize;
                if (dc.selectedSector > sectorsPerRow)
                {
                    this.gridStartRow = (dc.selectedSector / sectorsPerRow) - 1;
                }
                gridScroll.Value = (int)this.gridStartRow;
                sectorGrid.Invalidate();
                sectorGrid.Update();
            }
            else
            {
                MessageBox.Show("Value too large or too small.");
                _mapGoToForm = new MapGoTo();
                _mapGoToForm.GoTo += MapGoTo;
                _mapGoToForm.ShowDialog();
            }
        }
    }
}
