namespace BlazorApp
{
    public static class SolutionEncryption
    {
        public static string Encrypt(int k)
        {
            var str = Convert.ToBase64String(BitConverter.GetBytes(k));
            var reverseStr = Reverse(str);
            var swapStr = reverseStr[^1] + reverseStr[1..^1] + reverseStr[0];
            return swapStr;
        }

        public static string Decrypt(string encoded)
        {
            var swapStr = encoded[^1] + encoded[1..^1] + encoded[0];
            var reverseStr = Reverse(swapStr);
            var buffer = new Span<byte>(new byte[reverseStr.Length]);
            if (!Convert.TryFromBase64String(reverseStr, buffer, out _))
            {
                return "00000";
            }
            return BitConverter.ToInt32(buffer.ToArray(), 0).ToString();
        }

        private static string Reverse(string input)
        {
            return string.Create(input.Length, input, (chars, state) =>
            {
                state.AsSpan().CopyTo(chars);
                chars.Reverse();
            });
        }
    }
}