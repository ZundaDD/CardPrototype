#if TOOLS
using Godot;

namespace ExcelTool;

[Tool]
public partial class ExcelDock : EditorPlugin
{
    private Control dock = null;
    
    public override void _EnterTree()
    {
        dock = GD.Load<PackedScene>("res://addons/ExcelTool/ExcelDock.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.RightUl, dock);
    }

    public override void _ExitTree()
    {
        RemoveControlFromDocks(dock);
        dock.QueueFree();
        dock = null;
    }
}
#endif