using System;
using UnityEngine;

/// <summary>
/// Base scriptable object class for items.
/// </summary>
public abstract class ItemObject : ScriptableObject
{
    /// <summary>
    /// Containing data of the item.
    /// </summary>
    [SerializeReference]
    public ItemUI data = ItemUIFactory.CreateEmptyItemUI();
    /// <summary>
    /// Displayed sprite of the item.
    /// </summary>
    public Sprite itemSprite;
    /// <summary>
    /// Displayed model of the item.
    /// </summary>
    public GameObject itemModel;
    /// <summary>
    /// Type of the item.
    /// </summary>
    public ItemType type;
    /// <summary>
    /// Description of the item.
    /// </summary>
    [TextArea(15, 20)]
    public string description;
    /// <summary>
    /// Indicated if the item can be stacked.
    /// </summary>
    public bool stackable;
    /// <summary>
    /// Indicated if the item can be destroyed.
    /// </summary>
    public bool destroyable;

    public ItemUI GetItemData()
    {
        return data;
    }
}

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
    Miscellaneous,
    KeyItem,
}
