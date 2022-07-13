namespace SmartAddresser.Editor.Foundation.StateBasedUndo
{
    /// <summary>
    ///     Interface for taking and restoring a snapshot of a object's state.
    /// </summary>
    public interface IObjectStateSnapshot
    {
        /// <summary>
        ///     Take a snapshot.
        /// </summary>
        void Take();

        /// <summary>
        ///     Restore state from a snapshot.
        /// </summary>
        void Restore();
    }
}
