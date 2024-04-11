using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private InventoryObject itemsInventory;
    [SerializeField] private InventoryObject equipmentInventory;
    public Attribute[] attributes;

    [SerializeField] private Transform mainHandTransform;
    [SerializeField] private Transform offhandWristTransform;
    [SerializeField] private Transform offhandHandTransform;
    private Transform helmet;
    private Transform chest;
    private Transform boots;
    private Transform mainHand;
    private Transform offhand;
    private BoneCombiner boneCombiner;

    private void Start()
    {
        boneCombiner = new BoneCombiner(gameObject);
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetOwner(this);
        }
        for (int i = 0; i < equipmentInventory.GetSlots.Length; i++)
        {
            equipmentInventory.GetSlots[i].OnBeforeSlotUpdate += OnRemoveEquipment;
            equipmentInventory.GetSlots[i].OnAfterSlotUpdate += OnEquipEquipment;
        }
    }

    public void OnRemoveEquipment(InventorySlot slot)
    {
        if (slot.SlotItemObject == null)
        {
            return;
        }
        switch (slot.parent.inventory.interfaceType)
        {
            case InterfaceType.Equipment:
                Debug.Log(string.Concat("Removed ", slot.SlotItemObject, " from ", slot.parent.inventory.interfaceType));
                if (slot.item is not EquipmentItemUI equipmentItemUI)
                {
                    return;
                }
                for (int i = 0; i < equipmentItemUI.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].attributeType == equipmentItemUI.buffs[i].attribute)
                        {
                            attributes[j].value.RemoveModifier(equipmentItemUI.buffs[i]);
                        }
                    }
                }

                if(slot.SlotItemObject.itemModel != null)
                {
                    switch (slot.allowedEquipmentTypes[0])
                    {
                        case EquipmentType.Helmet:
                            Destroy(helmet.gameObject);
                            break;
                        case EquipmentType.Breastplate:
                            Destroy(chest.gameObject);
                            break;
                        case EquipmentType.Boots:
                            Destroy(boots.gameObject);
                            break;
                        case EquipmentType.Weapon:
                            Destroy(mainHand.gameObject);
                            break;
                        case EquipmentType.Shield:
                            Destroy(offhand.gameObject);
                            break;
                    }
                }
                return;
        }
    }

    public void OnEquipEquipment(InventorySlot slot)
    {
        if (slot.SlotItemObject == null)
        {
            return;
        }
        switch (slot.parent.inventory.interfaceType)
        {
            case InterfaceType.Equipment:
                Debug.Log(string.Concat("Placed ", slot.SlotItemObject, " on ", slot.parent.inventory.interfaceType));
                if (slot.item is not EquipmentItemUI equipmentItemUI)
                {
                    return;
                }
                for (int i = 0; i < equipmentItemUI.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].attributeType == equipmentItemUI.buffs[i].attribute)
                        {
                            attributes[j].value.AddModifier(equipmentItemUI.buffs[i]);
                        }
                    }
                }

                if (slot.SlotItemObject.itemModel != null)
                {
                    switch (slot.allowedEquipmentTypes[0])
                    {
                        case EquipmentType.Helmet:
                            helmet = boneCombiner.AddLimb(slot.SlotItemObject.itemModel);
                            break;
                        case EquipmentType.Breastplate:
                            chest = boneCombiner.AddLimb(slot.SlotItemObject.itemModel);
                            break;
                        case EquipmentType.Boots:
                            boots = boneCombiner.AddLimb(slot.SlotItemObject.itemModel);
                            break;
                        case EquipmentType.Weapon:
                            mainHand = Instantiate(slot.SlotItemObject.itemModel, mainHandTransform).transform;
                            break;
                        case EquipmentType.Shield:
                            switch (equipmentItemUI.equipmentType)
                            {
                                case EquipmentType.Weapon:
                                    offhand = Instantiate(slot.SlotItemObject.itemModel, offhandHandTransform).transform;
                                    break;
                                case EquipmentType.Shield:
                                    offhand = Instantiate(slot.SlotItemObject.itemModel, offhandWristTransform).transform;
                                    break;
                            }
                            break;
                    }
                }
                return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
            equipmentUI.gameObject.SetActive(!equipmentUI.gameObject.activeSelf);
            Cursor.lockState = inventoryUI.gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Save inventory and equipment");
            itemsInventory.SaveInventory();
            equipmentInventory.SaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Load inventory and equipment");
            itemsInventory.LoadInventory();
            equipmentInventory.LoadInventory();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            if (itemsInventory.AddItem(ItemUIFactory.CreateItemUI(item.GetItem()), 1))
            {
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Cannot add item to inventory!");
            }

        }
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.attributeType, " was updated! Current value: ", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        itemsInventory.ClearInventory();
        equipmentInventory.ClearInventory();
    }
}

public static class GameObjectExtensionMethods
{
    public static void Hide(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public static void Show(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
}
