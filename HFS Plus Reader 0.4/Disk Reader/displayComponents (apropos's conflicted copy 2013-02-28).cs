using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Disk_Reader
{
    class displayComponents
    {
        public string imagepath;
        public TreeNode displayTree = new TreeNode();
        public TreeNode fileTree = new TreeNode();
        public List<ListViewItem> listViewRows = new List<ListViewItem>();
        public absImageStream i;
        public HFSPlusCatalogFile selectedFile;
        public uint fileDataBlock;
        public imageMap map = new imageMap();
        public string[] forkview = { "Data fork", "Resource fork" };

        string hexHeadLine = "\t\t00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F  "
            + "10  11  12  13  14  15  16  17  18  19  1A  1B  1C  1D  1E  1F\r\n";
        public string contentDisplay;
        public string help_about =  "Disk Reader" + System.Environment.NewLine +
                                    "Version: 0.3.3" + System.Environment.NewLine +
                                    "© Joash Lewis" + System.Environment.NewLine + System.Environment.NewLine +
                                    "This copy is for beta testing purposes only.";

        public displayComponents()
        {
            contentDisplay = hexHeadLine;
        }

        public void getImageContents(string imagepath)
        {
            i = accessImage(imagepath);

            setFileTreeFromImage(i);
            fileTree.Text = i.bf.F.Name;

            absImageStream.imageProperties tag = new absImageStream.imageProperties();
            tag.imagepath = i.bf.F.FullName;
            tag.componentSize = i.componentSize;
            tag.sectorSize = i.sectorSize;
            
            i.Position = 0;

            tag.scheme = i.scheme;
            tag.totalSize = i.Length;

            fileTree.Tag = tag;

            displayTree = stripFilesFromTree(fileTree);

        }
        private absImageStream accessImage(string imagepath)
        {
            FileInfo f = new FileInfo(imagepath);
            absImageStream ais;

            switch (f.Extension)
            {
                case ".001": case ".dmg":
                    i = new DDSetStream(imagepath);
                    ais = new DDSetStream(imagepath);

                    break;
                default:
                    i = new DDSetStream(imagepath);
                    ais = new DDSetStream(imagepath);
                    break;
            }

            return i;
        }
        private void setFileTreeFromImage(absImageStream i)
        {
            TreeNode partitionTN = new TreeNode();

            fileTree = null;
            fileTree = new TreeNode();

            switch (i.scheme)
            {
                case absImageStream.schemeType.GPT:
                    GPTScheme ps = new GPTScheme(i);
                    List<GPTScheme.entry> entries = new List<GPTScheme.entry>();

                    if (ps.protectiveMBRExists)
                    {
                        imageMap.mapTile protectiveMBR = new imageMap.mapTile();
                        protectiveMBR.name = "Protective MBR";
                        protectiveMBR.type = imageMap.tileType.partitionTable;
                        protectiveMBR.location = 0;
                        protectiveMBR.length = i.sectorSize;

                        map.tiles.Add(protectiveMBR);
                    }

                    if (ps.headerFound)
                    {
                        imageMap.mapTile pTable = new imageMap.mapTile();
                        pTable.name = "GUID Partition Table";
                        pTable.type = imageMap.tileType.partitionTable;
                        pTable.location = i.sectorSize;
                        pTable.length = ps.numberofentries * ps.sizeofentry + i.sectorSize;

                        if (ps.headerMatchesCRC)
                        {
                            entries = ps.entries;
                            pTable.metadata = entries;
                        }

                        map.tiles.Add(pTable);
                    }

                    if (ps.backupFound)
                    {
                        imageMap.mapTile pTable = new imageMap.mapTile();
                        pTable.name = "Backup GUID Partition Table";
                        pTable.type = imageMap.tileType.partitionTable;
                        pTable.length = ps.numberofentries * ps.sizeofentry + i.sectorSize;
                        pTable.location = i.Length - pTable.length;

                        if (ps.backupMatchesCRC)
                        {
                            entries = ps.backupEntries;
                            pTable.metadata = entries;
                        }

                        map.tiles.Add(pTable);
                    }



                    foreach (GPTScheme.entry partition in entries)
                    {
                        GPTScheme.partitionType type = ps.findPartitionType(partition);

                        partitionTN = getVolumeTree(partition, type);
                        partitionTN.Text = partition.name;
                        fileTree.Nodes.Add(partitionTN);

                        imageMap.mapTile partitionTile = new imageMap.mapTile();
                        partitionTile.name = partition.name;
                        partitionTile.type = imageMap.tileType.partition;
                        partitionTile.location = partition.partStartLBA * i.sectorSize;
                        partitionTile.length = partition.partLength * i.sectorSize;
                        partitionTile.metadata = partitionTN.Tag;

                        map.tiles.Add(partitionTile);
                    }
                    break;
                default:
                    break;
            }
        }
        private TreeNode getVolumeTree(GPTScheme.entry partition, GPTScheme.partitionType type)
        {
            TreeNode tn = new TreeNode();

            if (type == GPTScheme.partitionType.HFSPlus)
            {
                HFSPlus hfsp = new HFSPlus(i, partition);
                volumeStream hfsp_vs = new volumeStream(hfsp);

                HFSPlusFile rawCatalog = new HFSPlusFile(hfsp.volHead.catalogFile);
                HFSPlusFile rawAttributes = new HFSPlusFile(hfsp.volHead.attributesFile);

                catalogFile catalog = new catalogFile(rawCatalog, hfsp_vs);
                attributesFile attributes = new attributesFile(rawAttributes, hfsp_vs);

                tn = hfsp.getFullDirectoryList();
                tn.Tag = hfsp.volHead;
            }

            return tn;
        }
        private TreeNode getVolumeTree(GPTScheme.entry partition, GPTScheme.partitionType type, HFSPlusCatalogFolder folderID)
        {
            TreeNode tn = new TreeNode();
            try
            {
                if (type == GPTScheme.partitionType.HFSPlus)
                {
                    HFSPlus hfsp = new HFSPlus(this.i, partition);


                    tn = getHFSPTree(hfsp, folderID);
                }
            }
            catch (OutOfMemoryException)
            {
                return tn;
                throw new OutOfMemoryException( "The list view has been truncated as there are too many items to fit in system memory.\r\n\r\n"
                                            +   "Try viewing a sub directory instead.");
            }

            return tn;
        }
        private TreeNode getHFSPTree(HFSPlus hfsp, HFSPlusCatalogFolder folderID)
        {
            TreeNode tn = new TreeNode();
            volumeStream hfsp_vs = new volumeStream(hfsp);

            HFSPlusFile rawCatalog = new HFSPlusFile(hfsp.volHead.catalogFile);
            HFSPlusFile rawAttributes = new HFSPlusFile(hfsp.volHead.attributesFile);
            HFSPlusFile rawExtentsOverflow = new HFSPlusFile(hfsp.volHead.extentsFile);
            // need to get all attributes files
            
            HFSPlusCatalogFolder folderRecord = folderID;

            catalogFile catalog = new catalogFile(rawCatalog, hfsp_vs);
            attributesFile attributes = new attributesFile(rawAttributes, hfsp_vs);
            extentsOverflowFile eof = new extentsOverflowFile(rawExtentsOverflow, hfsp_vs);
            displayTree = hfsp.getFullDirectoryList(folderRecord, catalog, eof, attributes);

            tn = displayTree;

            return tn;
        }
        private TreeNode stripFilesFromTree(TreeNode fileTree)
        {
            TreeNode replaceParent = new TreeNode();
            replaceParent.Tag = fileTree.Tag;
            replaceParent.Text = fileTree.Text;

            foreach (TreeNode childItem in fileTree.Nodes)
            {
                if (childItem != null)
                {
                    if (childItem.Tag is HFSPlusCatalogFile)
                    {
                    }
                    else if (childItem.Tag is attributesLeafNode.HFSPlusAttrInlineData)
                    {
                    }
                    else if (childItem.Tag is attributesLeafNode.HFSPlusAttrForkData)
                    {
                    }
                    else
                    {
                        replaceParent.Nodes.Add(stripFilesFromTree(childItem));
                    }
                }
            }

            return replaceParent;
        }
        private ListViewItem getNodeRowContents(TreeNode theTree)
        {
            ListViewItem row = new ListViewItem(theTree.Text);
            if (theTree.Tag != null)
            {
                string tagType = theTree.Tag.GetType().ToString();

                switch (tagType)
                {
                    case "Disk_Reader.HFSPlusCatalogFolder":
                        HFSPlusCatalogFolder folderTag = (HFSPlusCatalogFolder)theTree.Tag;
                        row.Tag = folderTag;


                        row.SubItems.Add(folderTag.folderID.ToString());
                        if (folderTag.createDate > HFSPlus.FromHFSPlusTime(0))
                        { row.SubItems.Add(folderTag.createDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (folderTag.contentModDate > HFSPlus.FromHFSPlusTime(0))
                        { row.SubItems.Add(folderTag.contentModDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (folderTag.attributeModDate > HFSPlus.FromHFSPlusTime(0))
                        { row.SubItems.Add(folderTag.attributeModDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (folderTag.backupDate > HFSPlus.FromHFSPlusTime(0))
                        { row.SubItems.Add(folderTag.backupDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (folderTag.accessDate > HFSPlus.FromHFSPlusTime(0))
                        { row.SubItems.Add(folderTag.accessDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        string folderPermissions = "";
                        if (folderTag.permissions.fileMode.owner.read) folderPermissions += "r"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.owner.write) folderPermissions += "w"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.owner.execute) folderPermissions += "x"; else folderPermissions += "-";
                        folderPermissions += "/";
                        if (folderTag.permissions.fileMode.group.read) folderPermissions += "r"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.group.write) folderPermissions += "w"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.group.execute) folderPermissions += "x"; else folderPermissions += "-";
                        folderPermissions += "/";
                        if (folderTag.permissions.fileMode.other.read) folderPermissions += "r"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.other.write) folderPermissions += "w"; else folderPermissions += "-";
                        if (folderTag.permissions.fileMode.other.execute) folderPermissions += "x"; else folderPermissions += "-";
                        row.SubItems.Add(folderPermissions);
                        row.SubItems.Add("");           // data fork size
                        row.SubItems.Add("");           // resource fork size
                        row.SubItems.Add("");           // data start sector LBA
                        row.SubItems.Add("");           // rsrc start sector
                        row.SubItems.Add("");           // data fragments count
                        row.SubItems.Add("");           // rsrc fragments count
                        row.SubItems.Add("");           // data fork MD5
                        row.SubItems.Add("");           // data fork SHA1
                        row.SubItems.Add("");           // resource fork MD5
                        row.SubItems.Add("");           // resource fork SHA1
                        row.SubItems.Add("");           // is deleted
                        row.SubItems.Add(folderTag.path);

                        break;
                    case "Disk_Reader.HFSPlus+volumeHeader":
                        HFSPlus.volumeHeader headerTag = (HFSPlus.volumeHeader)theTree.Tag;
                        row.Tag = headerTag;

                        row.SubItems.Add("");           // CNID
                        if (headerTag.createDate > HFSPlus.FromHFSPlusTime(0))
                        {
                            row.SubItems.Add(headerTag.createDate.ToString());
                        }
                        else { row.SubItems.Add(""); }

                        if (headerTag.modifyDate > HFSPlus.FromHFSPlusTime(0))
                        {
                            row.SubItems.Add(headerTag.modifyDate.ToString());
                        }
                        else { row.SubItems.Add(""); }
                        row.SubItems.Add("");           // attribute mod date

                        if (headerTag.backupDate > HFSPlus.FromHFSPlusTime(0))
                        {
                            row.SubItems.Add(headerTag.backupDate.ToString());
                        }
                        else { row.SubItems.Add(""); }
                        row.SubItems.Add("");           // access date
                        row.SubItems.Add("");           // permissions
                        row.SubItems.Add("");           // data fork size
                        row.SubItems.Add("");           // resource fork size
                        row.SubItems.Add("");           // data start sector LBA
                        row.SubItems.Add("");           // rsrc start sector
                        row.SubItems.Add("");           // data fragments count
                        row.SubItems.Add("");           // rsrc fragments count
                        row.SubItems.Add("");           // data fork MD5
                        row.SubItems.Add("");           // data fork SHA1
                        row.SubItems.Add("");           // resource fork MD5
                        row.SubItems.Add("");           // resource fork SHA1
                        row.SubItems.Add("");           // is deleted
                        row.SubItems.Add(headerTag.path);

                        break;
                    case "Disk_Reader.HFSPlusCatalogFile":

                        HFSPlusCatalogFile fileTag = (HFSPlusCatalogFile)theTree.Tag;
                        row.Tag = fileTag;
                        
                        row.SubItems.Add(fileTag.fileID.ToString());
                        if (fileTag.createDate > HFSPlus.FromHFSPlusTime(0))                                // creation date
                        { row.SubItems.Add(fileTag.createDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (fileTag.contentModDate > HFSPlus.FromHFSPlusTime(0))                            // content mod date
                        { row.SubItems.Add(fileTag.contentModDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (fileTag.attributeModDate > HFSPlus.FromHFSPlusTime(0))                          // attributes mod date
                        { row.SubItems.Add(fileTag.attributeModDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (fileTag.backupDate > HFSPlus.FromHFSPlusTime(0))                                // backup date
                        { row.SubItems.Add(fileTag.backupDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        if (fileTag.accessDate > HFSPlus.FromHFSPlusTime(0))                                // access date - Mac OS X does not use this - only POSIX implementations
                        { row.SubItems.Add(fileTag.accessDate.ToString()); }
                        else { row.SubItems.Add(""); }

                        string filePermissions = "";
                        if (fileTag.permissions.fileMode.owner.read) filePermissions += "r"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.owner.write) filePermissions += "w"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.owner.execute) filePermissions += "x"; else filePermissions += "-";
                        filePermissions += "/";
                        if (fileTag.permissions.fileMode.group.read) filePermissions += "r"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.group.write) filePermissions += "w"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.group.execute) filePermissions += "x"; else filePermissions += "-";
                        filePermissions += "/";
                        if (fileTag.permissions.fileMode.other.read) filePermissions += "r"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.other.write) filePermissions += "w"; else filePermissions += "-";
                        if (fileTag.permissions.fileMode.other.execute) filePermissions += "x"; else filePermissions += "-";
                        row.SubItems.Add(filePermissions);                                                  // file permissions
                        row.SubItems.Add(fileTag.dataFork.forkDataValues.logicalSize.ToString());           // data fork size
                        int rsrccount = 0;
                        if (fileTag.resourceFork != null)
                        {
                            row.SubItems.Add(fileTag.resourceFork.forkDataValues.logicalSize.ToString());

                            // only try to iterate through resource fork extents if a resource fork exists
                            // (volume metadata files do not have a resource fork)
                            for (int i = 0; i < fileTag.dataFork.forkDataValues.extents.Count(); i++)
                            {
                                if (fileTag.resourceFork.forkDataValues.extents[i].blockCount > 0)
                                {
                                    rsrccount++;
                                }
                            }
                        }
                        else
                        {
                            row.SubItems.Add("0");                                                         // resource fork size
                        }
                        row.SubItems.Add(fileTag.dataFork.forkDataValues.extents[0].startBlock.ToString()); // start sector LBA
                        if (fileTag.resourceFork != null) row.SubItems.Add(fileTag.resourceFork.forkDataValues.extents[0].startBlock.ToString());
                        else row.SubItems.Add("0");                                                         // resource start sector

                        int datacount = 0;
                        for (int i = 0; i < fileTag.dataFork.forkDataValues.extents.Count(); i++)
                        {
                            if (fileTag.dataFork.forkDataValues.extents[i].blockCount > 0)
                            {
                                datacount++;
                            }
                        }
                        row.SubItems.Add(datacount.ToString());                                             // data fragments count

                        row.SubItems.Add(rsrccount.ToString());                                             // resource fragments count
                        row.SubItems.Add("");                                                               // data fork MD5
                        row.SubItems.Add("");                                                               // data fork SHA1
                        row.SubItems.Add("");                                                               // resource fork MD5
                        row.SubItems.Add("");                                                               // resource fork SHA1
                        row.SubItems.Add("");                                                               // is deleted
                        row.SubItems.Add(fileTag.path);

                        break;
                    case "Disk_Reader.attributesLeafNode+HFSPlusAttrForkData":
                        break;
                    case "Disk_Reader.attributesLeafNode+HFSPlusAttrInlineData":
                        attributesLeafNode.HFSPlusAttrInlineData inlineTag = (attributesLeafNode.HFSPlusAttrInlineData)theTree.Tag;
                        row.Tag = inlineTag;


                        row.SubItems.Add(inlineTag.key.fileID.ToString());
                        row.SubItems.Add("");           // creation date
                        row.SubItems.Add("");           // content mod date
                        row.SubItems.Add("");           // attributes mod date
                        row.SubItems.Add("");           // backup date
                        row.SubItems.Add("");           // access date
                        row.SubItems.Add("");           // file permissions
                        row.SubItems.Add(inlineTag.otherData.Length.ToString());           // data fork size
                        row.SubItems.Add("");           // resource fork size
                        row.SubItems.Add("");           // data start sector LBA
                        row.SubItems.Add("");           // rsrc start sector LBA
                        row.SubItems.Add("");           // data fragments count
                        row.SubItems.Add("");           // rsrc fragments count
                        row.SubItems.Add("");           // data fork MD5
                        row.SubItems.Add("");           // data fork SHA1
                        row.SubItems.Add("");           // resource fork MD5
                        row.SubItems.Add("");           // resource fork SHA1
                        row.SubItems.Add("");           // is deleted
                        row.SubItems.Add("");           // path
                        break;
                }
            }
            return row;
        }
        public void generateListViewContent(TreeNode startDirectory)
        {
            TreeNode partitionTN = new TreeNode();

            if (startDirectory.Tag is absImageStream.imageProperties)
            {
                switch (i.scheme)
                {
                    case absImageStream.schemeType.GPT:
                        GPTScheme ps = new GPTScheme(i);

                        foreach (GPTScheme.entry partition in ps.entries)
                        {
                            GPTScheme.partitionType type = ps.findPartitionType(partition);

                            partitionTN = getVolumeTree(partition, type);
                            partitionTN.Text = partition.name;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (startDirectory.Tag is HFSPlusCatalogFolder)
            {
                HFSPlusCatalogFolder tag = (HFSPlusCatalogFolder)startDirectory.Tag;
                switch (i.scheme)
                {
                    case absImageStream.schemeType.GPT:
                        GPTScheme ps = new GPTScheme(i);


                        partitionTN = getVolumeTree(ps.entries[tag.partitionAssoc], GPTScheme.partitionType.HFSPlus, tag);
                        partitionTN.Text = startDirectory.Text;

                        break;
                    default:
                        break;
                }
            }
            else if (startDirectory.Tag is HFSPlus.volumeHeader)
            {
                HFSPlus.volumeHeader tag = (HFSPlus.volumeHeader) startDirectory.Tag;
                switch (i.scheme)
                {
                    case absImageStream.schemeType.GPT:
                        GPTScheme ps = new GPTScheme(i);


                        partitionTN = getVolumeTree(ps.entries[tag.partitionNo], GPTScheme.partitionType.HFSPlus);
                        partitionTN.Text = startDirectory.Text;

                        break;
                    default:
                        break;
                }
            }

            if (startDirectory.Tag != null)
            {
                addRowsToList(partitionTN);
            }

        }
        private void addRowsToList(TreeNode theTree)
        {
            ListViewItem row = getNodeRowContents(theTree);
            this.listViewRows.Add(row);

            foreach (TreeNode subNode in theTree.Nodes)
            {
                addRowsToList(subNode);
            }
        }
        public void showForkData(HFSPlusCatalogFile entry, uint block, forkStream.forkType type)
        {
            GPTScheme gpts = new GPTScheme(i);
            HFSPlus hfsp = new HFSPlus(i, gpts.entries[entry.partitionAssoc]);
            volumeStream vs = new volumeStream(hfsp);
            extentsOverflowFile eof = new extentsOverflowFile(new HFSPlusFile(hfsp.volHead.extentsFile), vs);

            HFSPlusFile hfsp_file = new HFSPlusFile(entry, eof);
            forkStream fs;
            if (type == forkStream.forkType.data)
            {
                fs = new forkStream(vs, hfsp_file, forkStream.forkType.data);
            }
            else
            {
                fs = new forkStream(vs, hfsp_file, forkStream.forkType.resource);
            }

            contentDisplay = hexHeadLine + "\r\n";

            if (fs.Length > 0)
            {
                byte[] showBlock = new byte[hfsp.blockSize];

                fs.Seek(hfsp.blockSize * block, SeekOrigin.Begin);
                fs.Read(showBlock, 0, (int)hfsp.blockSize);

                rawDataDisplay(showBlock);
            }
        }
        public void showForkData(HFSPlusCatalogFile entry, forkStream.forkType type)
        {
            GPTScheme gpts = new GPTScheme(i);
            HFSPlus hfsp = new HFSPlus(i, gpts.entries[entry.partitionAssoc]);
            volumeStream vs = new volumeStream(hfsp);
            extentsOverflowFile eof = new extentsOverflowFile(new HFSPlusFile(hfsp.volHead.extentsFile), vs);

            HFSPlusFile hfsp_file = new HFSPlusFile(entry, eof);
            forkStream fs;
            if (type == forkStream.forkType.data)
            {
                fs = new forkStream(vs, hfsp_file, forkStream.forkType.data);
            }
            else
            {
                fs = new forkStream(vs, hfsp_file, forkStream.forkType.resource);
            }

            throw new NotImplementedException();
        }
        public void showInlineAttrData(attributesLeafNode.HFSPlusAttrInlineData entry)
        {
            rawDataDisplay(entry.otherData);
        }
        public void exportFile(HFSPlusCatalogFile entry, forkStream.forkType type, string path)
        {
            if (entry.dataFork.forkDataValues.logicalSize > 0 || entry.resourceFork.forkDataValues.logicalSize > 0)
            {
                GPTScheme gpts = new GPTScheme(i);
                HFSPlus hfsp = new HFSPlus(i, gpts.entries[entry.partitionAssoc]);
                volumeStream vs = new volumeStream(hfsp);
                extentsOverflowFile eof = new extentsOverflowFile(new HFSPlusFile(hfsp.volHead.extentsFile), vs);

                HFSPlusFile hfsp_file = new HFSPlusFile(entry, eof);
                forkStream fs;
                long dataSize = 0;

                if (type == forkStream.forkType.data)
                {
                    fs = new forkStream(vs, hfsp_file, forkStream.forkType.data);
                    dataSize = (long)entry.dataFork.forkDataValues.logicalSize;
                }
                else
                {
                    fs = new forkStream(vs, hfsp_file, forkStream.forkType.resource);
                    dataSize = (long)entry.resourceFork.forkDataValues.logicalSize;
                }

                fs.Position = 0;

                FileStream writeStream = new FileStream(path, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(writeStream);

                long bytesWritten = 0;
                byte[] buffer;

                while (bytesWritten < dataSize)
                {
                    if (bytesWritten + 8192 <= dataSize)
                    {
                        buffer = new byte[8192];
                        fs.Read(buffer, 0, 8192);

                        bw.Write(buffer, 0, 8192);

                        bytesWritten += 8192;
                    }
                    else
                    {
                        buffer = new byte[dataSize - bytesWritten];
                        fs.Read(buffer, 0, buffer.Length);

                        bw.Write(buffer, 0, buffer.Length);

                        bytesWritten += buffer.Length;
                    }
                }

                bw.Close();
                writeStream.Close();
            }
        }
        private void rawDataDisplay(byte[] showData)
        {
            contentDisplay = this.hexHeadLine;

            int counter = 0;
            int linecounter = 0;
            string hexLine = "";
            string asciiLine = "";
            bool lineDisplayed = false;

            foreach (byte b in showData)
            {
                lineDisplayed = false;
                if (counter == 0)
                {
                    hexLine = "";
                    asciiLine = "";

                    byte[] lineCounterTextBytes = BitConverter.GetBytes(linecounter * 32);
                    contentDisplay += "\r\n0x" + lineCounterTextBytes[1].ToString("X2") + lineCounterTextBytes[0].ToString("X2") + "\t\t";
                    counter++;

                    hexLine += b.ToString("X2") + "  ";
                    if (Char.IsControl(Convert.ToChar(b)))
                    {
                        asciiLine += Convert.ToChar(0xB7);
                    }
                    else
                    {
                        asciiLine += Convert.ToChar(b);
                    }
                }
                else if (counter == 31)
                {
                    hexLine += b.ToString("X2") + "  ";
                    if (Char.IsControl(Convert.ToChar(b)))
                    {
                        asciiLine += Convert.ToChar(0xB7);
                    }
                    else
                    {
                        asciiLine += Convert.ToChar(b);
                    }

                    contentDisplay += hexLine + "\t" + asciiLine;
                    lineDisplayed = true;

                    linecounter++;
                    counter = 0;
                }
                else
                {
                    hexLine += b.ToString("X2") + "  ";
                    if (Char.IsControl(Convert.ToChar(b)))
                    {
                        asciiLine += Convert.ToChar(0xB7);
                    }
                    else
                    {
                        asciiLine += Convert.ToChar(b);
                    }

                    counter++;
                }

            }

            if (!lineDisplayed)
            {
                int remaining = 32 - counter;

                for (int i = 0; i < remaining; i++)
                {
                    hexLine += "    ";
                }

                contentDisplay += hexLine + "\t" + asciiLine;
            }

        }
        public dataOperations.hashValues addImageHash()
        {
            dataOperations.hashValues hashbytes = dataOperations.getHashValues(i, i.Length);

            return hashbytes;
        }
        public void hashAll()
        {
            TreeNode replaceFileTree = new TreeNode();
            setFileTreeFromImage(i);

            foreach (TreeNode child in fileTree.Nodes)
            {
                if (child.Tag is HFSPlus.volumeHeader)
                {
                    HFSPlus.volumeHeader vh = (HFSPlus.volumeHeader)child.Tag;

                    GPTScheme gpts = new GPTScheme(i);
                    HFSPlus hfsp = new HFSPlus(i, gpts.entries[vh.partitionNo]);
                    volumeStream vs = new volumeStream(hfsp);

                    replaceFileTree.Nodes.Add(iterateHashChildren(child, vs));
                }
                else
                {
                    replaceFileTree.Nodes.Add(child);
                }
            }

            replaceFileTree.Tag = displayTree.Tag;

            this.fileTree = replaceFileTree;

        }
        private TreeNode iterateHashChildren(TreeNode parent, volumeStream vs)
        {
            TreeNode replaceParent = new TreeNode();
            replaceParent.Tag = parent.Tag;


            foreach (TreeNode child in parent.Nodes)
            {
                TreeNode replaceChild = new TreeNode();

                if (child.Tag is HFSPlusCatalogFolder)
                {
                    replaceChild = iterateHashChildren(child, vs);
                    replaceChild.Tag = child.Tag;
                }
                else if (child.Tag is HFSPlusCatalogFile)
                {
                    HFSPlusCatalogFile tag = (HFSPlusCatalogFile)child.Tag;
                    dataOperations.hashValues hashes = new dataOperations.hashValues();

                    if (tag.dataFork != null && tag.dataFork.forkDataValues.logicalSize > 0)
                    {
                        HFSPlusFile theFileData = new HFSPlusFile(tag.dataFork);

                        forkStream fs = new forkStream(vs, theFileData, forkStream.forkType.data);

                        dataOperations.hashValues hv = dataOperations.getHashValues(fs, (long)theFileData.dataLogicalSize);

                        hashes.md5hash = hv.md5hash;
                    }

                    if (tag.resourceFork != null && tag.resourceFork.forkDataValues.logicalSize > 0)
                    {
                        HFSPlusFile theFileResource = new HFSPlusFile(tag.dataFork);

                        forkStream fs = new forkStream(vs, theFileResource, forkStream.forkType.data);

                        dataOperations.hashValues hv = dataOperations.getHashValues(fs, (long)theFileResource.dataLogicalSize);

                        hashes.sha1hash = hv.sha1hash;
                    }

                    tag.hashes = hashes;

                    replaceChild.Tag = tag;
                }
                else
                {
                    replaceChild.Tag = child.Tag;
                }

                replaceChild.Text = child.Text;
                replaceParent.Nodes.Add(replaceChild);
            }

            replaceParent.Text = parent.Text;

            return replaceParent;
        }
        public dataOperations.hashValues[] hashFile(HFSPlusCatalogFile file)
        {
            dataOperations.hashValues[] hv = new dataOperations.hashValues[2];

            GPTScheme gpts = new GPTScheme(i);
            HFSPlus hfsp = new HFSPlus(i, gpts.entries[file.partitionAssoc]);

            volumeStream vs = new volumeStream(hfsp);

            if (file.dataFork.forkDataValues.logicalSize > 0)
            {
                HFSPlusFile hfspfile = new HFSPlusFile(file.dataFork);
                forkStream fs = new forkStream(vs, hfspfile, forkStream.forkType.data);

                hv[0] = dataOperations.getHashValues(fs, (long)hfspfile.dataLogicalSize);
            }

            if(file.resourceFork.forkDataValues.logicalSize > 0)
            {
                HFSPlusFile hfspfile = new HFSPlusFile(file.resourceFork);
                forkStream fs = new forkStream(vs, hfspfile, forkStream.forkType.resource);
                
                hv[1] = dataOperations.getHashValues(fs, (long)hfspfile.dataLogicalSize);
            }

            return hv;
        }

    }
}
