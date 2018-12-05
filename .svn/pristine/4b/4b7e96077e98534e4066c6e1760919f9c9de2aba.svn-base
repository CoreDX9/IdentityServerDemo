using System;
using System.Text;

namespace Util.TypeExtensions
{
    public static class ByteArrayExtensions
    {
        public static string ToBase64String(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public static string ToHexString(this byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                
                foreach (var b in bytes)
                {
                    strB.Append(b.ToString("x2"));
                }

                hexString = strB.ToString();
            }
            return hexString;
        }
    }
}
