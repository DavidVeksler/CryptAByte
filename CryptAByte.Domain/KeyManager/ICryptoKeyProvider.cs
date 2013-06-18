using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptAByte.Domain.KeyManager
{
    interface ICryptoKeyProvider
    {
        CryptoKey GetKeyPairForDate(DateTime releaseDate);

        CryptoKey RequestForToken(Guid token);

        bool IsReleased(Guid token);
    }
}
