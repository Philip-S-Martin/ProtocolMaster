namespace ProtocolMasterCore.Prompt
{
    public static class DefaultPrompts
    {
        public static string UserSelect(string[] options, string prompt)
        {
            if (options != null && options.Length > 0 && options[0] != null)
                return options[0];
            return "null";
        }

        public static int UserNumber(int min, int max, string prompt)
        {
            return min;
        }
    }
}
