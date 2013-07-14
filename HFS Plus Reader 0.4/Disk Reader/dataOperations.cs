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
using System.Security.Cryptography;
using System.IO;

namespace Disk_Reader
{
    public static class dataOperations
    {
        public struct hashValues
        {
            public byte[] md5hash { get; set; }
            public byte[] sha1hash { get; set; }
        }
        public static hashValues getHashValues(Stream stream, long length)
        {
            hashValues result = new hashValues();

            var md5 = MD5.Create();
            var sha1 = SHA1.Create();

            stream.Position = 0;
            byte[] buffer;
            if (length > 8192)
            {
                buffer = new byte[8192];
            }
            else
            {
                buffer = new byte[length];
            }
            byte[] bufferout = new byte[8192];
            int bytesRead;
            long bytesRemaining = length;

            while (bytesRemaining > 0)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                md5.TransformBlock(buffer, 0, bytesRead, bufferout, 0);
                sha1.TransformBlock(buffer, 0, bytesRead, bufferout, 0);
                bytesRemaining -= bytesRead;

                if (bytesRemaining < buffer.Length)
                {
                    buffer = new byte[bytesRemaining];
                }
            }
            // We have to call TransformFinalBlock, but we don't have any
            // more data - just provide 0 bytes.
            md5.TransformFinalBlock(buffer, 0, 0);
            sha1.TransformFinalBlock(buffer, 0, 0);

            result.md5hash = md5.Hash;
            result.sha1hash = sha1.Hash;

            return result;
        }
        public static hashValues getHashValues(byte[] bytes)
        {
            hashValues result = new hashValues();

            var md5 = MD5.Create().ComputeHash(bytes);
            var sha1 = SHA1.Create().ComputeHash(bytes);

            result.md5hash = md5;
            result.sha1hash = sha1;

            return result;
        }
        public static string[] buildHashStrings(hashValues hv)
        {
            string[] hashStrings = new string[2];

            StringBuilder sbmd5 = new StringBuilder();
            if (hv.md5hash != null)
            {
                for (int j = 0; j < hv.md5hash.Length; j++)
                {
                    sbmd5.Append(hv.md5hash[j].ToString("X2"));
                }
                hashStrings[0] = sbmd5.ToString();
            } else {
                hashStrings[0] = "No data present";
            }

            StringBuilder sbsha1 = new StringBuilder();
            if (hv.sha1hash != null)
            {
                for (int j = 0; j < hv.sha1hash.Length; j++)
                {
                    sbsha1.Append(hv.sha1hash[j].ToString("X2"));
                }
                hashStrings[1] = sbsha1.ToString();
            }
            else
            {
                hashStrings[1] = "No data present";
            }

            return hashStrings;
        }
        public static ushort convToLE(ushort data)
        {
            byte[] b = new byte[2];
            b[1] = (byte)data;
            b[0] = (byte)(((ushort)data >> 8) & 0xFF);

            return BitConverter.ToUInt16(b, 0);
        }
        public static short convToLE(short data)
        {
            byte[] b = new byte[2];
            b[1] = (byte)data;
            b[0] = (byte)(((short)data >> 8) & 0xFF);

            return BitConverter.ToInt16(b, 0);
        }
        public static uint convToLE(uint data)
        {
            byte[] b = new byte[4];
            b[3] = (byte)data;
            b[2] = (byte)(((uint)data >> 8) & 0xFF);
            b[1] = (byte)(((uint)data >> 16) & 0xFF);
            b[0] = (byte)(((uint)data >> 24) & 0xFF);

            return BitConverter.ToUInt32(b, 0);
        }
        public static int convToLE(int data)
        {
            byte[] b = new byte[4];
            b[3] = (byte)data;
            b[2] = (byte)(((int)data >> 8) & 0xFF);
            b[1] = (byte)(((int)data >> 16) & 0xFF);
            b[0] = (byte)(((int)data >> 24) & 0xFF);

            return BitConverter.ToInt32(b, 0);
        }
        public static ulong convToLE(ulong data)
        {
            byte[] b = new byte[8];
            b[7] = (byte)data;
            b[6] = (byte)(((ulong)data >> 8) & 0xFF);
            b[5] = (byte)(((ulong)data >> 16) & 0xFF);
            b[4] = (byte)(((ulong)data >> 24) & 0xFF);
            b[3] = (byte)(((ulong)data >> 32) & 0xFF);
            b[2] = (byte)(((ulong)data >> 40) & 0xFF);
            b[1] = (byte)(((ulong)data >> 48) & 0xFF);
            b[0] = (byte)(((ulong)data >> 56) & 0xFF);

            return BitConverter.ToUInt64(b, 0);
        }
        public enum keyCompareResult
        {
            greaterThanTrialKey = 1,
            equalsTrialKey = 0,
            lessThanTrialKey = -1
        }
        public struct hashes 
        {
            public string md5sum { get; set; }
            public string sha1sum { get; set; }
        }
        public static bool is_bit_set(byte value, short bitindex)
        {
            return (value & (1 << bitindex)) != 0;
        }
    }
}
