/*
 *  This file is part of HFS+ Reader.
 *
 *  HFS+ Reader is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  HFS+ Reader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with HFS+ Reader.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Cryptography;
using System.IO;

namespace Disk_Reader
{
    class HFSPlus : absVolume
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        new public struct volumeHeader
        {
            private ushort signatureVal;
            private ushort versionVal;
            private uint attributesVal;
            private volAttributes volAttrVal;
            private uint lastMountedVersionVal;
            private uint journalInfoBlockVal;

            private DateTime createDateVal;
            private DateTime modifyDateVal;
            private DateTime backupDateVal;
            private DateTime checkedDateVal;

            private uint fileCountVal;
            private uint folderCountVal;

            private uint blockSizeVal;
            private uint totalBlocksVal;
            private uint freeBlocksVal;

            private uint nextAllocationVal;
            private uint rsrcClumpSizeVal;
            private uint dataClumpSizeVal;
            private uint nextCatalogIDVal;

            private uint writeCountVal;
            private ulong encodingsBitmapVal;

            private uint[] finderInfoVal;

            [TypeConverter(typeof(ExpandableObjectConverter))]
            private hfsPlusForkData allocationFileVal;
            [TypeConverter(typeof(ExpandableObjectConverter))]
            private hfsPlusForkData extentsFileVal;
            [TypeConverter(typeof(ExpandableObjectConverter))]
            private hfsPlusForkData catalogFileVal;
            [TypeConverter(typeof(ExpandableObjectConverter))]
            private hfsPlusForkData attributesFileVal;
            [TypeConverter(typeof(ExpandableObjectConverter))]
            private hfsPlusForkData startupFileVal;

            private int partitionNoVal;
            private string pathVal;

            public ushort signature { get { return signatureVal; } set { signatureVal = value; } }
            public ushort version { get { return versionVal; } set { versionVal = value; } }
            public uint attributes { get { return attributesVal; } set { attributesVal = value; } }
            public volAttributes volAttr { get { return volAttrVal; } set { volAttrVal = value; } }
            public uint lastMountedVersion { get { return lastMountedVersionVal; } set { lastMountedVersionVal = value; } }
            public uint journalInfoBlock { get { return journalInfoBlockVal; } set { journalInfoBlockVal = value; } }

            public DateTime createDate { get { return createDateVal; } set { createDateVal = value; } }
            public DateTime modifyDate { get { return modifyDateVal; } set { modifyDateVal = value; } }
            public DateTime backupDate { get { return backupDateVal; } set { backupDateVal = value; } }
            public DateTime checkedDate { get { return checkedDateVal; } set { checkedDateVal = value; } }

            public uint fileCount { get { return fileCountVal; } set { fileCountVal = value; } }
            public uint folderCount { get { return folderCountVal; } set { folderCountVal = value; } }

            public uint blockSize { get { return blockSizeVal; } set { blockSizeVal = value; } }
            public uint totalBlocks { get { return totalBlocksVal; } set { totalBlocksVal = value; } }
            public uint freeBlocks { get { return freeBlocksVal; } set { freeBlocksVal = value; } }

            public uint nextAllocation { get { return nextAllocationVal; } set { nextAllocationVal = value; } }
            public uint rsrcClumpSize { get { return rsrcClumpSizeVal; } set { rsrcClumpSizeVal = value; } }
            public uint dataClumpSize { get { return dataClumpSizeVal; } set { dataClumpSizeVal = value; } }
            public uint nextCatalogID { get { return nextCatalogIDVal; } set { nextCatalogIDVal = value; } }

            public uint writeCount { get { return writeCountVal; } set { writeCountVal = value; } }
            public ulong encodingsBitmap { get { return encodingsBitmapVal; } set { encodingsBitmapVal = value; } }

            public uint[] finderInfo { get { return finderInfoVal; } set { finderInfoVal = value; } }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData allocationFile { get { return allocationFileVal; } set { allocationFileVal = value; } }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData extentsFile { get { return extentsFileVal; } set { extentsFileVal = value; } }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData catalogFile { get { return catalogFileVal; } set { catalogFileVal = value; } }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData attributesFile { get { return attributesFileVal; } set { attributesFileVal = value; } }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData startupFile { get { return startupFileVal; } set { startupFileVal = value; } }

            public int partitionNo { get { return partitionNoVal; } set { partitionNoVal = value; } }
            public string path { get { return pathVal; } set { pathVal = value; } }

            public bool fileCountVerified { get; set; }
            public bool folderCountVerified { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct volAttributes
        {
            /* Bits 0-6 are reserved */
            public bool kHFSVolumeHardwareLockBit { get; set; }
            public bool kHFSVolumeUnmountedBit { get; set; }
            public bool kHFSVolumeSparedBlocksBit { get; set; }
            public bool kHFSVolumeNoCacheRequiredBit { get; set; }
            public bool kHFSBootVolumeInconsistentBit { get; set; }
            public bool kHFSCatalogNodeIDsReusedBit { get; set; }
            public bool kHFSVolumeJournaledBit { get; set; }
            /* Bit 14 is reserved */
            public bool kHFSVolumeSoftwareLockBit { get; set; }
            /* Bits 16-31 are reserved */
        }
        public enum volAttrFlags
        {
            /* Bits 0-6 are reserved */
            kHFSVolumeHardwareLockBit = 0x00000080,
            kHFSVolumeUnmountedBit = 0x00000100,
            kHFSVolumeSparedBlocksBit = 0x00000200,
            kHFSVolumeNoCacheRequiredBit = 0x00000400,
            kHFSBootVolumeInconsistentBit = 0x00000800,
            kHFSCatalogNodeIDsReusedBit = 0x00001000,
            kHFSVolumeJournaledBit = 0x00002000,
            /* Bit 14 is reserved */
            kHFSVolumeSoftwareLockBit = 0x00008000
            /* Bits 16-31 are reserved */
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public volumeHeader volHead { get; set; }
        public volumeHeader backupVolHead { get; set; }

        public int partitionNo { get; set; }
        public string path { get; set; }
        private long totalSize;
        public int filecount = 0;
        public int foldercount = 0;

        public HFSPlus(absImageStream fileSet, GPTScheme.entry partition) : base(fileSet, partition)
        {
            this.blockSize = 4096; // default block size

            this.partitionNo = partition.partitionNo;
            this.path = fileSet.bf.F.Name + "\\" + partition.name;


            byte[] rawHeader = new byte[512];
            ais.Seek(this.volumeStart + 1024, SeekOrigin.Begin);
            ais.Read(rawHeader, 0, 512);
            if (rawHeader[0] == 0x48 && rawHeader[1] == 0x2B)
            {
                volHead = getVolumeHeader(rawHeader);
            }

            long volEnd = this.volumeStart + (partition.partLength * ais.sectorSize) + ais.sectorSize;
            this.totalSize = partition.partLength * ais.sectorSize;
            ais.Seek(volEnd - 1024, SeekOrigin.Begin);
            ais.Read(rawHeader, 0, 512);
            if (rawHeader[0] == 0x48 && rawHeader[1] == 0x2B)
            {
                backupVolHead = getVolumeHeader(rawHeader);
            }
        }
        protected volumeHeader getVolumeHeader(byte[] rawHeader)
        {
            volumeHeader vh = new volumeHeader();       

            vh.signature = BitConverter.ToUInt16(rawHeader, 0);
            vh.version = BitConverter.ToUInt16(rawHeader, 2);
            vh.attributes = BitConverter.ToUInt32(rawHeader, 4);
            volAttributes volAttr = new volAttributes();
            byte[] attrBytes = new byte[4];
            Array.Copy(rawHeader, 4, attrBytes, 0, 4);

            volAttr.kHFSVolumeHardwareLockBit = ((vh.attributes & 0x80000000) == 0x80000000);
            volAttr.kHFSVolumeUnmountedBit = ((vh.attributes & 0x00010000) == 0x00010000);
            volAttr.kHFSVolumeSparedBlocksBit = ((vh.attributes & 0x00020000) == 0x00020000);
            volAttr.kHFSVolumeNoCacheRequiredBit = ((vh.attributes & 0x00040000) == 0x00040000);
            volAttr.kHFSBootVolumeInconsistentBit = ((vh.attributes & 0x0008000) == 0x00080000);
            volAttr.kHFSCatalogNodeIDsReusedBit = ((vh.attributes & 0x00100000) == 0x00100000);
            volAttr.kHFSVolumeJournaledBit = ((vh.attributes & 0x00200000) == 0x00200000);
            volAttr.kHFSVolumeSoftwareLockBit = ((vh.attributes & 0x00800000) == 0x00800000);

            vh.volAttr = volAttr;

            vh.lastMountedVersion = BitConverter.ToUInt32(rawHeader, 8);
            vh.journalInfoBlock = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 12));

            vh.createDate = FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 16)));
            vh.modifyDate = FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 20)));
            vh.backupDate = FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 24)));
            vh.checkedDate = FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 28)));

            vh.fileCount = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 32));
            vh.folderCount = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 36));

            vh.blockSize = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 40));
            this.blockSize = vh.blockSize;

            vh.totalBlocks = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 44));
            vh.freeBlocks = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 48));

            vh.nextAllocation = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 52));
            vh.rsrcClumpSize = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 56));
            vh.dataClumpSize = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 60));
            vh.nextCatalogID = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 64));
            vh.writeCount = dataOperations.convToLE(BitConverter.ToUInt32(rawHeader, 68));
            vh.encodingsBitmap = BitConverter.ToUInt64(rawHeader, 72);

            vh.finderInfo = new UInt32[8];

            for (int i = 0; i < 8; i++)
            {
                vh.finderInfo[i]= BitConverter.ToUInt32(rawHeader, 80+(4*i));
            }

            hfsPlusForkData volAllocationFile = new hfsPlusForkData(ref rawHeader, 112);
            hfsPlusForkData volExtentsFile = new hfsPlusForkData(ref rawHeader, 192);
            hfsPlusForkData volCatalogFile = new hfsPlusForkData(ref rawHeader, 272);
            hfsPlusForkData volAttributesFile = new hfsPlusForkData(ref rawHeader, 352);
            hfsPlusForkData volStartupFile = new hfsPlusForkData(ref rawHeader, 432);

            vh.allocationFile = volAllocationFile;
            vh.extentsFile = volExtentsFile;
            vh.catalogFile = volCatalogFile;
            vh.attributesFile = volAttributesFile;
            vh.startupFile = volStartupFile;

            vh.partitionNo = this.partitionNo;
            vh.path = this.path;

            return vh;
        }
        public TreeNode getRootDirectoryContents(catalogFile cf, extentsOverflowFile eof, attributesFile af)
        {
            HFSPlusCatalogFolder rootFolderParentRecord = new HFSPlusCatalogFolder();

            rootFolderParentRecord.folderID = 1;
            rootFolderParentRecord.partitionAssoc = this.partitionNo;

            TreeNode rootDirParent = getDirectoryChildren(rootFolderParentRecord, cf, eof);

            HFSPlusCatalogFolder rootFolderRecord = new HFSPlusCatalogFolder();
            rootFolderRecord = (HFSPlusCatalogFolder)rootDirParent.Nodes[0].Tag;
            rootFolderRecord.path = this.volHead.path;

            TreeNode rootDir = getDirectoryChildren(rootFolderRecord, cf, eof, af);

            addMetaFilesToTree(ref rootDir);

            foreach (TreeNode child in rootDir.Nodes)
            {
                if (child.Tag is HFSPlusCatalogFolder)
                {
                    TreeNode tn = getDirectoryChildren((HFSPlusCatalogFolder)child.Tag, cf, eof);
                    int counter = 0;
                    foreach(TreeNode childNode in tn.Nodes)
                    {
                        if (childNode.Tag is HFSPlusCatalogFolder)
                        {
                            counter++;
                        }
                    }

                    if (counter > 0)
                    {
                        // if there are children, add a placeholder
                        child.Nodes.Add("");
                    }
                }
            }

            return rootDir;
        }
        public TreeNode getDirectoryChildren(HFSPlusCatalogFolder folderRecord, catalogFile cf, extentsOverflowFile eof) 
        {
            TreeNode returnDir = new TreeNode();

            // get every file and directory inside the current one
            returnDir = cf.getDirectoryAndChildren(folderRecord, eof, this.partitionNo);

            foreach (TreeNode child in returnDir.Nodes)
            {
                if (child.Tag is HFSPlusCatalogFolder)
                {
                    TreeNode tn = cf.getDirectoryAndChildren((HFSPlusCatalogFolder)child.Tag, eof, this.partitionNo);
                    int counter = 0;
                    foreach (TreeNode childNode in tn.Nodes)
                    {
                        if (childNode.Tag is HFSPlusCatalogFolder)
                        {
                            counter++;
                        }
                    }

                    if (counter > 0)
                    {
                        // if there are children, add a placeholder
                        child.Nodes.Add("");
                    }
                }
            }

            return returnDir;
        }
        public TreeNode getDirectoryChildren(HFSPlusCatalogFolder folderRecord, catalogFile cf, extentsOverflowFile eof, attributesFile af)
        {
            TreeNode returnDir = new TreeNode();

            // get every file and directory inside the current one
            returnDir = cf.getDirectoryAndChildren(folderRecord, eof, this.partitionNo);

            foreach (TreeNode child in returnDir.Nodes)
            {
                // check if there are any alternate data streams for the files
                if (child.Tag is HFSPlusCatalogFile)
                {
                    HFSPlusCatalogFile data = (HFSPlusCatalogFile)child.Tag;

                    attributesFile.HFSPlusAttrKey attrKey = new attributesFile.HFSPlusAttrKey();

                    attrKey.fileID = data.fileID;
                    attrKey.startBlock = 0;
                    attributesLeafNode.attributesDataForFile allAttributes = af.getAttrFileDataWithKey(attrKey);

                    foreach (attributesLeafNode.HFSPlusAttrForkData fork in allAttributes.forks)
                    {
                        TreeNode attribute = new TreeNode();

                        attributesLeafNode.HFSPlusAttrForkData tag = fork;
                        tag.partitionAssoc = folderRecord.partitionAssoc;

                        attribute.Text = child.Text + " > " + System.Text.Encoding.BigEndianUnicode.GetString(fork.key.attrName);
                        attribute.Tag = tag;
                        
                        returnDir.Nodes.Add(attribute);
                    }
                    foreach (attributesLeafNode.HFSPlusAttrInlineData inline in allAttributes.inline)
                    {
                        TreeNode attribute = new TreeNode();

                        attributesLeafNode.HFSPlusAttrInlineData tag = inline;
                        tag.partitionAssoc = folderRecord.partitionAssoc;

                        attribute.Text = child.Text + " > " + System.Text.Encoding.BigEndianUnicode.GetString(inline.key.attrName);
                        attribute.Tag = tag;
                        returnDir.Nodes.Add(attribute);
                    }
                }
            }

            return returnDir;
        }
        public TreeNode getFullDirectoryList()
        {
            TreeNode result = new TreeNode();

            HFSPlusFile rawExtentsOverflow = new HFSPlusFile(volHead.extentsFile, forkStream.forkType.data);
            HFSPlusFile rawCatalog = new HFSPlusFile(volHead.catalogFile, forkStream.forkType.data);
            HFSPlusFile rawAttributesFile = new HFSPlusFile(volHead.attributesFile, forkStream.forkType.data);
            
            volumeStream hfsp_vs = new volumeStream(this);
            catalogFile cf = new catalogFile(rawCatalog, hfsp_vs);
            extentsOverflowFile eof = new extentsOverflowFile(rawExtentsOverflow, hfsp_vs);
            attributesFile af = new attributesFile(rawAttributesFile, hfsp_vs);

            addMetaFilesToTree(ref result);

            result = buildDirectoryTree(result, cf, eof, af);

            if (filecount == volHead.fileCount + 5)
            {
                volumeHeader vh = this.volHead;
                vh.fileCountVerified = true;
                this.volHead = vh;
            }

            if (foldercount == volHead.folderCount)
            {
                volumeHeader vh = this.volHead;
                vh.folderCountVerified = true;
                this.volHead = vh;
            }

            return result;
        }
        public TreeNode getFullDirectoryList(HFSPlusCatalogFolder folderRecord, catalogFile cf, extentsOverflowFile eof, attributesFile af)
        {
            TreeNode returnDir = new TreeNode();
            returnDir.Tag = folderRecord;
            returnDir.Text = System.Text.Encoding.BigEndianUnicode.GetString(folderRecord.key.nodeName);

            returnDir = getDirectoryChildren(folderRecord, cf, eof, af);

            returnDir = buildDirectoryTree(returnDir, cf, eof, af);

            return returnDir;
        }
        public static DateTime FromHFSPlusTime(uint hfsPlusTime)
        {
            var epoch = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(hfsPlusTime);
        }
        private TreeNode buildDirectoryTree(TreeNode parent, catalogFile cf, extentsOverflowFile eof, attributesFile af)
        {
            TreeNode replaceParent = new TreeNode();

            replaceParent.Tag = parent.Tag;
            replaceParent.Text = parent.Text;
            
            foreach (TreeNode childItem in parent.Nodes)
            {
                if(childItem.Tag is HFSPlusCatalogFolder)
                {
                    HFSPlusCatalogFolder childDirectoryRecord = (HFSPlusCatalogFolder)childItem.Tag;

                    TreeNode contents = getDirectoryChildren(childDirectoryRecord, cf, eof, af);

                    contents = buildDirectoryTree(contents, cf, eof, af);

                    replaceParent.Nodes.Add(contents);
                    foldercount++;
                }
                else if (childItem.Tag is HFSPlusCatalogFile)
                {
                    replaceParent.Nodes.Add(childItem);
                    filecount++;
                }
                else if (childItem.Tag is attributesLeafNode.HFSPlusAttrInlineData)
                {
                    replaceParent.Nodes.Add(childItem);
                } 
                else if (childItem.Tag is attributesLeafNode.HFSPlusAttrForkData)
                {
                    replaceParent.Nodes.Add(childItem);
                }
            }

            return replaceParent;
        }
        public dataOperations.hashes hashFileStream(forkStream fs)
        {
            MD5 md5sum = new MD5CryptoServiceProvider();
            SHA1 sha1sum = new SHA1CryptoServiceProvider();

            byte[] md5result;
            byte[] sha1result;

            dataOperations.hashes result = new dataOperations.hashes();

            md5result = md5sum.ComputeHash(fs);

            StringBuilder sbmd5 = new StringBuilder();

            for (int i = 0; i < md5result.Length; i++)
            {
                sbmd5.Append(md5result[i].ToString("X2"));
            }

            result.md5sum = sbmd5.ToString();

            sha1result = sha1sum.ComputeHash(fs);

            StringBuilder sbsha1 = new StringBuilder();

            for (int i = 0; i < md5result.Length; i++)
            {
                sbsha1.Append(md5result[i].ToString("X2"));
            }

            result.sha1sum = sbsha1.ToString();

            fs.Close();

            return result;
        }
        private void addMetaFilesToTree(ref TreeNode result)
        {            TreeNode catalog = new TreeNode();
            TreeNode extents = new TreeNode();
            TreeNode startup = new TreeNode();
            TreeNode attributes = new TreeNode();
            TreeNode allocation = new TreeNode();

            HFSPlusCatalogFile catalogProperties = new HFSPlusCatalogFile();
            HFSPlusCatalogFile extentsProperties = new HFSPlusCatalogFile();
            HFSPlusCatalogFile startupProperties = new HFSPlusCatalogFile();
            HFSPlusCatalogFile attributesProperties = new HFSPlusCatalogFile();
            HFSPlusCatalogFile allocationProperties = new HFSPlusCatalogFile();


            catalog.Text = "$CATALOG";
            catalogProperties.dataFork = this.volHead.catalogFile;
            catalogProperties.accessDate = FromHFSPlusTime(0);
            catalogProperties.attributeModDate = FromHFSPlusTime(0);
            catalogProperties.backupDate = FromHFSPlusTime(0);
            catalogProperties.contentModDate = FromHFSPlusTime(0);
            catalogProperties.createDate = FromHFSPlusTime(0);
            catalogProperties.fileID = 4;
            catalogProperties.path = this.volHead.path + "\\" + catalog.Text;
            catalog.Tag = catalogProperties;

            extents.Text = "$EXTENTSOVERFLOW";
            extentsProperties.dataFork = this.volHead.extentsFile;
            extentsProperties.accessDate = FromHFSPlusTime(0);
            extentsProperties.attributeModDate = FromHFSPlusTime(0);
            extentsProperties.backupDate = FromHFSPlusTime(0);
            extentsProperties.contentModDate = FromHFSPlusTime(0);
            extentsProperties.createDate = FromHFSPlusTime(0);
            extentsProperties.fileID = 3;
            extentsProperties.path = this.volHead.path + "\\" + extents.Text;
            extents.Tag = extentsProperties;

            startup.Text = "$STARTUP";
            startupProperties.dataFork = this.volHead.startupFile;
            startupProperties.accessDate = FromHFSPlusTime(0);
            startupProperties.attributeModDate = FromHFSPlusTime(0);
            startupProperties.backupDate = FromHFSPlusTime(0);
            startupProperties.contentModDate = FromHFSPlusTime(0);
            startupProperties.createDate = FromHFSPlusTime(0);
            startupProperties.fileID = 7;
            startupProperties.path = this.volHead.path + "\\" + startup.Text;
            startup.Tag = startupProperties;

            attributes.Text = "$ATTRIBUTES";
            attributesProperties.dataFork = this.volHead.attributesFile;
            catalogProperties.accessDate = FromHFSPlusTime(0);
            catalogProperties.attributeModDate = FromHFSPlusTime(0);
            catalogProperties.backupDate = FromHFSPlusTime(0);
            catalogProperties.contentModDate = FromHFSPlusTime(0);
            catalogProperties.createDate = FromHFSPlusTime(0);
            attributesProperties.fileID = 8;
            attributesProperties.path = this.volHead.path + "\\" + attributes.Text;
            attributes.Tag = attributesProperties;

            allocation.Text = "$ALLOCATION";
            allocationProperties.dataFork = this.volHead.allocationFile;
            allocationProperties.accessDate = FromHFSPlusTime(0);
            allocationProperties.attributeModDate = FromHFSPlusTime(0);
            allocationProperties.backupDate = FromHFSPlusTime(0);
            allocationProperties.contentModDate = FromHFSPlusTime(0);
            allocationProperties.createDate = FromHFSPlusTime(0);
            allocationProperties.fileID = 6;
            allocationProperties.path = this.volHead.path + "\\" + allocation.Text;
            allocation.Tag = allocationProperties;
            
            result.Nodes.Add(catalog);
            result.Nodes.Add(extents);
            result.Nodes.Add(startup);
            result.Nodes.Add(attributes);
            result.Nodes.Add(allocation);
        }
    }
}
