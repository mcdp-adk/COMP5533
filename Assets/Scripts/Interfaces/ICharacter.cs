using System;

public interface ICharacter
{
    #region Properties

    /// <summary>
    /// 角色的最大生命值
    /// </summary>
    int MaxHealth { get; set; }

    /// <summary>
    /// 角色当前的生命值
    /// </summary>
    int Health { get; }

    #endregion

    #region Methods

    /// <summary>
    /// 对角色造成伤害
    /// </summary>
    /// <param name="damageAmount"></param>
    void CauseDamage(int damageAmount);

    /// <summary>
    /// 治疗角色
    /// </summary>
    /// <param name="healAmount"></param>
    void Heal(int healAmount);

    #endregion

    #region Events

    /// <summary>
    /// 角色生命值改变时触发
    /// </summary>
    event Action OnHealthChanged;

    /// <summary>
    /// 角色生命值增加时触发
    /// </summary>
    event Action OnHealthIncreased;

    /// <summary>
    /// 角色生命值减少时触发
    /// </summary>
    event Action OnHealthDecreased;

    /// <summary>
    /// 角色死亡时触发
    /// </summary>
    event Action OnCharacterDeath;

    #endregion
}
