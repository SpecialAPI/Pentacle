namespace Pentacle.CustomEvent.Args.BasegameReferenceHolders
{
    internal class StatusFieldApplicationHolder(StatusFieldApplication statusFieldApplication) : IBoolHolder, IIntHolder, IStringHolder
    {
        public StatusFieldApplication statusFieldApplication = statusFieldApplication;

        bool IBoolHolder.this[int index]
        {
            get => index switch
            {
                0 => statusFieldApplication.canBeApplied,
                1 => statusFieldApplication.isStatusPositive,

                _ => false
            };
            set
            {
                if(index == 0)
                    statusFieldApplication.canBeApplied = value;
                else if(index == 1)
                    statusFieldApplication.isStatusPositive = value;
            }
        }
        bool IBoolHolder.Value
        {
            get => statusFieldApplication.canBeApplied;
            set => statusFieldApplication.canBeApplied = value;
        }

        int IIntHolder.this[int index]
        {
            get => statusFieldApplication.amount;
            set => statusFieldApplication.amount = value;
        }
        int IIntHolder.Value
        {
            get => statusFieldApplication.amount;
            set => statusFieldApplication.amount = value;
        }

        string IStringHolder.this[int index]
        {
            get => statusFieldApplication.statusID;
            set => statusFieldApplication.statusID = value;
        }
        string IStringHolder.Value
        {
            get => statusFieldApplication.statusID;
            set => statusFieldApplication.statusID = value;
        }
    }
}
