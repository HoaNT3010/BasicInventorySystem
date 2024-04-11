using UnityEditor;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private ItemObject item;

    public ItemObject GetItem() { return item; }

    private void Start()
    {
        // 3D model will be displayed when the game started
        if(item.itemModel != null)
        {
            Instantiate(item.itemModel, gameObject.transform);
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = item.itemSprite;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
