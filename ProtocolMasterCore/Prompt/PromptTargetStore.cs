namespace ProtocolMasterCore.Prompt
{
    public class PromptTargetStore
    {
        public UserSelectHandler UserSelect { get; set; }
        public UserNumberHandler UserNumber { get; set; }

        public PromptTargetStore()
        {
            UserSelect = DefaultPrompts.UserSelect;
            UserNumber = DefaultPrompts.UserNumber;
        }
    }
}
