// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SmartAddresser.Editor.Foundation.CommandBasedUndo
{
    /// <summary>
    ///     Implements Undo and Redo by managing the undo/redo commands.
    /// </summary>
    public sealed class History
    {
        private const int DefaultLimit = 10000;
        private readonly List<HistoryCommand> _redoes = new List<HistoryCommand>();
        private readonly List<HistoryCommand> _undoes = new List<HistoryCommand>();
        private int _currentGroupId;

        /// <summary>
        ///     The maximum number of history that can be saved.
        /// </summary>
        public int Limit { get; set; } = DefaultLimit;

        /// <summary>
        ///     Register the history command in the history.
        /// </summary>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        public void Register(Action redo, Action undo)
        {
            var unit = new HistoryCommand(redo, undo, _currentGroupId);
            unit.ExecuteRedo();
            if (_undoes.Count >= Limit) _undoes.RemoveAt(0);

            _undoes.Add(unit);
            _redoes.Clear();
        }

        /// <summary>
        ///     <para> Increment the current group id. </para>
        ///     <para> If you want to Undo/Redo state independently, call it after <see cref="Register(object)" />. </para>
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
            if (_undoes.Count == 0) return;

            var lastUndoIndex = _undoes.Count - 1;
            var lastUnit = _undoes[lastUndoIndex];
            var targetGroupId = lastUnit.GroupId;

            while (true)
            {
                if (_undoes.Count == 0) break;

                var unit = _undoes[lastUndoIndex];
                if (unit.GroupId != targetGroupId) break;

                unit.ExecuteUndo();
                _undoes.RemoveAt(lastUndoIndex);
                _redoes.Add(unit);
                lastUndoIndex--;
            }
        }

        /// <summary>
        ///     Redo.
        /// </summary>
        public void Redo()
        {
            if (_redoes.Count == 0) return;

            var lastRedoIndex = _redoes.Count - 1;
            var lastUnit = _redoes[lastRedoIndex];
            var targetGroupId = lastUnit.GroupId;

            while (true)
            {
                if (_redoes.Count == 0) break;

                var unit = _redoes[lastRedoIndex];
                if (unit.GroupId != targetGroupId) break;

                unit.ExecuteRedo();
                _redoes.RemoveAt(lastRedoIndex);
                _undoes.Add(unit);
                lastRedoIndex--;
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
