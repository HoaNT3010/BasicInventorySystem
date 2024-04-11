using System;

[Serializable]
public class ItemUI
{
    public int id = -1;
}

[Serializable]
public class EquipmentItemUI : ItemUI
{
    public EquipmentBuff[] buffs;
    public EquipmentType equipmentType;
}

/// <summary>
/// Implementing Factory Pattern for generating different types of ItemUI object.
/// </summary>
public static class ItemUIFactory
{
    public static ItemUI CreateItemUI(ItemObject itemObject)
    {
        if (itemObject is EquipmentItem equipment)
        {
            EquipmentItemUI equipmentItemUI = new EquipmentItemUI();
            equipmentItemUI.id = equipment.data.id;
            equipmentItemUI.equipmentType = equipment.equipmentType;
            equipmentItemUI.buffs = new EquipmentBuff[equipment.buffs.Length];
            for (int i = 0; i < equipmentItemUI.buffs.Length; i++)
            {
                equipmentItemUI.buffs[i] = new EquipmentBuff(equipment.buffs[i].minValue, equipment.buffs[i].maxValue);
                equipmentItemUI.buffs[i].attribute = equipment.buffs[i].attribute;
            }
            return equipmentItemUI;
        }
        else
        {
            ItemUI itemUI = new ItemUI();
            itemUI.id = itemObject.data.id;
            return itemUI;
        }
    }

    public static ItemUI CreateEmptyItemUI()
    {
        ItemUI itemUI = new ItemUI();
        itemUI.id = -1;
        return itemUI;
    }
}
