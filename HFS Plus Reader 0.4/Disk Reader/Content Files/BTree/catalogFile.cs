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
using System.IO;

namespace Disk_Reader
{
    class catalogFile : absHFSPlusBTree
    {
        public struct HFSPlusCatalogKey
        {
            private ushort keyLengthVal;
            private uint parentIDVal;
            private byte[] nodeNameVal;

            public ushort keyLength { get { return keyLengthVal; } set { keyLengthVal = value; } }
            public uint parentID { get { return parentIDVal; } set { parentIDVal = value; } }
            // nodeName is UniStr255 - 255 unicode characters, 510 bytes max
            public byte[] nodeName { 
                get { return nodeNameVal; } 
                set {   if (value.Length <= 510) { nodeNameVal = value; } 
                        else { throw new Exception("Node Name too long"); } } }
        }

        public catalogFile(HFSPlusFile knownExtents, volumeStream vs) : base(knownExtents, vs)
        {
            
        }

        private uint getLeafNodeContainingRecord(HFSPlusCatalogKey recordKeyID)
        {
            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            while (currentNodeType != absNode.nodeType.leaf)
            {
                fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);

                catalogIndexNode currentNode = new catalogIndexNode(ref nodeRawData);
                catalogIndexNode.catalogIndexRecord perhapsThisRecord = currentNode.records[0];

                // find out which pointer to follow
                foreach (catalogIndexNode.catalogIndexRecord record in currentNode.records)
                {
                    bool equalsSearch = catalogKeyCompare(record.catalogKey, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.equalsTrialKey;
                    bool lessThanSearch = catalogKeyCompare(record.catalogKey, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.greaterThanTrialKey;
                    bool greaterThanOrEqualToBestKnown = catalogKeyCompare(record.catalogKey, perhapsThisRecord.catalogKey, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.lessThanTrialKey
                        || catalogKeyCompare(record.catalogKey, perhapsThisRecord.catalogKey, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.equalsTrialKey;


                    if (lessThanSearch && greaterThanOrEqualToBestKnown || equalsSearch)
                    {
                        perhapsThisRecord = record;
                    }
                }

                currentNodeNumber = perhapsThisRecord.pointer;
                currentNodeType = (absNode.nodeType)getNodeType(currentNodeNumber);
            }

            // send back pointer
            return currentNodeNumber;
        }
        public HFSPlusCatalogFile getCatalogFileWithKey(HFSPlusCatalogKey recordKeyID)
        {
            HFSPlusCatalogFile result;

            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            uint leafNodeNumber = getLeafNodeContainingRecord(recordKeyID);


            fs.Seek(leafNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            catalogLeafNode leafNode = new catalogLeafNode(ref nodeRawData);

            foreach (HFSPlusCatalogFile leafRecord in leafNode.fileRecords)
            {
                if (dataOperations.keyCompareResult.equalsTrialKey == catalogKeyCompare(leafRecord.key, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC))
                {
                    result = leafRecord;
                    return result;
                }
            }

            throw new Exception("The specified search key was not found.");
        }
        public HFSPlusCatalogFolder getCatalogFolderWithKey(HFSPlusCatalogKey recordKeyID)
        {
            HFSPlusCatalogFolder result;

            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            uint leafNodeNumber = getLeafNodeContainingRecord(recordKeyID);

            fs.Seek(leafNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            catalogLeafNode leafNode = new catalogLeafNode(ref nodeRawData);

            foreach (HFSPlusCatalogFolder leafRecord in leafNode.folderRecords)
            {
                if (dataOperations.keyCompareResult.equalsTrialKey == catalogKeyCompare(leafRecord.key, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC))
                {
                    result = leafRecord;
                    return result;
                }
            }

            throw new Exception("The specified search key was not found.");
        }
        public catalogLeafNode.HFSPlusCatalogThread getCatalogThreadWithKey(HFSPlusCatalogKey recordKeyID, bool caseSensitiveCompare = false)
        {
            catalogLeafNode.HFSPlusCatalogThread result;

            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            uint leafRecordNumber = getLeafNodeContainingRecord(recordKeyID);

            fs.Seek(leafRecordNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            catalogLeafNode leafNode = new catalogLeafNode(ref nodeRawData);

            foreach (catalogLeafNode.HFSPlusCatalogThread leafRecord in leafNode.threadRecords)
            {
                if (dataOperations.keyCompareResult.equalsTrialKey == catalogKeyCompare(leafRecord.key, recordKeyID, caseSensitiveCompare))
                {
                    result = leafRecord;
                    return result;
                }
            }

            throw new Exception("The specified search key was not found.");
        }
        public static catalogFile.HFSPlusCatalogKey buildCatalogTrialKey(absIndexOrLeafNode.rawKeyAndRecord record)
        {
            catalogFile.HFSPlusCatalogKey result = new catalogFile.HFSPlusCatalogKey();

            result.parentID = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 0));

            result.nodeName = new byte[record.keyLength - 4];
            Array.Copy(record.keyData, 4, result.nodeName, 0, record.keyLength - 4);

            result.keyLength = record.keyLength;

            return result;
        }
        public dataOperations.keyCompareResult catalogKeyCompare(HFSPlusCatalogKey trialKey, HFSPlusCatalogKey searchKey, bool caseSensitive)
        {
            // catalog keys are compared by parent ID first - therefore all children 
            // of a given directory will be grouped together. This is followed by a 
            // comparison of the nodename, which is case-sensitive on some volumes
            if (searchKey.parentID > trialKey.parentID)
            {
                return dataOperations.keyCompareResult.greaterThanTrialKey;
            }
            else if (searchKey.parentID < trialKey.parentID)
            {
                return dataOperations.keyCompareResult.lessThanTrialKey;
            }
            else
            {
                if (searchKey.nodeName == null)
                {
                    return dataOperations.keyCompareResult.greaterThanTrialKey;
                }
                else if (caseSensitive)
                {
                    dataOperations.keyCompareResult result = dataOperations.keyCompareResult.equalsTrialKey;
                    for (int i = 0; i < searchKey.nodeName.Length; i++)
                    {
                        if (searchKey.nodeName[i] > trialKey.nodeName[i])
                        {
                            result = dataOperations.keyCompareResult.greaterThanTrialKey;
                            break;
                        }
                        else if (searchKey.nodeName[i] < trialKey.nodeName[i])
                        {
                            result = dataOperations.keyCompareResult.lessThanTrialKey;
                            break;
                        }
                    }
                    return result;
                } 
                else 
                {
                     return (dataOperations.keyCompareResult)string.CompareOrdinal(Encoding.Unicode.GetString(searchKey.nodeName), 
                         Encoding.Unicode.GetString(trialKey.nodeName));
                }
            }

        }
        public TreeNode getDirectoryAndChildren(HFSPlusCatalogFolder folderRecord, extentsOverflowFile eof, int partitionAssoc)
        {
            TreeNode returnDir = new TreeNode();
            if (folderRecord.key.nodeName != null)
            {
                returnDir.Text = System.Text.Encoding.BigEndianUnicode.GetString(folderRecord.key.nodeName);
                returnDir.Text = returnDir.Text.Replace('\0', ' ');
            }
            folderRecord.partitionAssoc = partitionAssoc;
            returnDir.Tag = folderRecord;

            HFSPlusCatalogKey matchParentDir = new HFSPlusCatalogKey();

            // find the first HFSPlusFileRecord for whom the current directory is the parent
            matchParentDir.parentID = folderRecord.folderID;
            if(folderRecord.key.nodeName != null) matchParentDir.nodeName = folderRecord.key.nodeName;
            uint readThisNode = getLeafNodeContainingRecord(matchParentDir);
            bool nextLeaf = true;

            // records with the same parent are stored sequentially in the file, 
            // but may continue over into the next node
            while (nextLeaf)
            {
                byte[] leafData = new byte[this.nodeSize];

                fs.Seek(readThisNode * this.nodeSize, SeekOrigin.Begin);
                fs.Read(leafData, 0, this.nodeSize);

                catalogLeafNode currentLeaf = new catalogLeafNode(ref leafData);

                foreach (HFSPlusCatalogFolder folder in currentLeaf.folderRecords)
                {
                    if (folder.key.parentID == folderRecord.folderID)
                    {
                        TreeNode childDir = new TreeNode();
                        if (folder.key.nodeName != null)
                        {
                            childDir.Text = System.Text.Encoding.BigEndianUnicode.GetString(folder.key.nodeName);
                            childDir.Text = childDir.Text.Replace('\0', ' ');
                        }

                        // set the treenode data for the child item
                        folder.path = folderRecord.path + "\\" + childDir.Text;
                        folder.partitionAssoc = partitionAssoc;

                        childDir.Tag = folder;
                        returnDir.Nodes.Add(childDir);
                    }
                }

                foreach (HFSPlusCatalogFile file in currentLeaf.fileRecords)
                {
                    if (file.key.parentID == folderRecord.folderID)
                    {
                        TreeNode childFile = new TreeNode();

                        HFSPlusCatalogFile eachFile = file;
                        eachFile.partitionAssoc = partitionAssoc;

                        // HFSPlusFile should be able to get all of a file's blocks as part of the constructor
                        HFSPlusFile blockFinder = new HFSPlusFile(eachFile, eof);

                        //add the discovered extents back into the return object
                        //eachFile.dataFork.forkDataValues.extents.Clear();
                        //eachFile.resourceFork.forkDataValues.extents.Clear();
                        //foreach (hfsPlusForkData.HFSPlusExtentRecord extent in blockFinder.fileContent.dataExtents)
                        //{
                        //    eachFile.dataFork.forkDataValues.extents.Add(extent);
                        //}
                        //foreach (hfsPlusForkData.HFSPlusExtentRecord extent in blockFinder.fileContent.resourceExtents)
                        //{
                        //    eachFile.resourceFork.forkDataValues.extents.Add(extent);
                        //}

                        // if it can't... cry?
                        if (!(blockFinder.allDataBlocksKnown && blockFinder.allResourceBlocksKnown))
                        {
                            throw new Exception("Disk_Reader.HFSPlusFile class failed to get all blocks.");
                        }

                        // a handful of volume metadata files have highly specialised permissions
                        HFSPlusCatalogFolder tag = (HFSPlusCatalogFolder)returnDir.Tag;
                        if (tag.key.parentID == 2)
                        {
                            if (returnDir.Text == "    HFS+ Private Data")
                            {
                                HFSPlusCatalogRecord.HFSPlusPermissions resetPermissions = new HFSPlusCatalogRecord.HFSPlusPermissions();
                                resetPermissions = eachFile.permissions;
                                resetPermissions.type = HFSPlusCatalogRecord.HFSPlusPermissions.specialType.iNodeNum;

                                eachFile.permissions = resetPermissions;
                            }
                        }
                        else if (eachFile.userInfo.fileType == 0x686C6E6B && eachFile.userInfo.fileCreator == 0x6866732B)
                        {
                            HFSPlusCatalogRecord.HFSPlusPermissions resetPermissions = new HFSPlusCatalogRecord.HFSPlusPermissions();
                            resetPermissions = eachFile.permissions;
                            resetPermissions.type = HFSPlusCatalogRecord.HFSPlusPermissions.specialType.linkCount;

                            eachFile.permissions = resetPermissions;
                        }
                        else if (eachFile.permissions.fileMode.blockSpecial || eachFile.permissions.fileMode.charSpecial)
                        {
                            HFSPlusCatalogRecord.HFSPlusPermissions resetPermissions = new HFSPlusCatalogRecord.HFSPlusPermissions();
                            resetPermissions = eachFile.permissions;
                            resetPermissions.type = HFSPlusCatalogRecord.HFSPlusPermissions.specialType.rawDevice;

                            eachFile.permissions = resetPermissions;
                        }
                        else
                        {
                            HFSPlusCatalogRecord.HFSPlusPermissions resetPermissions = new HFSPlusCatalogRecord.HFSPlusPermissions();
                            resetPermissions = eachFile.permissions;
                            resetPermissions.type = HFSPlusCatalogRecord.HFSPlusPermissions.specialType.reserved;

                            eachFile.permissions = resetPermissions;
                        }

                        childFile.Text = System.Text.Encoding.BigEndianUnicode.GetString(file.key.nodeName);
                        if (folderRecord.key.nodeName != null)
                        {
                            childFile.Text = System.Text.Encoding.BigEndianUnicode.GetString(file.key.nodeName);
                            childFile.Text = childFile.Text.Replace('\0', ' ');
                        }

                        // set the treenode data for the child item
                        eachFile.path = folderRecord.path + "\\" + childFile.Text;
                        
                        childFile.Tag = eachFile;

                        returnDir.Nodes.Add(childFile);
                    }
                }

                bool lastRecordMatchesKey =
                    matchParentDir.parentID == dataOperations.convToLE(
                        BitConverter.ToUInt32(
                            currentLeaf.rawRecords[currentLeaf.rawRecords.Count() - 1].keyData, 0));

                // if the last record in the current leaf is within the parent directory, 
                // the records may continue in the next leaf, so skip to the node in flink
                // in the next instance of the loop
                if (returnDir.Nodes.Count < folderRecord.valence)
                {
                    readThisNode = currentLeaf.BTNodeDescriptor.fLink;
                }
                else
                {
                    nextLeaf = false;
                }
            }

            return returnDir;
        }
        private catalogLeafNode getNextLeaf(uint flink)
        {
            byte[] nextLeafData = new byte[this.nodeSize];

            fs.Seek(flink * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nextLeafData, 0, this.nodeSize);

            catalogLeafNode result = new catalogLeafNode(ref nextLeafData);

            return result;
        }
        private catalogLeafNode getPreviousLeaf(uint blink)
        {
            byte[] previousLeafData = new byte[this.nodeSize];

            fs.Seek(blink * this.nodeSize, SeekOrigin.Begin);
            fs.Read(previousLeafData, 0, this.nodeSize);

            catalogLeafNode result = new catalogLeafNode(ref previousLeafData);

            return result;
        }
    }
}
