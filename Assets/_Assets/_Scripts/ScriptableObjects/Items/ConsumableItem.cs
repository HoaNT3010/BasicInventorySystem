using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory System/Items/Consumable")]
public class ConsumableItem : ItemObject
{
    /// <summary>
    /// The amount of health points restored when consumed.
    /// </summary>
    public int restoreHealthValue;
    /// <summary>
    /// The amount of mana points restored when consumed.
    /// </summary>
    public int restoreManaValue;
    private void Awake()
    {
        type = ItemType.Consumable;
        stackable = true;
        destroyable = true;
    }
}
