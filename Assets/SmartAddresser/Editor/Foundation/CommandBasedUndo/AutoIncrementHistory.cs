// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.CommandBasedUndo
{
    /// <summary>
    ///     <see cref="History" /> that does <see cref="History.IncrementCurrentGroup" /> automatically.
    /// </summary>
    public class AutoIncrementHistory
    {
        private readonly History _history = new History();
        private string _currentHistoryId;

        /// <summary>
        ///     The maximum number of history that can be saved.
        /// </summary>
        public int Limit
        {
            get => _history.Limit;
            set => _history.Limit = value;
        }

        /// <summary>
        ///     Register the history command in the history.
        /// </summary>
        /// <param name="actionTypeId">
        ///     A unique ID for each type of action to be registered.
        ///     <see cref="History.IncrementCurrentGroup" /> will be called when this changes.
        /// </param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Register(string actionTypeId, Action redo, Action undo)
        {
            if (actionTypeId == null) throw new ArgumentNullException(nameof(actionTypeId));

            if (!Equals(actionTypeId, _currentHistoryId)) _history.IncrementCurrentGroup();

            _history.Register(redo, undo);
            _currentHistoryId = actionTypeId;
        }

        /// <summary>
        ///     Undo.
        /// </summary>
        public void Undo()
        {
            _history.Undo();
        }

        /// <summary>
        ///     Redo.
        /// </summary>
        public void Redo()
        {
            _history.Redo();
        }

        /// <summary>
        ///     Clear the history.
        /// </summary>
        public void Clear()
        {
            _history.Clear();
        }
    }
}
