using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.Layouts;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer
{
    /// <summary>
    ///     Presenter for <see cref="LayoutViewerTreeView" />.
    /// </summary>
    internal sealed class LayoutViewerTreeViewPresenter
    {
        private readonly LayoutViewerTreeView _view;

        public LayoutViewerTreeViewPresenter(IEnumerable<Group> groups, LayoutViewerTreeView view)
        {
            _view = view;

            foreach (var group in groups)
                AddGroupView(group);
            _view.Reload();
        }

        private void AddGroupView(Group group, bool reload = true)
        {
            _view.AddGroup(group);
            if (reload)
                _view.Reload();
        }
    }
}
