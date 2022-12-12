using System;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableProperty;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.LayoutRules.Settings
{
    [Serializable]
    public sealed class LayoutRuleSettings
    {
        [SerializeField] private ObservableProperty<bool> _excludeUnversioned = new ObservableProperty<bool>(false);
        [SerializeField] private ObservableProperty<string> _versionExpression = new ObservableProperty<string>();

        public IObservableProperty<bool> ExcludeUnversioned => _excludeUnversioned;

        public IObservableProperty<string> VersionExpression => _versionExpression;
    }
}
