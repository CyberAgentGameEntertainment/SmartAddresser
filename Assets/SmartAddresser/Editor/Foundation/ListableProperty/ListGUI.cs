using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    public abstract class ListGUI<T>
    {
        private readonly Dictionary<int, T> _dirtyValues = new Dictionary<int, T>();
        private readonly IList<T> _list;

        protected ListGUI(IList<T> list)
        {
            _list = list;
        }

        public bool IndentElements { get; set; } = true;
        public bool ShowTitle { get; set; } = true;
        public bool ShowSize { get; set; } = true;
        public bool Foldout { get; set; }
        public bool CanMoveElements { get; set; } = true;

        public void DoLayout()
        {
            if (ShowTitle)
                DrawTitleField();

            if (!Foldout)
                return;

            if (IndentElements)
                EditorGUI.indentLevel++;

            if (ShowSize) DrawSizeField();

            DrawElementFields();

            if (IndentElements)
                EditorGUI.indentLevel--;
        }

        private void DrawTitleField()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Foldout = EditorGUILayout.Foldout(Foldout, "Extension", true);
            }
        }

        private void DrawSizeField()
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var size = EditorGUILayout.DelayedIntField("Size", _list.Count);
                if (ccs.changed)
                    ResizeList(size);
            }
        }

        private void DrawElementFields()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var index = i;
                using (new EditorGUILayout.HorizontalScope())
                {
                    var rect = EditorGUILayout.GetControlRect(true, 18f);
                    DrawElementGUI(rect, $"Element {i}", _list[i], value => _dirtyValues[index] = value);

                    if (_dirtyValues.TryGetValue(i, out var v))
                    {
                        _list[i] = v;
                        _dirtyValues.Remove(i);
                        GUI.changed = true;
                    }
                }

                if (Event.current.type == EventType.MouseDown)
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    if (rect.Contains(Event.current.mousePosition) && Event.current.button == 1)
                        OnElementRightClicked(i);
                }
            }
        }

        private void OnElementRightClicked(int index)
        {
            var menu = new GenericMenu();

            // Remove
            menu.AddItem(new GUIContent("Remove"), false, () => _list.RemoveAt(index));

            if (CanMoveElements)
            {
                // Move Up
                var moveUpLabel = new GUIContent("Move Up");
                if (index == 0)
                    menu.AddDisabledItem(moveUpLabel, false);
                else
                    menu.AddItem(moveUpLabel, false, () =>
                    {
                        var targetIndex = index - 1;
                        var item = _list[index];
                        _list.RemoveAt(index);
                        _list.Insert(targetIndex, item);
                    });

                // Move Down
                var moveDownLabel = new GUIContent("Move Down");
                if (index == _list.Count - 1)
                    menu.AddDisabledItem(moveDownLabel, false);
                else
                    menu.AddItem(moveDownLabel, false, () =>
                    {
                        var targetIndex = index + 1;
                        var item = _list[index];
                        _list.RemoveAt(index);
                        _list.Insert(targetIndex, item);
                    });
            }

            menu.ShowAsContext();
        }

        private void ResizeList(int newSize)
        {
            while (_list.Count < newSize)
                _list.Add(default);

            while (_list.Count > newSize)
                _list.RemoveAt(_list.Count - 1);
        }

        protected abstract void DrawElementGUI(Rect rect, string label, T value, Action<T> onValueChanged);
    }

    public sealed class AnonymousListGUI<T> : ListGUI<T>
    {
        private readonly Action<Rect, string, T, Action<T>> _drawElementGUI;

        public AnonymousListGUI(IList<T> list, Action<Rect, string, T, Action<T>> drawElementGUI) : base(list)
        {
            _drawElementGUI = drawElementGUI;
        }

        protected override void DrawElementGUI(Rect rect, string label, T value, Action<T> onValueChanged)
        {
            _drawElementGUI.Invoke(rect, label, value, onValueChanged);
        }
    }

    public sealed class IntListGUI : ListGUI<int>
    {
        public IntListGUI(IList<int> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.IntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class DelayedIntListGUI : ListGUI<int>
    {
        public DelayedIntListGUI(IList<int> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.DelayedIntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class FloatListGUI : ListGUI<float>
    {
        public FloatListGUI(IList<float> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, float value, Action<float> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.FloatField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class DelayedFloatListGUI : ListGUI<float>
    {
        public DelayedFloatListGUI(IList<float> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, float value, Action<float> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.DelayedFloatField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class LongListGUI : ListGUI<long>
    {
        public LongListGUI(IList<long> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, long value, Action<long> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.LongField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class DoubleListGUI : ListGUI<double>
    {
        public DoubleListGUI(IList<double> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, double value, Action<double> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.DoubleField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class DelayedDoubleListGUI : ListGUI<double>
    {
        public DelayedDoubleListGUI(IList<double> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, double value, Action<double> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.DelayedDoubleField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class TextListGUI : ListGUI<string>
    {
        public TextListGUI(IList<string> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, string value, Action<string> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.TextField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class DelayedTextListGUI : ListGUI<string>
    {
        public DelayedTextListGUI(IList<string> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, string value, Action<string> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.DelayedTextField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class ObjectListGUI : ListGUI<Object>
    {
        private readonly bool _allowSceneObject;
        private readonly Type _type;

        public ObjectListGUI(IList<Object> list, Type type, bool allowSceneObject) : base(list)
        {
            _type = type;
            _allowSceneObject = allowSceneObject;
        }

        protected override void DrawElementGUI(Rect rect, string label, Object value, Action<Object> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.ObjectField(rect, label, value, _type, _allowSceneObject);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class Vector2ListGUI : ListGUI<Vector2>
    {
        public Vector2ListGUI(IList<Vector2> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Vector2 value, Action<Vector2> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Vector2Field(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class Vector3ListGUI : ListGUI<Vector3>
    {
        public Vector3ListGUI(IList<Vector3> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Vector3 value, Action<Vector3> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Vector3Field(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class Vector4ListGUI : ListGUI<Vector4>
    {
        public Vector4ListGUI(IList<Vector4> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Vector4 value, Action<Vector4> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Vector4Field(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class Vector2IntListGUI : ListGUI<Vector2Int>
    {
        public Vector2IntListGUI(IList<Vector2Int> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Vector2Int value,
            Action<Vector2Int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Vector2IntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class Vector3IntListGUI : ListGUI<Vector3Int>
    {
        public Vector3IntListGUI(IList<Vector3Int> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Vector3Int value,
            Action<Vector3Int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Vector3IntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class RectListGUI : ListGUI<Rect>
    {
        public RectListGUI(IList<Rect> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Rect value, Action<Rect> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.RectField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class RectIntListGUI : ListGUI<RectInt>
    {
        public RectIntListGUI(IList<RectInt> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, RectInt value, Action<RectInt> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.RectIntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class BoundsListGUI : ListGUI<Bounds>
    {
        public BoundsListGUI(IList<Bounds> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Bounds value, Action<Bounds> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.BoundsField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class BoundsIntListGUI : ListGUI<BoundsInt>
    {
        public BoundsIntListGUI(IList<BoundsInt> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, BoundsInt value,
            Action<BoundsInt> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.BoundsIntField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class CurveListGUI : ListGUI<AnimationCurve>
    {
        public CurveListGUI(IList<AnimationCurve> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, AnimationCurve value,
            Action<AnimationCurve> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.CurveField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class ColorListGUI : ListGUI<Color>
    {
        public ColorListGUI(IList<Color> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Color value, Action<Color> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.ColorField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class GradientListGUI : ListGUI<Gradient>
    {
        public GradientListGUI(IList<Gradient> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Gradient value, Action<Gradient> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.GradientField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class PopupListGUI : ListGUI<int>
    {
        private readonly string[] _displayOptions;

        public PopupListGUI(IList<int> list, string[] displayOptions) : base(list)
        {
            _displayOptions = displayOptions;
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.Popup(rect, label, value, _displayOptions);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class IntPopupListGUI : ListGUI<int>
    {
        private readonly string[] _displayOptions;
        private readonly int[] _optionValues;

        public IntPopupListGUI(IList<int> list, string[] displayOptions, int[] optionValues) : base(list)
        {
            _displayOptions = displayOptions;
            _optionValues = optionValues;
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.IntPopup(rect, label, value, _displayOptions, _optionValues);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class EnumPopupListGUI : ListGUI<Enum>
    {
        public EnumPopupListGUI(IList<Enum> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Enum value, Action<Enum> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.EnumPopup(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class EnumFlagsListGUI : ListGUI<Enum>
    {
        public EnumFlagsListGUI(IList<Enum> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, Enum value, Action<Enum> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.EnumFlagsField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class LayerListGUI : ListGUI<int>
    {
        public LayerListGUI(IList<int> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.LayerField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class MaskListGUI : ListGUI<int>
    {
        private readonly string[] _displayOptions;

        public MaskListGUI(IList<int> list, string[] displayOptions) : base(list)
        {
            _displayOptions = displayOptions;
        }

        protected override void DrawElementGUI(Rect rect, string label, int value, Action<int> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.MaskField(rect, label, value, _displayOptions);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }

    public sealed class TagListGUI : ListGUI<string>
    {
        public TagListGUI(IList<string> list) : base(list)
        {
        }

        protected override void DrawElementGUI(Rect rect, string label, string value, Action<string> onValueChanged)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                var newValue = EditorGUI.TagField(rect, label, value);
                if (ccs.changed)
                    onValueChanged.Invoke(newValue);
            }
        }
    }
}
