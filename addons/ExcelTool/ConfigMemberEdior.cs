#if TOOLS
using Godot;
using System;

namespace ExcelTool;

/// <summary>
/// Config的单个字段编辑器
/// </summary>
[Tool]
public partial class ConfigMemberEdior : Control
{
    [Export] private LineEdit textDisplay;
    [Export] private Button browseButton;

    public Action<string> OnMemberChanged;

    private EditorFileDialog fileDialog;

    public override void _Ready()
    {
        try
        {
            if (browseButton != null)
            {
                browseButton = null;
                browseButton.Pressed += Browse;
            }
        }
        catch { }
    }


    public void InitMember(string value)
    {
        if(textDisplay != null) textDisplay.Text = value;
    }

    private void Browse()
    {
        if (fileDialog == null || !IsInstanceValid(fileDialog))
        {
            fileDialog = new EditorFileDialog();
            fileDialog.FileMode = EditorFileDialog.FileModeEnum.OpenDir;
            fileDialog.Access = EditorFileDialog.AccessEnum.Resources;

            AddChild(fileDialog);
            fileDialog.DirSelected += (val) =>
            {
                textDisplay.Text = val;
                OnMemberChanged?.Invoke(val);
            };
        }

        fileDialog.PopupCentered(new Vector2I(600, 800));
    }

    public override void _ExitTree()
    {
        if (fileDialog != null && IsInstanceValid(fileDialog)) fileDialog.QueueFree();
    }
}
#endif