using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk_Reader
{
    abstract class PartitionTable
    {
        public ulong tablestart { get; protected set; }
        public ulong tablelength { get; protected set; }
        public int numberofentries { get; protected set; }
        public int sizeofentry { get; protected set; }
    }
}
