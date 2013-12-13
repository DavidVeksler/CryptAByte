// UNIT RandomNumberGeneratorBase
// UNIT StrongRandomNumberGenerator
using System;
using System.Security.Cryptography;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    public abstract class RandomNumberGeneratorBase
{    
        private int _byteBufSize;
        private byte[] _buf;
        private int _idx;
        private int _lastsize;

        public RandomNumberGeneratorBase(int bufSize = 8092)
    {    
            _byteBufSize = bufSize;
            _buf = new byte[_byteBufSize];
            _idx = _byteBufSize;
        }

        protected abstract void GetNewBuf(byte[] buf);

        private void CheckBuf(int bytesFreeNeeded = 1)
        {    
            _idx += _lastsize;
            _lastsize = bytesFreeNeeded;
            if (_idx + bytesFreeNeeded < _byteBufSize) { return; }
            GetNewBuf(_buf);
            _idx      = 0;
            _lastsize = 0;
        }

        public byte GetRandomByteStartAtZero(int belowValue)
       {    
         return (byte)(Math.Round(((double)GetRandomByte() * (belowValue - 1)) / 255));
       }    

        public int GetRandomIntStartAtZero(int belowValue)
       {    
            return (int)(Math.Round(((double)GetRandomUInt32() * (double)(belowValue - 1)) / (double)uint.MaxValue));
       }    

        public byte GetRandomByte()
    {    
            CheckBuf();
        return _buf[_idx];
    }    

        public bool GetRandomBool()
    {    
            CheckBuf();
        return _buf[_idx] > 127;
    }    

        public ulong GetRandomULong()
    {    
            CheckBuf(sizeof(ulong));
        return BitConverter.ToUInt64(_buf, _idx);
    }    

        public int GetRandomInt()
    {    
            CheckBuf(sizeof(int));
        return BitConverter.ToInt32(_buf, _idx);
    }    

        /// <summary>
        ///     Double from 0 to 1 (might be zero, will never be 1)
        /// </summary>
        public double GetRandomDouble()
    {    
            return GetRandomUInt32() / (1d + UInt32.MaxValue);
    }    

        /// <summary>
        ///     Float from 0 to 1 (might be zero, will never be 1)
        /// </summary>
        public float GetRandomFloat()
    {    
            return GetRandomUInt32() / (1f + UInt32.MaxValue);
    }    

        public uint GetRandomUInt32()
    {    
            CheckBuf(sizeof(UInt32));
            return BitConverter.ToUInt32(_buf, _idx);
    }    
    }    

    public sealed class StrongRandomNumberGenerator : RandomNumberGeneratorBase
{    
        private RNGCryptoServiceProvider _rnd;

        public StrongRandomNumberGenerator()
    {    
            _rnd = new RNGCryptoServiceProvider();
    }    

        protected override void GetNewBuf(byte[] buf)
    {    
            _rnd.GetBytes(buf);
    }    

    }
}    