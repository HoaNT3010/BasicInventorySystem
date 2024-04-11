using UnityEngine;

[CreateAssetMenu(fileName = "New Key Item", menuName = "Inventory System/Items/Key Item")]
public class KeyItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.KeyItem;
        stackable = false;
        destroyable = false;
    }
}
