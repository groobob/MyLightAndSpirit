using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public static class BlockEnumGenerator
{
    private const string enumName = "BlockType";
    private const string outputPath = "Assets/EvanThings/BlockType.cs";

    public static void GenerateBlockEnum()
    {
        var allBlocks = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(InteractableBlock).IsAssignableFrom(t)
                     && t != typeof(InteractableBlock)
                     && !t.IsAbstract)
            .ToArray();

        Debug.Log($"Found {allBlocks.Length} subclasses of InteractableBlock.");

        string directory = Path.GetDirectoryName(outputPath);

        if (!Directory.Exists(directory))
        {
            Debug.Log($"Directory not found, creating: {directory}");
            Directory.CreateDirectory(directory);
        }

        using (StreamWriter writer = new StreamWriter(outputPath, false))
        {
            writer.WriteLine("// Auto-generated. Do not edit manually.");
            writer.WriteLine("public enum BlockType");
            writer.WriteLine("{");
            writer.WriteLine("    emptySpace,");
            writer.WriteLine("    repeat,");
            foreach (var type in allBlocks)
                writer.WriteLine($"    {type.Name},");
            writer.WriteLine("}");
        }

        Debug.Log($"✅ Enum regenerated at {outputPath}");
    }
}
