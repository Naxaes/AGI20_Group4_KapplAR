using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    Dictionary<Bloc, int> inventory;
    public Bloc currentItem { get; private set; }

    // Start is called before the first frame update
    public Inventory()
    {
        inventory = new Dictionary<Bloc, int>();
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
