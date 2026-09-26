using System;
using System.Diagnostics;
using System.Linq;

public static class ADB
{
    public static string RunCommand(string adbArguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "adb",                   
            Arguments = adbArguments,
            RedirectStandardOutput = true,      
            RedirectStandardError = true,       
            UseShellExecute = false,            
            CreateNoWindow = true               
        };

        try
        {
            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();
                GrabApi.logs.Add(output);
                return output;
            }
        }
        catch (Exception ex)
        {
            GrabApi.logs.Add($"Failed to start ADB: {ex.Message}");
            return "";
        }
    }

    public static bool HasConnectedDevices()
    {
        string output = RunCommand("devices");

        if (string.IsNullOrWhiteSpace(output))
        {
            return false;
        }

        string[] lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int deviceCount = lines.Count(line => line.Contains("\tdevice"));

        return deviceCount > 0;
    }
}
