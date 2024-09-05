using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Babelfisk.Entities
{
    public static class Hash
    {
        private readonly static HashAlgorithm _hash = HashAlgorithm.Create(typeof(SHA512).Name);

        public static string ComputeHash(string strValue)
        {
            if (String.IsNullOrEmpty(strValue))
                return null;

            _hash.ComputeHash(System.Text.Encoding.Unicode.GetBytes(strValue));

            return Convert.ToBase64String(_hash.Hash);
        }
    }
}
