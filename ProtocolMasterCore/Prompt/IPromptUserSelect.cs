namespace ProtocolMasterCore.Prompt
{
    public delegate string UserSelectHandler(string[] keys, string prompt = "Please select an item:");
    public interface IPromptUserSelect
    {
        public UserSelectHandler UserSelectPrompt { set; }
    }
}
