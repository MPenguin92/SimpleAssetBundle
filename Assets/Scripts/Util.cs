// Util.cs
// Created by Cui Lingzhi
// on 2024 - 02 - 17

using System.IO;

public static class Util
{
    public const string ASSET_BUNDLE_OUTPUT = "Bundles/";
    public const string ASSET_BUNDLE_UNDER_SA_PATH = "Bundles/";
    public const string MANIFEST_NAME = "MyBundleManifest.asset";
    
    
    public static void DeleteFolder(string path)
    {
        if (!Directory.Exists(path))
            return;
        foreach (var file in Directory.GetFiles(path))
        {
            var fileName = Path.GetFileName(file);
            File.Delete(fileName);
        }

        foreach (var folder in Directory.GetDirectories(path))
        {
            DeleteFolder(folder);
        }
    }

    public static void CopyFolder(string sourceFolderPath, string destFolderPath)
    {
        if (!Directory.Exists(destFolderPath))
            Directory.CreateDirectory(destFolderPath);

        foreach (var file in Directory.GetFiles(sourceFolderPath))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destFolderPath, fileName), true);
        }

        foreach (var folder in Directory.GetDirectories(sourceFolderPath))
        {
            var newDestinationFolderPath = Path.Combine(destFolderPath, Path.GetFileName(folder));
            CopyFolder(folder, newDestinationFolderPath);
        }
    }
}