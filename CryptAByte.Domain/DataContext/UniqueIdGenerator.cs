using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptAByte.Domain.DataContext
{
    public static class UniqueIdGenerator
    {
        public static string GetUniqueId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

    }
}
