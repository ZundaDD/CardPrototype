using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogTools;

/// <summary>
/// 用于格式化输出的工具
/// </summary>
public static class LogTool
{
    public static void Debug(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}[/color] [color=aqua]{member}:{line}[/color] -> {richText}", 
            "[color=green]Debug[/color]");
    }

    public static void Trace(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}[/color] [color=aqua]{member}:{line}[/color] -> {richText}",
            "[color=yellow]Trace[/color]");
    }

    public static void Error(string richText, [CallerLineNumber] int line = 0, [CallerMemberName] string member = "", [CallerFilePath] string path = "")
    {
        Print($"[color=skyblue]{path.Split('\\')[^1]}[/color] [color=aqua]{member}:{line}[/color] -> {richText}",
            "[color=red]Error[/color]");
    }

    private static void Print(string processedText, string levelText)
    {
        GD.PrintRich($"[{levelText}] {processedText}");
    }
}
