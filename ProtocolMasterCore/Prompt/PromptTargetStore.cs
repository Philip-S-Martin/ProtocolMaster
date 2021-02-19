namespace ProtocolMasterCore.Prompt
{
    public class PromptTargetStore
    {
        public UserSelectHandler UserSelect { get; set; }
        public PromptTargetStore()
        {
            UserSelect = DefaultPrompts.UserSelect;
        }
    }
}
