using Godot;
using System;
using System.Collections.Generic;

namespace CardPrototype.Model;

/// <summary>
/// 卡牌接口，表征一个标准的卡牌对象
/// </summary>
public interface ICard
{
    /// <summary>
    /// 卡牌的唯一ID，用于区分同名卡牌
    /// </summary>
    Guid InstanceId { get; }

    /// <summary>
    /// 卡牌的原始属性
    /// </summary>
    CardMeta Meta { get; }

    /// <summary>
    /// 卡牌实时费用
    /// </summary>
    public int CurrentCost { get; }

    /// <summary>
    /// 卡牌的标签，用于筛选，关键字段(如Target)的值一定会被添加到标签中，但不应合并，因为关键字段一定要从枚举中选中值，而标签可以不写。
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// 深拷贝一份卡牌，共享Meta
    /// </summary>
    /// <returns>新的卡牌</returns>
    ICard Clone();
}
