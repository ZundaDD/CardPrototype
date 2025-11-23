/*
此文件根据Excel文件自动生成，请不要手动修改。
This file is auto-generated from Excel. Do not modify manually.
*/
using Godot;
using ParseTool;
using System;
using System.Collections.Generic;

namespace CardPrototype.Model;

[GlobalClass]
public partial class CardMeta : Resource
{
	[Export] public string ID { get; set; }

	[Export] public string NameKey { get; set; }

	[Export] public string DescKey { get; set; }

	[Export] public int Cost { get; set; }

	[Export] public TargetType Target { get; set; }

#if TOOLS
    public void Parse(Dictionary<string, string> row)
    {

		this.ID = Parser.ParseString(row, "ID");
		this.NameKey = Parser.ParseString(row, "NameKey");
		this.DescKey = Parser.ParseString(row, "DescKey");
		this.Cost = Parser.ParseInt(row, "Cost");
		this.Target = Parser.ParseEnum<TargetType>(row, "Target");

    }

    public static void Export(string exportPath, Dictionary<string, string> row)
    {
        string id = Parser.ParseString(row, "ID");
        if (string.IsNullOrEmpty(id)) return;

        var instance = new CardMeta();
        instance.Parse(row);
        Godot.ResourceSaver.Save(instance, $"{exportPath}/{id}.tres");
    }
#endif
}

