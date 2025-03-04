using System.Text;

namespace HontelOS.System.Conversion
{
    public static class Conversion
    {
        public static string Hex(this byte[] value)
        {
            int offset = 0;
            int length = -1;

            if (length < 0)
                length = value.Length - offset;
            var builder = new StringBuilder(length * 2);
            int b;
            for (int i = offset; i < length + offset; i++)
            {
                b = value[i] >> 4;
                builder.Append((char)(55 + b + (b - 10 >> 31 & -7)));
                b = value[i] & 0xF;
                builder.Append((char)(55 + b + (b - 10 >> 31 & -7)));
            }
            return builder.ToString();
        }
    }
}
