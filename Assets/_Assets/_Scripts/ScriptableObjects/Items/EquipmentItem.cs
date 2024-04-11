using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Inventory System/Items/Equipment")]
public class EquipmentItem : ItemObject
{
    public EquipmentType equipmentType;
    public EquipmentBuff[] buffs;
    private void Awake()
    {
        type = ItemType.Equipment;
        stackable = false;
        destroyable = true;
    }
}

public enum EquipmentType
{
    Helmet,
    Breastplate,
    Boots,
    Weapon,
    Shield,
}

public enum EquipmentAttribute
{
    Strength,   // physical attack
    Armor,    // Physical defense
    MaxHealth,
    CriticalChance,
    CriticalDamage,
    Evasion,
    Precision,
    Luck,
    Intelligent,    // Magical attack
    Resistance, // Magical defense
    Speed,
}

[Serializable]
public class EquipmentBuff : IModifier
{
    public EquipmentAttribute attribute;
    public int value;
    public int minValue;
    public int maxValue;

    public EquipmentBuff(int minValue, int maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        GenerateAttributeValue();
    }

    public void GenerateAttributeValue()
    {
        value = UnityEngine.Random.Range(minValue, maxValue);
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }
}
