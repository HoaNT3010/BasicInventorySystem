using System;

[Serializable]
public class Attribute
{
    [NonSerialized]
    public Player owner;
    public EquipmentAttribute attributeType;
    public ModifiableInteger value;

    public void SetOwner(Player owner)
    {
        this.owner = owner;
        value = new ModifiableInteger(AttributeModified);
    }

    public void AttributeModified()
    {
        owner.AttributeModified(this);
    }
}
