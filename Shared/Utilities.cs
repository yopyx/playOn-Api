using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Utilities
    {
        // Utility: Generate a random Base64url string
        public static string RandomBase64url() {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+','-').Replace('/','_');

        }
    }
}
