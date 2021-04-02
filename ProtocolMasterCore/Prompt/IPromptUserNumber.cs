namespace ProtocolMasterCore.Prompt
{
    public delegate int UserNumberHandler(int min, int max, string prompt = "Please select a number:");
    public interface IPromptUserNumber
    {
        public UserNumberHandler UserNumberPrompt { set; }
    }
}
