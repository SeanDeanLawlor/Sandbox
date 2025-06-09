using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PythonServerLauncher : MonoBehaviour
{
    void Start()
    {
        string pythonPath = "python"; // Use full path if needed
        string scriptPath = Path.Combine(Application.dataPath, "../Python/zx_flask_server.py");

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("❌ Python server script not found: " + scriptPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        try
        {
            Process.Start(startInfo);
            UnityEngine.Debug.Log("✅ Python server launched.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("❌ Failed to launch Python server: " + e.Message);
        }
    }
}