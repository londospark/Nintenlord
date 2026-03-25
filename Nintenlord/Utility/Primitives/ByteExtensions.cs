using System;
using System.Text;

namespace Nintenlord.Utility.Primitives
{
    /// <summary>
    /// Extensions for byte and arrays of it
    /// </summary>
    public static class ByteExtensions
    {
        public static byte Shift(this byte i, int amount)
        {
            if (amount >= 0)
            {
                return (byte)(i << amount);
            }
            else
            {
                return (byte)(i >> -amount);
            }
        }

        public static bool IsInRange(this byte i, byte min, byte max) => i <= max && i >= min;

        public static int Clamp(this byte i, byte min, byte max) =>
            i < min ? min :
            i > max ? max : i;

        public static string ToHexString(this byte i, string prefix) => prefix + Convert.ToString(i, 16).ToUpper();

        public static byte GetBits(this byte i, int position, int length) => (byte)(i & GetMask(position, length));


        public static byte GetMask(int position, int length)
        {
            if (length < 0 || position < 0 || position + length > sizeof(byte) * 8)
            {
                throw new IndexOutOfRangeException();
            }
            var mask = byte.MaxValue;
            unchecked
            {
                mask >>= sizeof(byte) * 8 - length;
                mask <<= position;
            }

            return mask;
        }

        public static byte[] GetMaskArray(int position, int length)
        {
            byte[] result;

            var begIndex = position / 8;
            var firstByteBits = (8 - position & 0x7) & 0x7;

            var endIndex = (position + length) / 8;
            var lastByteBits = (position + length) & 0x7;

            var resultLength = endIndex;

            if (lastByteBits != 0)
            {
                resultLength++;
            }
            result = new byte[resultLength];
            if (((position & 0x7) + length) < 9)
            {
                result[begIndex] = GetMask(position & 0x7, length);
            }
            else
            {
                if (firstByteBits != 0)
                {
                    result[begIndex] = GetMask(8 - firstByteBits, firstByteBits);
                    begIndex++;
                }

                if (lastByteBits != 0)
                {
                    result[endIndex] = GetMask(0, lastByteBits);
                    //endIndex--;
                }

                for (var j = begIndex; j < endIndex; j++)
                {
                    result[j] = 0xFF;
                }

            }

            return result;
        }

        public static byte[] GetBits(this byte[] i, int position, int length)
        {
            if (position < 0 || length < 0 || position + length > i.Length * 8)
                throw new IndexOutOfRangeException();
            if (length == 0)
                return new byte[0];

            var byteIndex = position / 8;
            var bitIndex = position % 8;

            var byteLength = length / 8;
            var bitLength = length % 8;
            var bitTail = (position + length) % 8;

            var resultLength = byteLength;
            if (bitTail > 0)
                resultLength++;
            if (bitIndex > 0)
                resultLength++;
            var result = new byte[resultLength];

            var toTrim = resultLength - byteLength;
            if (bitLength > 0)
                toTrim--;

            Array.Copy(i, byteIndex, result, 0, Math.Min(result.Length, i.Length));

            if (bitIndex != 0)
                result = result.ShiftRight(bitIndex);

            if (toTrim > 0)
                Array.Resize(ref result, result.Length - toTrim);

            if (bitLength != 0)
                result[result.Length - 1] &= GetMask(0, bitLength);

            return result;
        }


