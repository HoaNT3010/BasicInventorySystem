using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Interfaces that can be changed
/// </summary>
public class DynamicInterface : UserInterface
{
    [SerializeField] private GameObject inventoryPrefab;

    public override void InitializeItemDisplay()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            GameObject inventoryItem = Instantiate(inventoryPrefab, gameObject.transform);

            AddEvent(inventoryItem, EventTriggerType.PointerEnter, delegate { OnPointerEnterSlot(inventoryItem); });
            AddEvent(inventoryItem, EventTriggerType.PointerExit, delegate { OnPointerExitSlot(inventoryItem); });
            AddEvent(inventoryItem, EventTriggerType.BeginDrag, delegate { OnBeginDragSlot(inventoryItem); });
            AddEvent(inventoryItem, EventTriggerType.EndDrag, delegate { OnEndDragSlot(inventoryItem); });
            AddEvent(inventoryItem, EventTriggerType.Drag, delegate { OnDragSlot(inventoryItem); });

            inventory.GetSlots[i].slotDisplay = inventoryItem;

            slotsOnInterface.Add(inventoryItem, inventory.GetSlots[i]);
        }
    }
}
