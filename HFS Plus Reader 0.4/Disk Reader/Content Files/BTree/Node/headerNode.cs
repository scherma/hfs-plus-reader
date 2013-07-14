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

namespace Disk_Reader
{
    class headerNode : absNode
    {
        public struct headerRecord
        {
            private ushort treeDepthVal;
            private uint rootNodeVal;
            private uint leafRecordsVal;
            private uint firstLeafNodeVal;
            private uint lastLeafNodeVal;
            private ushort nodeSizeVal;
            private ushort maxKeyLengthVal;
            private uint totalNodesVal;
            private uint freeNodesVal;
            private ushort reserved1Val;
            private uint clumpSizeVal;      // misaligned
            private byte btreeTypeVal;
            private byte keyCompareTypeVal;
            private uint attributesVal;     // long aligned again
            private byte[] reserved3Val; // must be 16 in this array

            public ushort treeDepth { get { return treeDepthVal; } set { treeDepthVal = value; } }
            public uint rootNode { get { return rootNodeVal; } set { rootNodeVal = value; } }
            public uint leafRecords { get { return leafRecordsVal; } set { leafRecordsVal = value; } }
            public uint firstLeafNode { get { return firstLeafNodeVal; } set { firstLeafNodeVal = value; } }
            public uint lastLeafNode { get { return lastLeafNodeVal; } set { lastLeafNodeVal = value; } }
            public ushort nodeSize { get { return nodeSizeVal; } set { nodeSizeVal = value; } }
            public ushort maxKeyLength { get { return maxKeyLengthVal; } set { maxKeyLengthVal = value; } }
            public uint totalNodes { get { return totalNodesVal; } set { totalNodesVal = value; } }
            public uint freeNodes { get { return freeNodesVal; } set { freeNodesVal = value; } }
            public ushort reserved1 { get { return reserved1Val; } set { reserved1Val = value; } }
            public uint clumpSize { get { return clumpSizeVal; } set { clumpSizeVal = value; } }
            public byte btreeType { get { return btreeTypeVal; } set { btreeTypeVal = value; } }
            public byte keyCompareType { get { return keyCompareTypeVal; } set { keyCompareTypeVal = value; } }
            public uint attributes { get { return attributesVal; } set { attributesVal = value; } }
            public byte[] reserved3 { get { return reserved3Val; } set { reserved3Val = new byte[64]; reserved3Val = value; } }

        }
        public enum headerAttributes
        {
            kBTBadCloseMask = 0x00000001,
            kBTBigKeysMask = 0x00000002,
            kBTVariableIndexKeysMask = 0x00000004
        }
        public struct userDataRecord
        {
            private byte[] userDataVal;

            public byte[] userData { get { return userDataVal; } set { userDataVal = new byte[128]; userDataVal = value; } }
        }
        public struct mapRecord
        {
            private byte[] bitmapComponentVal;

            public byte[] bitmapComponent { get { return bitmapComponentVal; } set { bitmapComponentVal = value; } }
        }

        public headerRecord headerInfo;
        public userDataRecord userData;
        public mapRecord map;

        public headerNode(ref byte[] nodeRawData) : base (ref nodeRawData)
        {
            getHeaderRecord(this.nodeData);
            getUserDataRecord(this.nodeData);
            getMapRecord(this.nodeData);
        }

        private void getHeaderRecord(byte[] nodeRawData)
        {
            byte[] headerData = getRecordData(0);
            
            this.headerInfo.treeDepth = dataOperations.convToLE(BitConverter.ToUInt16(headerData, 0));
            this.headerInfo.rootNode = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 2));
            this.headerInfo.leafRecords = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 6));
            this.headerInfo.firstLeafNode = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 10));
            this.headerInfo.lastLeafNode = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 14));
            this.headerInfo.nodeSize = dataOperations.convToLE(BitConverter.ToUInt16(headerData, 16));
            this.headerInfo.maxKeyLength = dataOperations.convToLE(BitConverter.ToUInt16(headerData, 18));
            this.headerInfo.totalNodes = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 22));
            this.headerInfo.freeNodes = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 26));
            this.headerInfo.reserved1 = dataOperations.convToLE(BitConverter.ToUInt16(headerData, 30));
            this.headerInfo.clumpSize = dataOperations.convToLE(BitConverter.ToUInt32(headerData, 32));
            this.headerInfo.btreeType = headerData[36];
            this.headerInfo.keyCompareType = headerData[37];
            this.headerInfo.attributes = BitConverter.ToUInt32(headerData, 38);

            headerInfo.reserved3 = new byte[64];
            Array.Copy(headerData, 42, headerInfo.reserved3, 0, 64);
        }
        private void getMapRecord(byte[] nodeRawData)
        {
            this.map.bitmapComponent = getRecordData(1);
        }
        private void getUserDataRecord(byte[] nodeRawData)
        {
            this.userData.userData = getRecordData(2);
        }


    }
}
