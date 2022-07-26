using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Foundation.TinyRx;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressLayoutViewer
{
    /// <summary>
    ///     Presenter for <see cref="AddressLayoutViewerView" />.
    /// </summary>
    internal sealed class AddressLayoutViewerViewPresenter
    {
        private readonly AddressLayoutViewerView _view;

        public AddressLayoutViewerViewPresenter(IEnumerable<Group> groups, AddressLayoutViewerView view)
        {
            _view = view;

            foreach (var group in groups)
                AddGroupView(group);
            _view.TreeView.Reload();
        }

        private void AddGroupView(Group group, bool reload = true)
        {
            group.RefreshErrorType();
            _view.TreeView.AddGroup(group);
            if (reload)
                _view.TreeView.Reload();
        }
    }
}
