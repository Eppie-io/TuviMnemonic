///////////////////////////////////////////////////////////////////////////////
//   Copyright 2023 Eppie (https://eppie.io)
//
//   Licensed under the Apache License, Version 2.0(the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Numerics;

namespace MnemonicSharingLib
{
    public static class BigIntegerExtension
    {
        public const int byteSizeInBits = 8;

        /// <summary>
        /// Converting POSITIVE BigInteger into byte array with big-endian format.
        /// </summary>
        /// <param name="number">BigInteger number.</param>
        /// <returns>Byte array with big-endian format.</returns>
        public static byte[] ToBigEndianByteArray(this BigInteger number)
        {
            BigInteger temp = number;
            List<byte> result = new List<byte>();
            while (temp > 0)
            {
                byte currentByte = (byte)(temp & 0xff);
                result.Add(currentByte);
                temp >>= byteSizeInBits;
            }

            result.Reverse();
            return result.ToArray();
        }

        /// <summary>
        /// Converting POSITIVE BigInteger into byte array of choosen length with big-endian format.
        /// </summary>
        /// <param name="number">BigInteger number.</param>
        /// <param name="length">Array length.</param>
        /// <returns>Byte array with big-endian format.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToBigEndianByteArrayWithSpecificLength(this BigInteger number, int length)
        {
            if (number.ToBigEndianByteArray().Length > length)
            {
                throw new ArgumentException("Length parameter should be bigger than number size.", nameof(length));
            }
            BigInteger temp = number;
            byte[] bytes = new byte[length];
            int index = length - 1;
            while (temp > 0)
            {
                byte currentByte = (byte)(temp & 0xff);
                bytes[index] = currentByte;
                temp >>= byteSizeInBits;
                index--;
                if (index < 0)
                {
                    break;
                }
            }
            return bytes;
        }
    }
}
