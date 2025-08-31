namespace Pentacle.Triggers.Args.BasegameReferenceHolders
{
    internal class StringReferenceHolder(StringReference stringRef) : IStringHolder
    {
        public StringReference stringRef = stringRef;

        string IStringHolder.this[int index]
        {
            get => stringRef.value;
            set => stringRef.value = value;
        }
        string IStringHolder.Value
        {
            get => stringRef.value;
            set => stringRef.value = value;
        }
    }
}
