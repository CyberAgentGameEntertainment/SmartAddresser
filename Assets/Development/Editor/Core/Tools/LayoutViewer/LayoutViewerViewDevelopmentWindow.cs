using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartAddresser.Editor.Core.Models.Layouts;
using SmartAddresser.Editor.Core.Tools.Addresser.LayoutViewer;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Development.Editor.Core.Tools.LayoutViewer
{
    internal sealed class LayoutViewerViewDevelopmentWindow : EditorWindow
    {
        private const string LoremIpsum =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private const string WindowName = "[Dev] Address Viewer View";

        [SerializeField] private LayoutViewerTreeView.State _treeViewState;
        private readonly List<Group> _groups = new List<Group>();
        private LayoutViewerView _view;

        private void OnEnable()
        {
            minSize = new Vector2(600, 200);

            var splitLoremIpsum = LoremIpsum.Split(' ');

            string GetRandomWord()
            {
                return splitLoremIpsum[Random.Range(0, splitLoremIpsum.Length)];
            }

            var dummyAssetPath = AssetDatabase.GetAllAssetPaths();

            string GetRandomAssetPath()
            {
                return dummyAssetPath[Random.Range(0, dummyAssetPath.Length)];
            }

            var layoutErrorTypeValues = Enum.GetValues(typeof(LayoutErrorType));

            LayoutErrorType GetRandomErrorType()
            {
                var index = Random.Range(0, layoutErrorTypeValues.Length);
                return (LayoutErrorType)layoutErrorTypeValues.GetValue(index);
            }

            string GetRandomSentence()
            {
                var startIndex = Random.Range(0, splitLoremIpsum.Length - 30);
                var length = Random.Range(0, 20);
                return string.Join(" ", splitLoremIpsum.Skip(startIndex).Take(length));
            }

            // Create some groups at random.
            for (var i = 0; i < 10; i++)
            {
                var addressableGroup = CreateInstance<AddressableAssetGroup>();
                addressableGroup.Name = $"Group - {GetRandomWord()}";
                var group = new Group(addressableGroup);
                var assetCount = Random.Range(3, 20);

                for (var j = 0; j < assetCount; j++)
                {
                    var assetName = Path.GetFileNameWithoutExtension(GetRandomAssetPath());
                    var assetPath = GetRandomAssetPath();
                    var labels = Enumerable.Range(0, Random.Range(0, 5)).Select(x => GetRandomWord()).ToArray();
                    var tags = Enumerable.Range(0, Random.Range(0, 3)).Select(x => GetRandomWord()).ToArray();
                    var errorType = GetRandomErrorType();
                    var message = GetRandomSentence();
                    var entry = new Entry(assetName, assetPath, labels, tags);
                    switch (errorType)
                    {
                        case LayoutErrorType.Warning:
                            entry.Errors.Add(new EntryError(EntryErrorType.Warning, message));
                            break;
                        case LayoutErrorType.Error:
                            entry.Errors.Add(new EntryError(EntryErrorType.Error, message));
                            break;
                        case LayoutErrorType.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    group.Entries.Add(entry);
                }

                _groups.Add(group);
            }

            if (_treeViewState == null)
                _treeViewState = new LayoutViewerTreeView.State();
            _view = new LayoutViewerView(_treeViewState);
            var _ = new LayoutViewerViewPresenter(_groups, _view);
        }

        private void OnGUI()
        {
            _view.DoLayout();
        }

        [MenuItem("Window/Smart Addresser/Development/Layout Viewer/Layout Viewer View")]
        public static void Open()
        {
            GetWindow<LayoutViewerViewDevelopmentWindow>(WindowName);
        }
    }
}
