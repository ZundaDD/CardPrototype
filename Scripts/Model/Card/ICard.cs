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
    public Guid InstanceId { get; }

    /// <summary>
    /// 卡牌名称的翻译键，直接显示
    /// </summary>
    public string NameKey { get; }

    /// <summary>
    /// 卡牌描述的翻译键，通过一定方法加工生成最终显示的描述
    /// </summary>
    public string DescKey { get; }

    /// <summary>
    /// 卡牌的费用，低于0的费用保留用于特殊设计
    /// </summary>
    public int Cost { get; }

    /// <summary>
    /// 卡牌的选择目标
    /// </summary>
    public TargetType Target { get; }

    /// <summary>
    /// 卡牌的标签，用于筛选，关键字段(如Target)的值一定会被添加到标签中，但不应合并，因为关键字段一定要从枚举中选中值，而标签可以不写。
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// 深拷贝一份卡牌
    /// </summary>
    /// <returns>新的卡牌</returns>
    ICard Clone();
}
