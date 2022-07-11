namespace SmartAddresser.Editor.Foundation.StateBasedUndo
{
    internal class HistoryUnit
    {
        public HistoryUnit(IObjectStateSnapshot snapshot, int groupId)
        {
            Snapshot = snapshot;
            GroupId = groupId;
        }

        public IObjectStateSnapshot Snapshot { get; }
        public int GroupId { get; }
    }
}
