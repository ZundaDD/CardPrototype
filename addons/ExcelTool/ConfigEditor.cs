using Godot;
using LogTools;
using LitJson;
using System.Collections.Generic;

namespace ExcelTool;

/// <summary>
/// 整体的Config编辑器
/// </summary>
[GlobalClass, Tool]
public partial class ConfigEditor : BoxContainer
{
    
    private const string configPath = "res://addons/ExcelTool/config.json";

    private List<string> xlsxFiles = new();
    private ExcelToolConfig config;

    [Export] private OptionButton xlsxList;

    [Export] private ConfigMemberEdior xlsxPath;
    [Export] private ConfigMemberEdior scriptExportPath;
    [Export] private ConfigMemberEdior resourceExportPath;

    [Export] private LineEdit exportNamespace;
    [Export] private Button exportScriptButton;

    public override void _EnterTree()
    {
        exportNamespace.TextSubmitted += OnNamespaceChanged;
        exportScriptButton.Pressed += () => ExcelExporter.ExportScript(config, xlsxFiles);

        xlsxPath.OnMemberChanged += OnXlsxPathChanged;
        scriptExportPath.OnMemberChanged += (val) => { config.ScriptExportPath = val; SaveConfig(); };
        resourceExportPath.OnMemberChanged += (val) => { config.ResourceExportPath = val; SaveConfig(); };

        if (FileAccess.FileExists(configPath))
        {
            var json = FileAccess.GetFileAsString(configPath);
            config = JsonMapper.ToObject<ExcelToolConfig>(json) ?? new();
        }
        else { config = new(); }

        LoadConfig();
    }

    public override void _ExitTree()
    {
        SaveConfig();
        config = null;
    }

    private void OnNamespaceChanged(string text)
    {
        config.Namespace = text;
        SaveConfig();
    }

    /// <summary>
    /// Xlsx文件路径改变，将重新加载插件数据
    /// </summary>
    /// <param name="path">新路径</param>
    private void OnXlsxPathChanged(string path)
    {
        config.XlsxPath = path;
        xlsxList.Clear();
        xlsxFiles.Clear();

        foreach(var file in DirAccess.GetFilesAt(path))
        {
            if(file.EndsWith("xlsx"))
            {
                xlsxList.AddItem(file);
                xlsxFiles.Add(file);
            }
        }

        SaveConfig();
    }

    /// <summary>
    /// 将Config加载到UI中
    /// </summary>
    private void LoadConfig()
    {
        xlsxPath.InitMember(config.XlsxPath);
        scriptExportPath.InitMember(config.ScriptExportPath);
        resourceExportPath.InitMember(config.ResourceExportPath);

        exportNamespace.Text = config.Namespace;
        OnXlsxPathChanged(config.XlsxPath);
    }

    /// <summary>
    /// 将Config保存到文件中
    /// </summary>
    private void SaveConfig()
    {
        var json = JsonMapper.ToJson(config);
        using var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Write);

        if (file == null)
        {
            var error = FileAccess.GetOpenError();
            LogTool.Error($"无法打开文件: {configPath}, 错误代码: {error}");
            return;
        }

        file.StoreString(json);
        LogTool.Trace("ExcelToolConfig saved");
    }
}
