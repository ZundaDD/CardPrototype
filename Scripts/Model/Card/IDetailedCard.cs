namespace CardPrototype.Model;

/// <summary>
/// 具体的卡牌接口，在此处添加额外功能
/// </summary>
public interface IDetailedCard : ICard
{
    /// <summary>
    /// 使用完后是否销毁
    /// </summary>
    public bool Exhausted { get; }

    /// <summary>
    /// 卡牌稀有度
    /// </summary>
    public CardRarity Rarity { get; }

    /// <summary>
    /// 卡牌类型
    /// </summary>
    public CardType Type { get; }
}
