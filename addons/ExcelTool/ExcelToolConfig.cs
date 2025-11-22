using System.Collections.Generic;
using System.IO;

namespace ExcelTool;

public class ExcelToolConfig
{
    public string Namespace { get; set; } = "AutoGen";

    public string XlsxPath { get; set; } = "res://Assets/Xlsx";

    public string ScriptExportPath { get; set; } = $"res://Scripts/Generated";

    public string ResourceExportPath { get; set; } = $"res://Assets/Generated";
}