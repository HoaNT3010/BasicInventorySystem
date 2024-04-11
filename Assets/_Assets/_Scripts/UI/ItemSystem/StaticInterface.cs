using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Interface created in the editor and only applied update
/// </summary>
public class StaticInterface : UserInterface
{
    public GameObject[] slots;

    public override void InitializeItemDisplay()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            GameObject obj = slots[i];
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnPointerEnterSlot(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnPointerExitSlot(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnBeginDragSlot(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDragSlot(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDragSlot(obj); });

            inventory.GetSlots[i].slotDisplay = obj;

            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
        }
    }
}
