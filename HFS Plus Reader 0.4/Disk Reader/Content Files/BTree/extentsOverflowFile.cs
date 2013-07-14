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
    class extentsOverflowFile : absHFSPlusBTree
    {
        public struct HFSPlusExtentKey
        {
            public ushort keyLength;
            public forkType type;
            public byte pad;
            public uint fileID;
            public uint startBlock;
        }

        public enum forkType 
        { 
            data = 0x00,
            resource = 0xFF
        }


        public extentsOverflowFile(HFSPlusFile knownExtents, volumeStream vs) : base(knownExtents, vs)
        {
            
        }

        public extentsOverflowLeafNode.extentsOverflowLeafRecord getExtentRecordWithKey(HFSPlusExtentKey recordKeyID)
        {
            extentsOverflowLeafNode.extentsOverflowLeafRecord result;
            
            byte[] nodeRawData = new byte[this.nodeSize];
            uint currentNodeNumber = this.header.headerInfo.rootNode;

            absNode.nodeType currentNodeType = getNodeType(currentNodeNumber);

            // until a leaf node is found
            while (currentNodeType != absNode.nodeType.leaf)
            {
                fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);
                
                extentsOverflowIndexNode currentNode = new extentsOverflowIndexNode(ref nodeRawData);
                extentsOverflowIndexNode.extentsOverflowIndexRecord perhapsThisRecord = currentNode.records[0];

                // finds the subtree that contains the desired record and follows it
                foreach (extentsOverflowIndexNode.extentsOverflowIndexRecord record in currentNode.records)
                {
                    bool equalsSearch = extentsOverflowKeyCompare(record.extentKey, recordKeyID) == dataOperations.keyCompareResult.equalsTrialKey;
                    bool lessThanSearch = extentsOverflowKeyCompare(record.extentKey, recordKeyID) == dataOperations.keyCompareResult.greaterThanTrialKey;
                    bool greaterThanOrEqualToBestKnown = extentsOverflowKeyCompare(record.extentKey, perhapsThisRecord.extentKey) == dataOperations.keyCompareResult.lessThanTrialKey
                        || extentsOverflowKeyCompare(record.extentKey, perhapsThisRecord.extentKey) == dataOperations.keyCompareResult.equalsTrialKey;


                    if (lessThanSearch && greaterThanOrEqualToBestKnown || equalsSearch)
                    {
                        perhapsThisRecord = record;
                    }
                }

                currentNodeNumber = perhapsThisRecord.pointer;
                currentNodeType = (absNode.nodeType)getNodeType(currentNodeNumber);
            }

            // once the leaf node is found, compile the data from it and return that node
            fs.Seek(currentNodeNumber * this.nodeSize, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            extentsOverflowLeafNode leafNode = new extentsOverflowLeafNode(ref nodeRawData);

            foreach (extentsOverflowLeafNode.extentsOverflowLeafRecord leafRecord in leafNode.records)
            {
                if (dataOperations.keyCompareResult.equalsTrialKey == extentsOverflowKeyCompare(leafRecord.key, recordKeyID))
                {
                    result = leafRecord;
                    return result;
                }
            }

            throw new Exception("The specified search key was not found.");
        }
        public dataOperations.keyCompareResult extentsOverflowKeyCompare(HFSPlusExtentKey trialKey, HFSPlusExtentKey searchKey)
        {
            // extents overflow keys are compared in the order fileID, extent type, start block
            dataOperations.keyCompareResult result;

            if (searchKey.fileID > trialKey.fileID)
            {
                result = dataOperations.keyCompareResult.greaterThanTrialKey;
            }
            else if (searchKey.fileID < trialKey.fileID)
            {
                result = dataOperations.keyCompareResult.lessThanTrialKey;
            }
            else
            {
                if (searchKey.type > trialKey.type)
                {
                    result = dataOperations.keyCompareResult.greaterThanTrialKey;
                }
                else if (searchKey.type < trialKey.type)
                {
                    result = dataOperations.keyCompareResult.lessThanTrialKey;
                }
                else
                {
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
                }
            }

            return result;
        }
        public List<hfsPlusForkData.HFSPlusExtentRecord> getSelf(uint totalBlocks, uint knownBlocks)
        {
            // get any additional extents not defined in the volume header of the extents overflow file

            List<hfsPlusForkData.HFSPlusExtentRecord> result = new List<hfsPlusForkData.HFSPlusExtentRecord>();
            uint getNode = this.header.headerInfo.firstLeafNode;
            byte[] nodeRawData = new byte[this.nodeSize];

            fs.Seek(this.nodeSize * getNode, SeekOrigin.Begin);
            fs.Read(nodeRawData, 0, this.nodeSize);

            HFSPlusExtentKey theKey = new HFSPlusExtentKey();
            theKey.fileID = 3;
            theKey.startBlock = knownBlocks;
            theKey.type = forkType.data;

            // do this until all of the extents have been found
            while (knownBlocks < totalBlocks)
            {
                extentsOverflowLeafNode leaf = new extentsOverflowLeafNode(ref nodeRawData);

                foreach (extentsOverflowLeafNode.extentsOverflowLeafRecord record in leaf.records)
                {
                    dataOperations.keyCompareResult correctRecord = extentsOverflowKeyCompare(record.key, theKey);
                    if (correctRecord == dataOperations.keyCompareResult.equalsTrialKey)
                    {
                        // add all of the extents that contain any blocks
                        foreach (hfsPlusForkData.HFSPlusExtentRecord extent in record.extents)
                        {
                            if (extent.blockCount > 0)
                            {
                                result.Add(extent);
                                knownBlocks += extent.blockCount;
                            }
                        }
                    }
                }

                // prepare the next node's data in case extents go into following node
                getNode = leaf.BTNodeDescriptor.fLink;
                fs.Seek(this.nodeSize * getNode, SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);
            }

            return result;
        }
        public static extentsOverflowFile.HFSPlusExtentKey buildExtentSearchKey(ref byte[] rawKeyData)
        {
            extentsOverflowFile.HFSPlusExtentKey result = new extentsOverflowFile.HFSPlusExtentKey();

            result.type = (extentsOverflowFile.forkType)rawKeyData[0];
            result.pad = rawKeyData[1];
            result.fileID = dataOperations.convToLE(BitConverter.ToUInt32(rawKeyData, 2));
            result.startBlock = dataOperations.convToLE(BitConverter.ToUInt32(rawKeyData, 6));

            return result;
        }
    }
}
