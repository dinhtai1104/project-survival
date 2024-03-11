#if UNITY_EDITOR && UNITY_ANDROID
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AndroidSymbolShrinkerPostProcessor: IPostprocessBuildWithReport
{
    private static string LastSymbolToShrinkLocation = nameof(LastSymbolToShrinkLocation);

    class ProcessResult
    {
        internal int ExitCode { get; }
        internal string StdOut { get; }
        internal string StdErr { get; }

        internal bool Failure => ExitCode != 0;
        internal ProcessResult(int exitCode, string stdOut, string stdErr)
        {
            ExitCode = exitCode;
            StdOut = stdOut;
            StdErr = stdErr;
        }

        public override string ToString()
        {
            return $"Exit Code: {ExitCode}\nStdOut:\n{StdOut}\nStdErr:\n{StdErr}";
        }
    }

    public int callbackOrder { get; } = 0;
    
    public void OnPostprocessBuild(BuildReport report)
    {
        if (!EditorUserBuildSettings.androidCreateSymbolsZip) return;

        var outputFilePath = report.files.FirstOrDefault(file =>
            file.path.Substring(Math.Max(0, file.path.Length - 4)) ==
            (EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk")).path;

        if (string.IsNullOrWhiteSpace(outputFilePath)) return;

        var outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputFilePath);
        var outputFileParentDirectoryPath = Path.GetDirectoryName(outputFilePath);

        var symbolFileExistRegex = new Regex($"^{outputFileNameWithoutExtension}.+\\.symbols\\.zip$");

        var symbolFilePath = Directory
            .GetFiles(outputFileParentDirectoryPath ?? string.Empty, "*zip", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(path => symbolFileExistRegex.IsMatch(Path.GetFileName(path)));

        if (string.IsNullOrWhiteSpace(symbolFilePath))
        {
            LogError("Symbol file not found with given format!");
            return;
        }

        ShrinkSymbols(symbolFilePath);
        // File.Delete(symbolFilePath);
    }

    private static void Log(string message)
    {
        UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, message);
    }

    private static void LogError(string message)
    {
        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, message);
    }

    private static ProcessResult RunProcess(string workingDirectory, string fileName, string args)
    {
        Log($"Executing {fileName} {args} (Working Directory: {workingDirectory}");
        Process process = new Process();
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.WorkingDirectory = workingDirectory;
        process.StartInfo.CreateNoWindow = true;
        var output = new StringBuilder();
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.AppendLine(e.Data);
            }
        });

        var error = new StringBuilder();
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.AppendLine(e.Data);
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        Log($"{fileName} exited with {process.ExitCode}");
        return new ProcessResult(process.ExitCode, output.ToString(), error.ToString());
    }

    private static void Cleanup(string path)
    {
        if (Directory.Exists(path))
        {
            Log($"Delete {path}");
            Directory.Delete(path, true);
        }
        if (File.Exists(path))
        {
            Log($"Delete {path}");
            File.Delete(path);
        }
    }

    [MenuItem("Tools/Android Symbols/Shrink")]
    public static void ShrinkSymbols()
    {
        var location = EditorPrefs.GetString(LastSymbolToShrinkLocation, Path.Combine(Application.dataPath, ".."));
        location = EditorUtility.OpenFilePanel(
            "Open Android Symbol Package to shrink",
            location, "*.zip");
        ShrinkSymbols(location);
    }

    private static void ShrinkSymbols(string location)
    {
        if (string.IsNullOrEmpty(location))
            return;

        var targetDirectory = Path.GetDirectoryName(location);
        var intermediatePath = Path.Combine(targetDirectory, "TempShrink");
        var newZip = Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(location) + ".shrink.zip");
        EditorPrefs.SetString(LastSymbolToShrinkLocation, targetDirectory);

        var zipFileName = Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "Tools", "7z"));
        if (Application.platform == RuntimePlatform.WindowsEditor)
            zipFileName += ".exe";

        if(Application.platform == RuntimePlatform.OSXEditor)
            zipFileName += "a";
        
        if (!File.Exists(zipFileName))
            throw new Exception($"Failed to locate {zipFileName}");

        Cleanup(intermediatePath);
        Cleanup(newZip);
        var result = RunProcess(targetDirectory, zipFileName, $"x -o\"{intermediatePath}\" \"{location}\"");
        if (result.Failure)
            throw new Exception(result.ToString());

        EditorUtility.DisplayProgressBar("Shrinking symbols", "Deleting/Renaming/Compressing symbol files", 0.5f);
        var files = Directory.GetFiles(intermediatePath, "*.*", SearchOption.AllDirectories);
        var symSo = ".sym.so";
        foreach (var file in files)
        {
            if (file.EndsWith(".dbg.so"))
                Cleanup(file);
            if (file.EndsWith(symSo))
            {
                var fileSO = file.Substring(0, file.Length - symSo.Length) + ".so";
                Log($"Rename {file} --> {fileSO}");
                File.Move(file, fileSO);
            }
        }

        result = RunProcess(intermediatePath, zipFileName, $"a -tzip \"{newZip}\"");
        EditorUtility.ClearProgressBar();
        if (result.Failure)
            throw new Exception(result.ToString());

        Cleanup(intermediatePath);

        Log($"New small symbol package: {newZip}");
        EditorUtility.RevealInFinder(newZip);
    }
}
#endif