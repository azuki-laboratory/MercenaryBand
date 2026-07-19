using System;
using System.IO;
using System.Text.Json;
using Godot;

namespace MercenaryBand.Core;

public partial class SaveManager : Node
{
    private const string SaveExtension = ".json";
    private static string SaveDirectory =>
        System.IO.Path.Combine(
            OS.GetUserDataDir(),
            "saves"
        );

    public override void _Ready()
    {
        if (!System.IO.Directory.Exists(SaveDirectory))
            System.IO.Directory.CreateDirectory(SaveDirectory);

        GD.Print($"[SaveManager] Save directory: {SaveDirectory}");
    }

    public void Save<T>(string fileName, T data)
    {
        var path = GetSavePath(fileName);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        System.IO.File.WriteAllText(path, json);
        GD.Print($"[SaveManager] Saved: {fileName}");
    }

    public T? Load<T>(string fileName) where T : class
    {
        var path = GetSavePath(fileName);
        if (!System.IO.File.Exists(path))
        {
            GD.PushWarning($"[SaveManager] Save file not found: {fileName}");
            return null;
        }

        var json = System.IO.File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json);
    }

    public string[] ListSaveFiles()
    {
        if (!System.IO.Directory.Exists(SaveDirectory))
            return Array.Empty<string>();

        return System.IO.Directory.GetFiles(SaveDirectory, $"*{SaveExtension}");
    }

    public bool DeleteSave(string fileName)
    {
        var path = GetSavePath(fileName);
        if (!System.IO.File.Exists(path))
            return false;

        System.IO.File.Delete(path);
        GD.Print($"[SaveManager] Deleted: {fileName}");
        return true;
    }

    private static string GetSavePath(string fileName) =>
        System.IO.Path.Combine(SaveDirectory, fileName + SaveExtension);
}
