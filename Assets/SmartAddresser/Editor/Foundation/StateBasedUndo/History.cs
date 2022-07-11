using System;
using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.StateBasedUndo
{
    /// <summary>
    ///     Implements Undo and Redo by managing the state history of objects.
    /// </summary>
    public sealed class History
    {
        private const int DefaultLimit = 10000;
        private readonly List<HistoryUnit> _redoes = new List<HistoryUnit>();
        private readonly Func<object, IObjectStateSnapshot> _takeSnapshot;
        private readonly object _target;
        private readonly List<HistoryUnit> _undoes = new List<HistoryUnit>();
        private HistoryUnit _current;
        private int _currentGroupId;

        public History(object target, Func<object, IObjectStateSnapshot> takeSnapshot = null)
        {
            _target = target;
            _takeSnapshot = takeSnapshot;
            if (_takeSnapshot == null)
                _takeSnapshot = obj => new UnityJsonObjectStateSnapshot(obj);
        }

        /// <summary>
        ///     The maximum number of history that can be saved.
        /// </summary>
        public int Limit { get; set; } = DefaultLimit;

        public IObjectStateSnapshot TakeSnapshot()
        {
            var snapshot = _takeSnapshot(_target);
            snapshot.Take();
            return snapshot;
        }

        /// <summary>
        ///     <para> Register the current state of the object in the history. </para>
        ///     <para> Unity's JsonUtility is used to capture Snapshots. </para>
        /// </summary>
        public bool RegisterSnapshot()
        {
            var snapshot = TakeSnapshot();
            return RegisterSnapshot(snapshot);
        }

        public bool RegisterSnapshot(IObjectStateSnapshot snapshot)
        {
            if (_current != null && _current.Snapshot.Equals(snapshot))
                return false;
            
            var unit = new HistoryUnit(snapshot, _currentGroupId);

            if (_undoes.Count >= Limit)
                _undoes.RemoveAt(0);

            if (_current != null)
                _undoes.Add(_current);

            _current = unit;
            _redoes.Clear();

            return true;
        }

        /// <summary>
        ///     <para> Increment the current group id. </para>
        ///     <para> If you want to Undo/Redo state independently, call it after <see cref="RegisterSnapshot" />. </para>
        /// </summary>
        public void IncrementCurrentGroup()
        {
            _currentGroupId++;
        }

        /// <summary>
        ///     Undo.
        /// </summary>
        public void Undo()
        {
            if (_undoes.Count == 0)
                return;

            if (_current == null)
                return;

            var targetGroupId = _current.GroupId;

            var index = _undoes.Count - 1;
            while (true)
            {
                if (_undoes.Count == 0) break;

                var unit = _undoes[index];
                var snapshot = unit.Snapshot;
                snapshot.Restore();
                _undoes.RemoveAt(index);
                _redoes.Add(_current);
                _current = unit;
                index--;

                if (unit.GroupId != targetGroupId) break;
            }
        }

        /// <summary>
        ///     Redo.
        /// </summary>
        public void Redo()
        {
            if (_redoes.Count == 0) return;

            if (_current == null) return;

            var lastRedoIndex = _redoes.Count - 1;
            var lastUnit = _redoes[lastRedoIndex];
            var targetGroupId = lastUnit.GroupId;

            var index = lastRedoIndex;
            while (true)
            {
                if (_redoes.Count == 0) break;

                var unit = _redoes[index];

                if (unit.GroupId != targetGroupId) break;

                var snapshot = unit.Snapshot;
                snapshot.Restore();
                _redoes.RemoveAt(index);
                _undoes.Add(_current);
                _current = unit;
                index--;
            }
        }

        /// <summary>
        ///     Clear the history.
        /// </summary>
        public void Clear()
        {
            _undoes.Clear();
            _redoes.Clear();
        }
    }
}
