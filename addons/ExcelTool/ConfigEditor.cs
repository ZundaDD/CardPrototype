#if TOOLS
using Godot;
using LitJson;
using LogTools;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExcelTool;

/// <summary>
/// 整体的Config编辑器
/// </summary>
[Tool]
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
    [Export] private Button exportResourceButton;
    
    public override void _Ready()
    {
        if (exportScriptButton == null || exportResourceButton == null) return;

        exportNamespace.TextSubmitted += OnNamespaceChanged;

        exportScriptButton.Pressed += OnExportScriptPressed;

        exportResourceButton.Pressed += OnExportResourcePressed;

        xlsxPath.OnMemberChanged -= OnXlsxPathChanged;
        xlsxPath.OnMemberChanged += OnXlsxPathChanged;

        scriptExportPath.OnMemberChanged = (val) => { config.ScriptExportPath = val; SaveConfig(config); };
        resourceExportPath.OnMemberChanged = (val) => { config.ResourceExportPath = val; SaveConfig(config); };

        LoadConfig();
    }

    private void OnExportScriptPressed()
    {
        LogTool.Trace("Exporting scripts");
        if (config == null) LoadConfig();
        ExcelExporter.ExportScript(config, xlsxFiles);
    }

    private void OnExportResourcePressed()
    {
        LogTool.Trace("Exporting Resources");
        if (config == null) LoadConfig();
        ExcelExporter.ExportResource(config, xlsxList.GetItemText(xlsxList.Selected));
    }

    private void OnNamespaceChanged(string text)
    {
        config.Namespace = text;
        SaveConfig(config);
    }

    /// <summary>
    /// Xlsx文件路径改变，将重新加载插件数据
    /// </summary>
    /// <param name="path">新路径</param>
    private void OnXlsxPathChanged(string path)
    {
        config.XlsxPath = path;
        RefreshFileList(path);
        SaveConfig(config);
    }

    private void RefreshFileList(string path)
    {
        xlsxList.Clear();
        xlsxFiles.Clear();

        foreach (var file in DirAccess.GetFilesAt(path))
        {
            if (file.EndsWith("xlsx"))
            {
                xlsxList.AddItem(file);
                xlsxFiles.Add(file);
            }
        }
    }

    /// <summary>
    /// 将Config加载到UI中
    /// </summary>
    private void LoadConfig()
    {
        if (config == null)
        {
            if (FileAccess.FileExists(configPath))
            {
                var json = FileAccess.GetFileAsString(configPath);
                config = JsonMapper.ToObject<ExcelToolConfig>(json) ?? new();
            }
            else { config = new(); }
        }

        if (xlsxPath != null) xlsxPath.InitMember(config.XlsxPath);
        if (scriptExportPath != null) scriptExportPath.InitMember(config.ScriptExportPath);
        if (resourceExportPath != null) resourceExportPath.InitMember(config.ResourceExportPath);
        if (exportNamespace != null) exportNamespace.Text = config.Namespace;

        RefreshFileList(config.XlsxPath);
    }

    /// <summary>
    /// 将Config保存到文件中
    /// </summary>
    private static void SaveConfig(ExcelToolConfig config)
    {
        try
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
        catch(Exception ex)
        {
            LogTool.Error($"保存时出错：{ex.Message}");
        }
    }

}
#endif