using System;
using System.Linq;

namespace SmartAddresser.Editor.Foundation
{
    internal static class CommandLineUtility
    {
        private static string[] _args;

        public static bool Contains(string argName)
        {
            return GetCommandLineArgs().Contains(argName);
        }

        public static string GetStringValue(string argName)
        {
            if (!TryGetStringValue(argName, out var value))
                throw new InvalidOperationException($"Argument {argName} or its value is not found.");

            return value;
        }

        public static bool TryGetStringValue(string argName, out string value)
        {
            var args = GetCommandLineArgs();
            var argNameIndex = Array.IndexOf(args, argName);
            var targetIndex = argNameIndex + 1;

            if (argNameIndex == -1 || args.Length <= targetIndex)
            {
                value = default;
                return false;
            }

            value = args[targetIndex];
            return true;
        }

        public static bool GetBoolValue(string argName)
        {
            return bool.Parse(GetStringValue(argName));
        }

        public static bool TryGetBoolValue(string argName, out bool value)
        {
            if (!TryGetStringValue(argName, out var strValue))
            {
                value = default;
                return false;
            }

            if (!bool.TryParse(strValue, out value))
                return false;

            return true;
        }

        public static int GetIntValue(string argName)
        {
            return int.Parse(GetStringValue(argName));
        }

        public static bool TryGetIntValue(string argName, out int value)
        {
            if (!TryGetStringValue(argName, out var strValue))
            {
                value = default;
                return false;
            }

            if (!int.TryParse(strValue, out value))
                return false;

            return true;
        }

        public static float GetFloatValue(string argName)
        {
            return float.Parse(GetStringValue(argName));
        }

        public static bool TryGetFloatValue(string argName, out float value)
        {
            if (!TryGetStringValue(argName, out var strValue))
            {
                value = default;
                return false;
            }

            if (!float.TryParse(strValue, out value))
                return false;

            return true;
        }

        private static string[] GetCommandLineArgs()
        {
            if (_args == null)
                _args = Environment.GetCommandLineArgs().ToArray();

            return _args;
        }
    }
}
