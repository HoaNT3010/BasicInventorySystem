using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Item Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] itemObjects;

    [ContextMenu("Update ID's")]
    public void UpdateItemsId()
    {
        for (int i = 0; i < itemObjects.Length; i++)
        {
            if (itemObjects[i].data.id != i)
            {
                itemObjects[i].data.id = i;
            }
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateItemsId();
    }

    public void OnBeforeSerialize()
    {
    }
}
