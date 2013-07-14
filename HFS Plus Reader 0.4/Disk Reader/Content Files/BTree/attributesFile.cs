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
using System.IO;

namespace Disk_Reader
{
    class attributesFile : absHFSPlusBTree
    {
        public struct HFSPlusAttrKey
        {
            private ushort keyLengthVal;
            private ushort padVal;
            private uint fileIDVal;
            private uint startBlockVal;
            private byte[] attrNameVal;

            public ushort keyLength { get { return keyLengthVal; } set { keyLengthVal = value; } }
            public ushort pad { get { return padVal; } set { padVal = value; } }
            public uint fileID { get { return fileIDVal; } set { fileIDVal = value; } }
            public uint startBlock { get { return startBlockVal; } set { startBlockVal = value; } }
            public byte[] attrName
            {
                get { return attrNameVal; }
                set
                {
                    if (value.Length <= 255) { attrNameVal = value; }
                    else { throw new Exception("Node Name too long"); }
                }
            }
        }

        public attributesFile(HFSPlusFile knownExtents, volumeStream vs) : base(knownExtents, vs)
        {
        }

        private uint getLeafNodeContainingRecord(HFSPlusAttrKey recordKeyID)
        {
            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            // why is currentNodeType showing as 255 when it should be -1?
            while (currentNodeType != absNode.nodeType.leaf)
            {
                fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);

                attributesIndexNode currentNode = new attributesIndexNode(ref nodeRawData);
                attributesIndexNode.attrIndexRecord perhapsThisRecord = currentNode.records[0];

                foreach (attributesIndexNode.attrIndexRecord record in currentNode.records)
                {
                    bool equalsSearch = attrKeyCompare(record.attrKey, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.equalsTrialKey;
                    bool lessThanSearch = attrKeyCompare(record.attrKey, recordKeyID, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.greaterThanTrialKey;
                    bool greaterThanOrEqualToBestKnown = attrKeyCompare(record.attrKey, perhapsThisRecord.attrKey, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.lessThanTrialKey
                        || attrKeyCompare(record.attrKey, perhapsThisRecord.attrKey, this.header.headerInfo.keyCompareType == 0xBC) == dataOperations.keyCompareResult.equalsTrialKey;


                    if (equalsSearch)
                    {
                        return record.pointer;
                    }
                    else if (lessThanSearch && greaterThanOrEqualToBestKnown)
                    {
                        perhapsThisRecord = record;
                    }
                }

                currentNodeNumber = perhapsThisRecord.pointer;
                currentNodeType = (absNode.nodeType)getNodeType(currentNodeNumber);
            }

            return currentNodeNumber;
        }
        public static HFSPlusAttrKey buildAttrTrialKey(absIndexOrLeafNode.rawKeyAndRecord record)
        {
            HFSPlusAttrKey result = new HFSPlusAttrKey();

            result.fileID = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 2));
            result.startBlock = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 6));

            result.attrName = new byte[record.keyLength - 12];
            Array.Copy(record.keyData, 12, result.attrName, 0, record.keyLength - 12);
            result.pad = dataOperations.convToLE(BitConverter.ToUInt16(record.keyData, 0));
            result.keyLength = record.keyLength;

