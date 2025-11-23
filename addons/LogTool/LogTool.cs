using Godot;
using System.Runtime.CompilerServices;

namespace LogTools;

/// <summary>
/// 用于格式化输出的工具
/// </summary>
public static class LogTool
{
    /// <summary>
    /// 输出Debug信息
    /// </summary>
    public static void Debug(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}[/color] [color=aqua]{member}:{line}[/color] -> {richText}", 
            "[color=green]Debug[/color]");
    }
    
    /// <summary>
    /// 输出溯源信息
    /// </summary>
    public static void Trace(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}:{line}[/color] [color=aqua]{member}[/color] -> {richText}",
            "[color=yellow]Trace[/color]");
    }

    /// <summary>
    /// 输出警告信息
    /// </summary>
    public static void Error(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}:{line}[/color] [color=aqua]{member}[/color] -> {richText}",
            "[color=red]Error[/color]");
    }

    private static void Print(string processedText, string levelText)
    {
        GD.PrintRich($"[{levelText}]{processedText}");
    }
}
