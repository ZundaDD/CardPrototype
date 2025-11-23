#if TOOLS
using Godot;

namespace ExcelTool;

/// <summary>
/// 单个表的编辑器
/// </summary>
public partial class ConfigXlsxEditor : Control
{
    
    private string file;

    public override void _EnterTree()
    {
    }

    public void BindFile(string file)
    {
        this.file = file;
    }

}
#endif