            return result;
        }
        public attributesLeafNode.attributesDataForFile getAttrFileDataWithKey(HFSPlusAttrKey recordKeyID, bool caseSensitiveCompare = false)
        {
            byte[] nodeRawData = new byte[this.nodeSize];
            attributesLeafNode.attributesDataForFile allAttributes = new attributesLeafNode.attributesDataForFile();
            allAttributes.inline = new List<attributesLeafNode.HFSPlusAttrInlineData>();
            allAttributes.forks = new List<attributesLeafNode.HFSPlusAttrForkData>();

            // find the root node
            uint currentNodeNumber = this.header.headerInfo.rootNode;
            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);
            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            // find the leaf where records matching this key start
            if(this.header.headerInfo.rootNode > 0)
            {
                uint leafRecordNumber = getLeafNodeContainingRecord(recordKeyID);
                fs.Seek(leafRecordNumber * this.nodeSize, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);
                attributesLeafNode leafNode = new attributesLeafNode(ref nodeRawData);


                bool allDataFound = false;
                while (!allDataFound)
                {
                    int recordsAdded = 0;
                    foreach (attributesLeafNode.HFSPlusAttrForkData leafRecord in leafNode.forkDataRecords)
                    {
                        if (dataOperations.keyCompareResult.equalsTrialKey == attrKeyCompare(leafRecord.key, recordKeyID, caseSensitiveCompare))
                        {
                            allAttributes.forks.Add(leafRecord);
                            recordsAdded++;
                        }
                    }
                    foreach (attributesLeafNode.HFSPlusAttrExtents leafRecord in leafNode.extentsRecords)
                    {
                        if (dataOperations.keyCompareResult.equalsTrialKey == attrKeyCompare(leafRecord.key, recordKeyID, caseSensitiveCompare))
                        {
                            // find out which forkdata record to add the extent to
                            foreach (attributesLeafNode.HFSPlusAttrForkData fork in allAttributes.forks)
                            {
                                if (fork.key.fileID == leafRecord.key.fileID && fork.key.attrName == leafRecord.key.attrName)
                                {
                                    // then add all the extents in the record
                                    for (int i = 0; i < leafRecord.extents.Count(); i++)
                                    {
                                        fork.theFork.forkDataValues.extents.Add(leafRecord.extents[i]);
                                    }
                                }
                            }
                            recordsAdded++;
                        }
                    }
                    foreach (attributesLeafNode.HFSPlusAttrInlineData leafRecord in leafNode.inlineRecords)
                    {
                        if (dataOperations.keyCompareResult.equalsTrialKey == attrKeyCompare(leafRecord.key, recordKeyID, caseSensitiveCompare))
                        {
                            allAttributes.inline.Add(leafRecord);
                            recordsAdded++;
                        }
                    }

                    // if the last record in the node matches the search key, there may be more matching records in the next node
                    // if the node is the last leaf node, its flink will be 0
                    if (attrKeyCompare(buildAttrTrialKey(leafNode.rawRecords[leafNode.rawRecords.Count() - 1]), recordKeyID, caseSensitiveCompare) == dataOperations.keyCompareResult.equalsTrialKey
                        && leafNode.BTNodeDescriptor.fLink > 0)
                    {
                        uint nextNode = leafNode.BTNodeDescriptor.fLink;

                        fs.Seek(nextNode * this.nodeSize, SeekOrigin.Begin);
                        fs.Read(nodeRawData, 0, this.nodeSize);

                        leafNode = new attributesLeafNode(ref nodeRawData);
                    }
                    else
                    {
                        allDataFound = true;
                    }

                    //if (!allDataFound && recordsAdded == 0)
                    //{
                    //    throw new Exception("The specified search key was not found.");
                    //}
                }
            }

            return allAttributes;
        }
        public attributesLeafNode.HFSPlusAttrInlineData getAttriInlineDataWithKey(HFSPlusAttrKey recordKeyID, bool caseSensitiveCompare = false)
        {
            byte[] nodeRawData = new byte[this.nodeSize];

            attributesLeafNode.HFSPlusAttrInlineData result = new attributesLeafNode.HFSPlusAttrInlineData();

            // find the root node
            uint currentNodeNumber = this.header.headerInfo.rootNode;
            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);
            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            bool found = false;

            // get the leaf
            if (this.header.headerInfo.rootNode > 0)
            {
                uint leafRecordNumber = getLeafNodeContainingRecord(recordKeyID);
                fs.Seek(leafRecordNumber * this.nodeSize, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);
                attributesLeafNode leafNode = new attributesLeafNode(ref nodeRawData);

                foreach (attributesLeafNode.HFSPlusAttrInlineData leafRecord in leafNode.inlineRecords)
                {
                    if (dataOperations.keyCompareResult.equalsTrialKey == attrKeyCompare(leafRecord.key, recordKeyID, caseSensitiveCompare) && found == false)
                    {
                        result = leafRecord;
                        found = true;
                    }
                }
            }

            if (found)
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        public attributesLeafNode.HFSPlusAttrForkData getAttrForkContentWithKey(HFSPlusAttrKey recordKeyID, bool caseSensitiveCompare = false)
        {
            throw new NotImplementedException();
        }
        public dataOperations.keyCompareResult attrKeyCompare(HFSPlusAttrKey trialKey, HFSPlusAttrKey searchKey, bool caseSensitive)
        {
            if (searchKey.fileID > trialKey.fileID)
            {
                return dataOperations.keyCompareResult.greaterThanTrialKey;
            }
            else if (searchKey.fileID < trialKey.fileID)
            {
                return dataOperations.keyCompareResult.lessThanTrialKey;
            }
            else
            {
                dataOperations.keyCompareResult result = dataOperations.keyCompareResult.equalsTrialKey;

                if (searchKey.attrName == null)
                {
                    // this will match all keys relating to a given fileID
                    return dataOperations.keyCompareResult.equalsTrialKey;
                }
                else if (caseSensitive)
                {
                    // case sensitive names are compared on a per-byte basis
                    for (int i = 0; i < searchKey.attrName.Length; i++)
                    {
                        if (searchKey.attrName[i] > trialKey.attrName[i])
                        {
                            result = dataOperations.keyCompareResult.greaterThanTrialKey;
                            break;
                        }
                        else if (searchKey.attrName[i] < trialKey.attrName[i])
                        {
                            result = dataOperations.keyCompareResult.lessThanTrialKey;
                            break;
                        }
                    }
                    return result;
                }
                else
                {
                    int num = string.CompareOrdinal(Encoding.Unicode.GetString(searchKey.attrName),
                        Encoding.Unicode.GetString(trialKey.attrName));
                    switch ((dataOperations.keyCompareResult)num)
                    {
                        case dataOperations.keyCompareResult.equalsTrialKey:
                            // if it is for the same attribute, the startblock is the final determinant
                            if (searchKey.startBlock > trialKey.startBlock)
                            {
                                result = dataOperations.keyCompareResult.greaterThanTrialKey;
                            }
                            else if (searchKey.startBlock < trialKey.startBlock)
                            {
                                result = dataOperations.keyCompareResult.lessThanTrialKey;
                            }
                            else
                            {
                                result = dataOperations.keyCompareResult.equalsTrialKey;
                            }
                            break;
                        case dataOperations.keyCompareResult.greaterThanTrialKey:
                            result = dataOperations.keyCompareResult.greaterThanTrialKey;
                            break;
                        case dataOperations.keyCompareResult.lessThanTrialKey:
                            result = dataOperations.keyCompareResult.lessThanTrialKey;
                            break;
                    }

                    return result;
                }
            }

        }
    }
}
