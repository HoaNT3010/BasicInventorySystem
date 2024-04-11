using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;

    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventory.itemsContainer.inventorySlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterSlotUpdate += OnSlotUpdate;
        }
        InitializeItemDisplay();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnPointerEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnPointerExitInterface(gameObject); });
        gameObject.Hide();
    }

    private void OnSlotUpdate(InventorySlot slot)
    {
        // Slot contains item
        if (slot.item.id >= 0)
        {
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = slot.SlotItemObject.itemSprite;
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = slot.SlotItemObject.stackable ? slot.amount.ToString("n0") : "";
        }
        // Empty slot
        else
        {
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public abstract void InitializeItemDisplay();

    // Update is called once per frame
    void Update()
    {
        //UpdateItemDisplay();
    }

    private void UpdateItemDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in slotsOnInterface)
        {
            // Slot contains item
            if (slot.Value.item.id >= 0)
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = slot.Value.SlotItemObject.itemSprite;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.SlotItemObject.stackable ? slot.Value.amount.ToString("n0") : "";
            }
            // Empty slot
            else
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    /// <summary>
    /// Adding event to inventory item slot.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="type"></param>
    /// <param name="action"></param>
    protected void AddEvent(GameObject gameObject, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnPointerEnterSlot(GameObject gameObject)
    {
        MouseData.hoveredGameObject = gameObject;
    }

    public void OnPointerExitSlot(GameObject gameObject)
    {
        MouseData.hoveredGameObject = null;
    }

    public void OnBeginDragSlot(GameObject gameObject)
    {
        if(slotsOnInterface[gameObject].item.id <= -1)
        {
            return;
        }
        GameObject mouseObject = CreateTemporaryItem(gameObject);
        MouseData.draggedGameObject = mouseObject;
    }

    private GameObject CreateTemporaryItem(GameObject draggedObject)
    {
        GameObject mouseObject = new GameObject();
        RectTransform rectTransform = mouseObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(82, 82);
        mouseObject.transform.SetParent(transform.parent);
        if (slotsOnInterface[draggedObject].item.id > -1)
        {
            Image image = mouseObject.AddComponent<Image>();
            image.sprite = slotsOnInterface[draggedObject].SlotItemObject.itemSprite;
            image.raycastTarget = false;
        }
        return mouseObject;
    }

    public void OnEndDragSlot(GameObject gameObject)
    {
        Destroy(MouseData.draggedGameObject);
        // Destroy item by dragging it out of inventory interface
        if (MouseData.mouseOverInterface == null)
        {
            slotsOnInterface[gameObject].RemoveItem();
            return;
        }
        // If cursor is hover on a inventory slot and dragging an item in different inventory slot
        if (MouseData.hoveredGameObject && slotsOnInterface[gameObject].item.id > -1)
        {
            InventorySlot mouseHoverSlotData = MouseData.mouseOverInterface.slotsOnInterface[MouseData.hoveredGameObject];
            inventory.SwapEquipmentItems(slotsOnInterface[gameObject], mouseHoverSlotData);
        }
    }

    public void OnDragSlot(GameObject gameObject)
    {
        if (MouseData.draggedGameObject != null)
        {
            MouseData.draggedGameObject.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnPointerEnterInterface(GameObject gameObject)
    {
        MouseData.mouseOverInterface = gameObject.GetComponent<UserInterface>();
    }

    public void OnPointerExitInterface(GameObject gameObject)
    {
        MouseData.mouseOverInterface = null;
    }
}

public static class MouseData
{
    /// <summary>
    /// Specified which interfaces the cursor is currently on.
    /// </summary>
    public static UserInterface mouseOverInterface;
    /// <summary>
    /// Temporary cloned gameobject of the dragged gameobject.
    /// </summary>
    public static GameObject draggedGameObject;
    public static GameObject hoveredGameObject;
}
