using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest,
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    [SerializeField] private string savePath;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    public ItemDatabaseObject ItemDatabase { get => itemDatabase; }
    public InterfaceType interfaceType;
    public Inventory itemsContainer;
    public InventorySlot[] GetSlots { get => itemsContainer.inventorySlots; }
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public bool AddItem(ItemUI item, int amount)
    {
        // Inventory is NOT FULL
        if (EmptySlotCount > 0)
        {
            // Add NOT STACKABLE item
            if (!itemDatabase.itemObjects[item.id].stackable)
            {
                SetEmptyInventorySlot(item, amount);
                return true;
            }
            // Check for adding EXISTS STACKABLE item
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.id == item.id && itemDatabase.itemObjects[item.id].stackable)
                {
                    GetSlots[i].AddAmount(amount);
                    return true;
                }
            }
            // Add NON-EXISTING item (Stackable or not)
            SetEmptyInventorySlot(item, amount);
            return true;
        }
        // Inventory is FULL
        else
        {
            // Only add EXISTING STACKABLE item, can specify limits for item amount
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.id == item.id && itemDatabase.itemObjects[item.id].stackable)
                {
                    GetSlots[i].AddAmount(amount);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Loop through the inventory to find the first empty inventory slot then fill the empty inventory slot with new item and amount.
    /// </summary>
    /// <returns>Updated InventorySlot object or null (If inventory is full).</returns>
    public InventorySlot SetEmptyInventorySlot(ItemUI item, int amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            // Empty slot will have id of -1
            if (GetSlots[i].item.id <= -1)
            {
                GetSlots[i].UpdateInventorySlot(item, amount);
                return GetSlots[i];
            }
        }
        // Inventory does not have any empty slot
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot tmp = new InventorySlot(item2.item, item2.amount);
        item2.UpdateInventorySlot(item1.item, item1.amount);
        item1.UpdateInventorySlot(tmp.item, tmp.amount);
    }

    /// <summary>
    /// Swapping equipment items in the equipment interface.
    /// </summary>
    /// <param name="item1">The dragged item slot, can be in both normal and equipment interface.</param>
    /// <param name="item2">The hovered item slot in the equipment interface</param>
    public void SwapEquipmentItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceEquipmentInSlot(item1.SlotItemObject) && item1.CanPlaceEquipmentInSlot(item2.SlotItemObject))
        {
            SwapItems(item1, item2);
        }
    }

    public void RemoveItem(ItemUI item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item == item)
            {
                GetSlots[i].UpdateInventorySlot(ItemUIFactory.CreateEmptyItemUI(), 0);
            }
        }
    }

    [ContextMenu("Save Inventory")]
    public void SaveInventory()
    {
        // Serialized save file using JsonUtility will be easier to modify, which is in JSON
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter binaryFormatter = new BinaryFormatter();
        //FileStream fileStream = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //binaryFormatter.Serialize(fileStream, saveData);
        //fileStream.Close();

        // Serialized save file using IFormatter will be harder to modify, which is in BINARY
        Debug.Log("Saving player inventory.");
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, itemsContainer);
        stream.Close();
        Debug.Log("Saved player inventory.");
    }

    [ContextMenu("Load Inventory")]
    public void LoadInventory()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //FileStream fileStream = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(fileStream).ToString(), this);
            //fileStream.Close();

            Debug.Log("Loading player inventory, clear all existing inventory's items.");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newItemsContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateInventorySlot(newItemsContainer.inventorySlots[i].item, newItemsContainer.inventorySlots[i].amount);
            }
            stream.Close();
            Debug.Log("Loaded player inventory.");
        }
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        itemsContainer.Clear();
    }
}

[Serializable]
public class Inventory
{
    public InventorySlot[] inventorySlots = new InventorySlot[40];

    public void Clear()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot slot);

[Serializable]
public class InventorySlot
{ 
    [NonSerialized]
    public UserInterface parent;
    [NonSerialized]
    public GameObject slotDisplay;
    [NonSerialized]
    public SlotUpdated OnAfterSlotUpdate;
    [NonSerialized]
    public SlotUpdated OnBeforeSlotUpdate;
    // Add SerializeReference attribute so Unity will serialize the derived classes of the base class ItemUI
    [SerializeReference]
    public ItemUI item;

    //public ItemType[] allowedItemTypes = new ItemType[0];
    public EquipmentType[] allowedEquipmentTypes = new EquipmentType[0];
    public int amount;

    public ItemObject SlotItemObject
    {
        get
        {
            if (item.id >= 0)
            {
                return parent.inventory.ItemDatabase.itemObjects[item.id];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateInventorySlot(ItemUIFactory.CreateEmptyItemUI(), 0);
    }

    public InventorySlot(ItemUI item, int amount)
    {
        UpdateInventorySlot(item, amount);
    }

    public void UpdateInventorySlot(ItemUI item, int amount)
    {
        if(OnBeforeSlotUpdate != null)
        {
            OnBeforeSlotUpdate.Invoke(this);
        }
        this.item = item;
        this.amount = amount;
        if (OnAfterSlotUpdate != null)
        {
            OnAfterSlotUpdate.Invoke(this);
        }
    }

    public void AddAmount(int value)
    {
        UpdateInventorySlot(item, amount += value);
    }

    public void RemoveItem()
    {
        UpdateInventorySlot(ItemUIFactory.CreateEmptyItemUI(), 0);
    }

    /// <summary>
    /// Check if the item can be placed in the inventory slot. Currently redundant
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    //public bool CanPlaceItemInSlot(ItemObject item)
    //{
    //    if (allowedItemTypes.Length <= 0)
    //    {
    //        return true;
    //    }
    //    for (int i = 0; i < allowedItemTypes.Length; i++)
    //    {
    //        if (item.type == allowedItemTypes[i])
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public bool CanPlaceEquipmentInSlot(ItemObject item)
    {
        // Allow placing null items (empty slots) or slot with no type requirements
        if (item == null || allowedEquipmentTypes.Length <= 0 || item.data.id < 0)
        {
            return true;
        }
        // Check if the item is an equipment item
        if (item is not EquipmentItem equipment)
        {
            return false;
        }
        // Check if the equipment type of the item is among allowed types
        return allowedEquipmentTypes.Contains(equipment.equipmentType);
    }
}
