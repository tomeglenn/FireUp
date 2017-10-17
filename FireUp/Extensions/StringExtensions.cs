using System.Text;

namespace FireUp.Extensions
{
    internal static class StringExtensions
    {
        public static byte[] ToUtf8Bytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}
