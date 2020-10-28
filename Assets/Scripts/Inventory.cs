using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemUsedEvent : UnityEvent<Bloc>
{}

public class Inventory
{

    public Dictionary<Bloc, int> inventory { get; private set; }
    public Bloc currentItem { get; private set; }
    public ItemUsedEvent ItemUsedEvent { get; }

    // Start is called before the first frame update
    public Inventory()
    {
        inventory = new Dictionary<Bloc, int>();
        ItemUsedEvent = new ItemUsedEvent();
    }
    
    public void AddItem(Bloc bloc, int quantity = 1)
    {
        
        if(inventory.ContainsKey(bloc))
        {
            inventory[bloc]+= quantity;
        } else
        {
            inventory.Add(bloc, quantity);
        }
        if(currentItem == null)
        {
            currentItem = bloc;
        }
    }

    /// <summary>
    /// Returns true if the inventory contains at least one bloc that can be used.
    /// Remove 1 element of the corresponding item from the inventory.
    /// </summary>
    public bool UseItem()
    {
        if(inventory[currentItem]>0)
        {
            inventory[currentItem]--;
            ItemUsedEvent.Invoke(currentItem);
            return true;
        } else
        {
            return false;
        }
    }

    public bool SelectItem(Bloc bloc)
    {
        if (inventory.ContainsKey(bloc))
        {
            currentItem = bloc;
            return true;
        }
        else
        {
            return false;
        }
    }
}
