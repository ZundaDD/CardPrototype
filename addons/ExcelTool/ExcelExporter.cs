#if TOOLS
using ParseTool;
using Godot;
using LogTools;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FAcc = System.IO.FileAccess;

namespace ExcelTool;

public static class ExcelExporter
{
    public static void ExportResource(ExcelToolConfig config, string file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file);
        var filePath = Path.Combine(config.XlsxPath, file);

        // 覆盖路径
        var exportPath = Path.Combine(config.ResourceExportPath, fileName);
        {
            string globalPath = ProjectSettings.GlobalizePath(exportPath);
            if(Path.Exists(globalPath)) Directory.Delete(globalPath, true);
            Directory.CreateDirectory(globalPath);
        }

        // 通过反射获取单元Export函数
        Type targetType = Type.GetType($"{config.Namespace}.{fileName}");
        if (targetType == null)
        {
            LogTool.Error($"Class {config.Namespace}.{fileName} not generated correctly");
            return;
        }

        MethodInfo methodInfo = targetType.GetMethod("Export", BindingFlags.Public | BindingFlags.Static);
        if (methodInfo == null)
        {
            LogTool.Error($"Method {config.Namespace}.{fileName}.Export not generated correctly");
            return;
        }
        var exportDelegate = methodInfo.CreateDelegate<Action<string, Dictionary<string, string>>>();

        var rows = ReadExcelRows(filePath);
        foreach (var row in rows)
        {
            //调用xxx.Export(exportPath, row);
            exportDelegate(exportPath, row);
        }
    }

    public static IEnumerable<Dictionary<string, string>> ReadExcelRows(string path)
    {
        var globalPath = ProjectSettings.GlobalizePath(path);
        using var stream = new FileStream(globalPath, FileMode.Open, FAcc.Read, FileShare.ReadWrite);

        var rows = stream.Query(useHeaderRow: true);
        using var enumerator = rows.GetEnumerator();

        if (enumerator.MoveNext()) { }

        while (enumerator.MoveNext())
        {
            var rowData = (IDictionary<string, object>)enumerator.Current;

            var rowDict = new Dictionary<string, string>();
            bool isEmptyRow = true;

            foreach (var kvp in rowData)
            {
                string val = kvp.Value?.ToString()?.Trim();

                if (!string.IsNullOrEmpty(val))
                {
                    rowDict[kvp.Key] = val;
                    isEmptyRow = false;
                }
            }

            if (!isEmptyRow) yield return rowDict;
        }
    }

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
        StringBuilder sb = new ();
        StringBuilder parseBuilder = new();
        sb.AppendLine($@"/*
此文件根据Excel文件自动生成，请不要手动修改。
This file is auto-generated from Excel. Do not modify manually.
*/
using Godot;
using ParseTool;
using System;
using System.Collections.Generic;

namespace {@namespace};

[GlobalClass]
public partial class {className} : Resource
{{");

        foreach (var col in sources)
        {
            sb.AppendLine($"\t[Export] public {col.type} {col.name} {{ get; set; }}\n");
            parseBuilder.AppendLine($"\t\tthis.{col.name} = Parser.Parse{Parser.GetParseSuffix(col.type)}(row, \"{col.name}\");");
        }

        sb.AppendLine($@"#if TOOLS
    public void Parse(Dictionary<string, string> row)
    {{

{parseBuilder.ToString()}
    }}

    public static void Export(string exportPath, Dictionary<string, string> row)
    {{
        string id = Parser.ParseString(row, ""ID"");
        if (string.IsNullOrEmpty(id)) return;

        var instance = new {className}();
        instance.Parse(row);
        Godot.ResourceSaver.Save(instance, $""{{exportPath}}/{{id}}.tres"");
    }}
#endif
}}
");

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
        LogTool.Trace($"Success to generate file [color=green]{filePath}[/color]");
    }

    private static List<(string name, string type)> ReadStructure(string filePath)
    {
        var result = new List<(string Name, string Type)>();
        try
        {
            var globalPath = ProjectSettings.GlobalizePath(filePath);
            using var stream = new FileStream(globalPath, FileMode.Open, FAcc.Read, FileShare.ReadWrite);

            var rows = stream.Query(useHeaderRow: false);

            using var enumerator = rows.GetEnumerator();

            if (!enumerator.MoveNext()) return null;

            var row1Data = enumerator.Current as IDictionary<string, object>;
            var fieldNames = row1Data.Values
                .Select(v => v?.ToString()?.Trim())
                .ToList();

            if (!enumerator.MoveNext()) return null;

            var row2Data = enumerator.Current as IDictionary<string, object>;
            var fieldTypes = row2Data.Values
                .Select(v => v?.ToString()?.Trim())
                .ToList();

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
#endif