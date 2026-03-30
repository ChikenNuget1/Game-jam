using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public ItemSO fish;
    public ItemSO rod;

    public GameObject inventorySlotParent;
    private List<Slot> inventorySlots = new List<Slot>();

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
    }

    // testing add items
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            AddItem(fish, 1);
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            AddItem(rod, 1);
        }
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        // fill existing stacks
        foreach (Slot slot in inventorySlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0)
                        return;
                }
            }
        }

        // fill empty slots
        foreach (Slot slot in inventorySlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0)
                    return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory is full, could not add " + remaining + " of " + itemToAdd.itemName);
        }
    }
}