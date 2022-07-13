// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace SmartAddresser.Editor.Foundation.CommandBasedUndo
{
    internal class HistoryCommand
    {
        private readonly Action _redo;
        private readonly Action _undo;

        public HistoryCommand(Action redo, Action undo, int groupId)
        {
            if (redo == null) throw new ArgumentNullException(nameof(redo));

            if (undo == null) throw new ArgumentNullException(nameof(undo));

            _redo = redo;
            _undo = undo;
            GroupId = groupId;
        }

        public int GroupId { get; }

        public void ExecuteRedo()
        {
            _redo.Invoke();
        }

        public void ExecuteUndo()
        {
            _undo.Invoke();
        }
    }
}
