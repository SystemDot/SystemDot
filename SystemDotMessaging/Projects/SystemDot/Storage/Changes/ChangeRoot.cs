namespace SystemDot.Storage.Changes
{
    public abstract class ChangeRoot
    {
        readonly IChangeStore changeStore;
        readonly object checkPointLock = new object();
        int changeCount;

        protected ChangeRoot(IChangeStore changeStore)
        {
            this.changeStore = changeStore;
        }

        protected string Id { get; set; }

        public virtual void Initialise()
        {
            this.changeStore.GetChanges(this.Id).ForEach(ReplayChange);
        }

        protected void AddChange(Change change)
        {
            if (ShouldCheckPoint())
                UrgeCheckPoint();

            AddChangeWithoutCheckPoint(change);
        }

        protected abstract void UrgeCheckPoint();

        protected void CheckPoint(CheckPointChange change)
        {
            lock (this.checkPointLock)
            {
                if (!ShouldCheckPoint()) return;
                
                AddChangeWithoutCheckPoint(change);
                
                this.changeCount = 0;
            }
        }

        bool ShouldCheckPoint()
        {
            return this.changeCount >= 1000;
        }

        void AddChangeWithoutCheckPoint(Change change)
        {
            this.changeStore.StoreChange(this.Id, change);
            ReplayChange(change);

            this.changeCount++;
        }

        void ReplayChange(Change change)
        {
            GetType()
                .GetMethod("ApplyChange", new[] { change.GetType() })
                .Invoke(this, new object[] { change });
        }
    }
}