using System;

namespace SmartAddresser.Editor.Core.Models.Shared
{
    public static class IdentifierFactory
    {
        public static string Create()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
