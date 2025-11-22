using ExcelDataReader;
using Godot;
using LogTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ExcelTool;

public static class ExcelExporter
{
    public static void ExportScript(ExcelToolConfig config, List<string> files)
    {
        foreach (var xlsx in files)
        {
            var filePath = Path.Combine(config.XlsxPath, xlsx);
            var structure = ReadStructure(filePath);
            
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var code = GenerateCode(fileName, config.Namespace, structure);

            var genFilePath = Path.Combine(config.ScriptExportPath, $"{fileName}.cs");
            SaveCode(genFilePath, code);
        }
    }

    private static string GenerateCode(string className,string @namespace, List<(string name, string type)> sources)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($@"
/*
此文件根据Excel文件自动生成，请不要手动修改。
This file is auto-generated from Excel. Do not modify manually.
*/
using Godot;
using System;
using System.Collections.Generic;

namespace {@namespace};

[GlobalClass]
public partial class {className} : Resource
{{
");

        foreach (var col in sources)
        {
            sb.AppendLine($"\t[Export] public {col.type} {col.name} {{ get; set; }}\n");
        }

        sb.AppendLine($@"
}}");

        return sb.ToString(); 
    }

    private static void SaveCode(string filePath, string code)
    {
        using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);
        if (file == null)
        {
            LogTool.Error($"Failed to write to {filePath}");
            return;
        }
        file.StoreString(code);
        LogTool.Trace($"Success to generate file {filePath}");
    }

    private static List<(string name, string type)> ReadStructure(string filePath)
    {
        var result = new List<(string Name, string Type)>();
        try
        {
            using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);

            if (file == null)
            {
                LogTool.Error($"Failed to open file: {filePath}. Error: {Godot.FileAccess.GetOpenError()}");
                return result;
            }

            byte[] buffer = file.GetBuffer((long)file.GetLength());

            using var stream = new MemoryStream(buffer);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            if (!reader.Read()) return null;
            var fieldNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                fieldNames.Add(reader.GetValue(i)?.ToString()?.Trim());
            }

            if (!reader.Read()) return null;
            var fieldTypes = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                fieldTypes.Add(reader.GetValue(i)?.ToString()?.Trim());
            }

            for (int i = 0; i < fieldNames.Count; i++)
            {
                string name = fieldNames[i];
                string type = i < fieldTypes.Count ? fieldTypes[i] : null;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) continue;
                result.Add((name, type));
            }
        }
        catch (Exception ex)
        {
            LogTool.Error($"Excel Read Error: {ex.Message}");
        }

        return result;
    }
}
