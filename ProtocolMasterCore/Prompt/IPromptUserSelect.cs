namespace ProtocolMasterCore.Prompt
{
    public delegate string UserSelectHandler(string[] keys);
    public interface IPromptUserSelect
    {
        public UserSelectHandler UserSelectPrompt { set; }
    }
}
