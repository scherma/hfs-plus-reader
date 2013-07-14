using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk_Reader
{
    abstract class Partition
    {
        public ulong startblock { get; set; }
        public ulong length { get; set; }
        public partType partitionType { get; set; }

        public enum partType
        {
            unknown = -1,
            FAT12 = 0,
            FAT16 = 1,
            FAT32 = 2,
            NTFS = 3,
            HFS = 4,
            HFSPlus = 5
        }
    }
}
