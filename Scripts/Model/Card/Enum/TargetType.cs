namespace CardPrototype.Model;

/// <summary>
/// 目标选择类型
/// </summary>
public enum TargetType
{
    /// <summary>
    /// 无目标
    /// </summary>
    None,
    /// <summary>
    /// 选择单个非同阵营方
    /// </summary>
    SingleEnemy,
    /// <summary>
    /// 选择单个同阵营方
    /// </summary>
    SingleAlly,
}
