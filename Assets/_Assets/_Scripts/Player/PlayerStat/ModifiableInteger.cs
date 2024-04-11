using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();

[Serializable]
public class ModifiableInteger
{
    [SerializeField] private int baseValue;
    public int BaseValue
    {
        get { return baseValue; }
        set
        {
            baseValue = value;
            UpdateModifiedValue();
        }
    }

    [SerializeField] private int modifiedValue;
    public int ModifiedValue
    {
        get { return modifiedValue; }
        private set { modifiedValue = value; }
    }

    public List<IModifier> modifiers = new List<IModifier>();

    public event ModifiedEvent ValueModified;
    public ModifiableInteger(ModifiedEvent method = null)
    {
        modifiedValue = BaseValue;
        if (method != null)
        {
            ValueModified += method;
        }
    }

    public void RegisterModifiedEvent(ModifiedEvent method)
    {
        ValueModified += method;
    }

    public void UnregisterModifiedEvent(ModifiedEvent method)
    {
        ValueModified -= method;
    }

    public void UpdateModifiedValue()
    {
        int valueToAdd = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].AddValue(ref valueToAdd);
        }
        ModifiedValue = baseValue + valueToAdd;
        if (ValueModified != null)
        {
            ValueModified.Invoke();
        }
    }

    public void AddModifier(IModifier modifier)
    {
        modifiers.Add(modifier);
        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier modifier)
    {
        if (modifiers.Contains(modifier))
        {
            modifiers.Remove(modifier);
        }
        UpdateModifiedValue();
    }
}
