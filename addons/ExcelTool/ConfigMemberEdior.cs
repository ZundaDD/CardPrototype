using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public override void _EnterTree()
    {
        browseButton.Pressed += Browse;
        OnMemberChanged += (val) => textDisplay.Text = val;
    }

    public void InitMember(string value)
    {
        textDisplay.Text = value;
    }

    private void Browse()
    {
        var dirDialog = new EditorFileDialog();
        dirDialog.FileMode = EditorFileDialog.FileModeEnum.OpenDir;
        dirDialog.Access = EditorFileDialog.AccessEnum.Resources;

        AddChild(dirDialog);
        dirDialog.DirSelected += (val) =>
        {
            OnMemberChanged?.Invoke(val);
            dirDialog.QueueFree();
        };

        dirDialog.PopupCentered(new Vector2I(600, 800));
    }
}
