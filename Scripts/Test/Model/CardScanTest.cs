using CardPrototype.Model;
using CardPrototype.Service;
using Godot;
using LogTools;
using System;
using System.IO;

namespace CardPrototype.Test;

public partial class CardScanTest : Node
{
    private const string dirPath = "res://Assets/Generated/CardMeta";

    public override void _Ready()
    {
        LogTool.Trace("开始卡牌扫描测试");

        ITranslateService translator = new TranslateService();
        CardMetaDb db = new ();

        foreach(var file in DirAccess.GetFilesAt(dirPath))
        {
            string id = Path.GetFileNameWithoutExtension(file);
            var meta = db[id];

            if (meta == null) LogTool.Error($"Failed to parse cardmeta {id}");
            else LogTool.Debug($"{id} : {meta.Target} - {translator.Translate(meta.NameKey)}");
        }
    }
}
