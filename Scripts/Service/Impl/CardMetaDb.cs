using CardPrototype.Model;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FAcc = Godot.FileAccess;

namespace CardPrototype.Service;

public class CardMetaDb
{
    private readonly Dictionary<string, CardMeta> cards = new();

    private const string dirPath = "res://Assets/Generated/CardMeta";

    public CardMeta this[string id]
    {
        get
        {
            if (cards.ContainsKey(id)) return cards[id];

            var targetPath = Path.Combine(dirPath, $"{id}.tres");
            if (!FAcc.FileExists(targetPath)) return null;

            var meta = ResourceLoader.Load<CardMeta>(targetPath);
            if (meta == null) return null;

            cards[id] = meta;
            return meta;
        }
    }
}
