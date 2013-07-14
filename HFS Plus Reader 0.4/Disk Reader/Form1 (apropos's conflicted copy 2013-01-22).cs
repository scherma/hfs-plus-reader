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
    public partial class Form1 : Form
    {
        string imagepath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectfile = new OpenFileDialog();
            selectfile.Filter = "DD Files|*.001";
            selectfile.Title = "Select Image File";
            if (selectfile.ShowDialog() == DialogResult.OK)
            {
                string text="";
                
                imagepath = selectfile.FileName;
                FileInfo f = new FileInfo(imagepath);
                absImage i;

                switch (f.Extension)
                {
                    case ".001":
                        i = new DDSet(imagepath);
                        break;
                    default:
                        i = new DDSet(imagepath);
                        break;
                }

                switch (i.scheme)
                {
                    case absImage.schemeType.GPT:
                        GPTScheme ps = new GPTScheme(i);

                        foreach (GPTScheme.entry partition in ps.entries)
                        {
                            text += "Name: " + partition.name + "\r\n" +
                            "Start block: " + partition.partStartLBA.ToString() + "\r\n" +
                            "End block: " + partition.partEndLBA.ToString() + "\r\n" +
                            "Type GUID: " + partition.parttype_guid.ToString() + "\r\n" +
                            "GUID: " + partition.part_guid.ToString() + "\r\n" +
                            "\r\n";

                            if (ps.findPartitionType(partition) == GPTScheme.partitionType.HFSPlus)
                            {
                                HFSPlus hfsp = new HFSPlus(i, partition);

                                HFSPlusFile rawCatalog = new HFSPlusFile(hfsp.volHead.catalogFile);

                                catalogFile catalog = hfsp.getCatalog(rawCatalog);
                                HFSPlus.Directory fullList = hfsp.getFullDirectoryList();

                                string rootFolderName = System.Text.Encoding.BigEndianUnicode.GetString(fullList.properties.key.nodeName);

                                DateTime dt = hfsp.FromHFSPlusTime(hfsp.volHead.createDate);

                                text += "Date Created: " + dt.ToString() + "\r\n\r\n";

                                text += "Root Folder Name: " + rootFolderName;
                            }
                        }
                        break;
                    default:
                        break;
                }


                
                textBox1.Text = text;

            }
        }

        public void addChildrenToTree(HFSPlus.Directory directoryStructure, TreeNode rootNode)
        {
            foreach (HFSPlus.File childFile in directoryStructure.childFiles)
            {
                TreeNode currentFile = new TreeNode(System.Text.Encoding.BigEndianUnicode.GetString(childFile.properties.key.nodeName));
            }
        }


    }
}
