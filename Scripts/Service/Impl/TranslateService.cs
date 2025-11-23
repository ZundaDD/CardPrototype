using Godot;
using LogTools;
using System;
using System.Collections.Generic;

namespace CardPrototype.Service;

public partial class TranslateService : ITranslateService
{

    public string Translate(string key, params object[] args)
    {
        string rawText = TranslationServer.Translate(key);

        if (args == null || args.Length == 0) return rawText;
        
        try
        {
            return string.Format(rawText, args);
        }
        catch (System.FormatException)
        {
            LogTool.Error($"Format failed for key: {key}");
            return rawText;
        }
    }
}
