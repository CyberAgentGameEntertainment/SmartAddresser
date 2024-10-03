using System;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Models.Shared.AssetGroups
{
    [Serializable]
    public class TypeReference
    {
        [SerializeField] private string _name;
        [SerializeField] private string _fullName;
        [SerializeField] private string _assemblyQualifiedName;

        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        public string FullName
        {
            get => _fullName;
            internal set => _fullName = value;
        }

        public string AssemblyQualifiedName
        {
            get => _assemblyQualifiedName;
            internal set => _assemblyQualifiedName = value;
        }

        public static TypeReference Create(Type type)
        {
            var instance = new TypeReference
            {
                _name = type.Name,
                _fullName = type.FullName,
                _assemblyQualifiedName = type.AssemblyQualifiedName
            };
            return instance;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(_assemblyQualifiedName);
        }
    }
}
