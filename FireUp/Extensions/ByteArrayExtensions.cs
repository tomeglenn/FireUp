using System.Text;

namespace FireUp.Extensions
{
    internal static class ByteArrayExtensions
    {
        public static string ToUtf8String(this byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }
    }
}
