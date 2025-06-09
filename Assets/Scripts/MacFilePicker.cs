// Assets/Scripts/Utilities/MacFilePicker.cs
using System.Diagnostics;
using System.IO;

public static class MacFilePicker
{
    public static string OpenFile()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/osascript",
                Arguments = "-e 'POSIX path of (choose file with prompt \"Select a ZX JSON file\")'",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            process.Start();
            string result = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            return File.Exists(result) ? result : null;
        }
        catch
        {
            return null;
        }
#else
        return null; // Not supported on other platforms
#endif
    }
}