        public static string ToString(this byte[] i, int bytesPerWord)
        {
            var result = new StringBuilder();
            for (var j = 0; j < i.Length; j++)
            {
                result.Append(i[j].ToHexString("").PadLeft(2, '0'));
                if (j % bytesPerWord == bytesPerWord - 1)
                {
                    result.Append(" ");
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Shifts bytes in array. Assumes bytes are in little endian order and 
        /// low priority bits are first
        /// </summary>
        /// <remarks>Could be made faster with using uints...</remarks>
        /// <param name="array">Array to shift</param>
        /// <param name="toShift">Positive means left shifting, negative right</param>
        /// <returns>New shifted array</returns>
        public static byte[] Shift(this byte[] array, int toShift)
        {
            if (toShift == 0)// <_<
                return array.Clone() as byte[];
            else if (toShift > 0)
                return ShiftLeft(array, toShift);
            else
                return ShiftRight(array, -toShift);
        }

        private static byte[] ShiftLeft(this byte[] array, int toShift)
        {
            var bytesToMove = toShift / 8;
            var bitsToMove = toShift % 8;
            var result = new byte[array.Length + bytesToMove + bitsToMove == 0 ? 0 : 1];

            Array.Copy(array, 0, result, bytesToMove, array.Length);

            var mask = GetMask(8 - bitsToMove, bitsToMove);
            byte temp = 0;
            for (var i = 0; i < result.Length; i++)
            {
                var value = (byte)((result[i] << bitsToMove) | (temp >> (8 - bitsToMove)));
                temp = (byte)(result[i] & mask);
                result[i] = value;
            }

            return result;
        }

        private static byte[] ShiftRight(this byte[] array, int toShift)
        {
            if (array.Length * 8 <= toShift)
                return new byte[0];

            var bytesToMove = toShift / 8;
            var bitsToMove = toShift % 8;
            var result = new byte[array.Length - bytesToMove];

            Array.Copy(array, bytesToMove, result, 0, result.Length);

            var mask = GetMask(0, bitsToMove);
            byte temp = 0;
            for (var i = result.Length - 1; i >= 0; i--)
            {
                var value = (byte)((result[i] >> bitsToMove) | (temp << (8 - bitsToMove)));
                temp = (byte)(result[i] & mask);
                result[i] = value;
            }

            return result;
        }


        public static byte[] And(this byte[] array, byte[] array2)
        {
            var result = new byte[Math.Max(array.Length, array2.Length)];
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                result[i] = (byte)(array[i] & array2[i]);
            }
            return result;
        }
        public static byte[] Or(this byte[] array, byte[] array2)
        {
            var result = new byte[Math.Max(array.Length, array2.Length)];
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                result[i] = (byte)(array[i] | array2[i]);
            }
            if (array.Length > array2.Length)
            {
                Array.Copy(array, index, result, index, result.Length - index);
            }
            else if (array.Length < array2.Length)
            {
                Array.Copy(array2, index, result, index, result.Length - index);
            }

            return result;
        }
        public static byte[] Xor(this byte[] array, byte[] array2)
        {
            var result = new byte[Math.Max(array.Length, array2.Length)];
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                result[i] = (byte)(array[i] ^ array2[i]);
            }
            if (array.Length > array2.Length)
            {
                Array.Copy(array, index, result, index, result.Length - index);
            }
            else if (array.Length < array2.Length)
            {
                Array.Copy(array2, index, result, index, result.Length - index);
            }
            return result;
        }
        public static byte[] Neg(this byte[] array)
        {
            var result = new byte[array.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(~array[i]);
            }
            return result;
        }
        public static void AndWith(this byte[] array, byte[] array2)
        {
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                array[i] = (byte)(array[i] & array2[i]);
            }
        }
        public static void OrWith(this byte[] array, byte[] array2)
        {
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                array[i] = (byte)(array[i] | array2[i]);
            }
        }
        public static void XorWith(this byte[] array, byte[] array2)
        {
            var index = Math.Min(array.Length, array2.Length);
            for (var i = 0; i < index; i++)
            {
                array[i] = (byte)(array[i] ^ array2[i]);
            }
        }
        public static void NegWith(this byte[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(~array[i]);
            }
        }

        public static void WriteTo(this byte[] array, int destination, byte[] source, int length)
        {
            var sourceIndex = 0;
            if (destination + length > array.Length * 8
             || sourceIndex + length > source.Length * 8
             || destination < 0
             || sourceIndex < 0)
            {
                throw new IndexOutOfRangeException();
            }

            for (var i = 0; i < length; i++)
            {
                var destIndex = destination + i;
                var destByteIndex = destIndex / 8;
                var destbitIndex = destIndex % 8;
                var srcByteIndex = i / 8;
                var srcBitIndex = i % 8;
                var srcMask = GetMask(srcBitIndex, 1);
                var destMask = GetMask(destbitIndex, 1);
                array[destByteIndex] &= (byte)~destMask;
                if ((source[srcByteIndex] & srcMask) != 0)
                {
                    array[destByteIndex] |= (byte)(1 << destbitIndex);
                }
            }
        }
    }
}
