using Microsoft.Win32;

namespace Launcher
{
    public static class RegUtils
    {
        internal static string ReadString(this RegistryKey root, string path, string value)
        {
            try
            {
                var key = root.OpenSubKey(path);
                var result = (string)key?.GetValue(value);
                return result ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
