using System.Diagnostics;
using System.Reflection;

namespace ZimraEGS.Helpers
{
    public static class VersionHelper
    {
        public static long GetNumberOnly(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;
            string numberString = new string(input.Where(char.IsDigit).ToArray());
            return long.TryParse(numberString, out long result) ? result : 0;
        }
        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return "v" + fvi.FileVersion;
        }

        public static long GetLongVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return GetNumberOnly(fvi.FileVersion);
        }
    }
}