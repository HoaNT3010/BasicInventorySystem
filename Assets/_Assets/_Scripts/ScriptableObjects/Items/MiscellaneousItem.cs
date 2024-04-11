using UnityEngine;

[CreateAssetMenu(fileName = "New Miscellaneous Item", menuName = "Inventory System/Items/Miscellaneous")]
public class MiscellaneousItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.Miscellaneous;
        stackable = true;
        destroyable = true;
    }
}
