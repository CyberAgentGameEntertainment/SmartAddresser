// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SmartAddresser.Editor.Foundation.ListableProperty
{
    [CustomPropertyDrawer(typeof(ListableProperty<>), true)]
    internal sealed class ListablePropertyDrawer : PropertyDrawer
    {
        private const string IsListModePropertyName = "_isListMode";
        private const string ValuesPropertyName = "_values";
        private const string ToggleButtonControlName = "ToggleButtonControl";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            var isListModeProperty = property.FindPropertyRelative(IsListModePropertyName);
            var valuesProperty = property.FindPropertyRelative(ValuesPropertyName);
            var isListMode = isListModeProperty.boolValue;

            var firstFieldRect = fieldRect;
            firstFieldRect.xMax -= 28;
            var modeButtonRect = fieldRect;
            modeButtonRect.xMin += firstFieldRect.width + EditorGUIUtility.standardVerticalSpacing;

            if (!isListMode)
            {
                if (valuesProperty.arraySize == 0) valuesProperty.arraySize++;

                var firstValueProperty = valuesProperty.GetArrayElementAtIndex(0);
                EditorGUI.PropertyField(firstFieldRect, firstValueProperty, label);

                GUI.SetNextControlName(ToggleButtonControlName);
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    isListModeProperty.boolValue = GUI.Toggle(modeButtonRect, isListModeProperty.boolValue,
                        ListablePropertyEditorUtility.ListIcon, GUI.skin.button);
                    if (ccs.changed)
                        GUI.FocusControl(ToggleButtonControlName);
                }
            }
            else
            {
                var labelRect = firstFieldRect;
                labelRect.width = EditorGUIUtility.labelWidth;
                var arraySizeRect = firstFieldRect;
                valuesProperty.isExpanded =
                    EditorGUI.Foldout(labelRect, valuesProperty.isExpanded, $"{label.text} List", true);
                var depth = valuesProperty.depth;
                var isExpanded = valuesProperty.isExpanded;

                valuesProperty.NextVisible(true);
                arraySizeRect.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(arraySizeRect, valuesProperty, new GUIContent());
                EditorGUI.indentLevel = indentLevel;

                isListModeProperty.boolValue = GUI.Toggle(modeButtonRect, isListModeProperty.boolValue,
                    ListablePropertyEditorUtility.ListIcon, GUI.skin.button);

                EditorGUI.indentLevel++;
                if (isExpanded)
                    while (valuesProperty.NextVisible(false))
                    {
                        if (depth >= valuesProperty.depth) break;

                        fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(fieldRect, valuesProperty);
                    }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0.0f;
            var isListModeProperty = property.FindPropertyRelative(IsListModePropertyName);
            var valuesProperty = property.FindPropertyRelative(ValuesPropertyName);
            var isListMode = isListModeProperty.boolValue;

            if (!isListMode)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var lineCount = 1;
                if (valuesProperty.isExpanded) lineCount += valuesProperty.arraySize;

                height += lineCount * EditorGUIUtility.singleLineHeight
                          + (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}
