using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public ItemSO fish;
    public ItemSO rod;

    public GameObject inventorySlotParent;
    private List<Slot> inventorySlots = new List<Slot>();
    public GameObject container;

    public Image dragIcon;
    private Slot draggedSlot = null;
    private bool isDragging = false;

    public GameObject itemDescriptionParent;
    public Image itemDescriptionImage;
    public TextMeshProUGUI descriptionItemNameTxt;
    public TextMeshProUGUI itemDescriptionTxt;

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        dragIcon.enabled = false; // hide drag icon on start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddItem(fish, 1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            AddItem(rod, 1);
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            container.SetActive(!container.activeInHierarchy);
        }
        StartDrag();
        UpdateDragItemPosition();
        EndDrag();

        UpdateItemDescription();
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
            Debug.Log("Inventory is full, could not add " + remaining + " of " + itemToAdd.itemName);
    }

    private void StartDrag()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Slot hovered = GetHoveredSlot();

            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;

                // show drag icon
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            Slot hovered = GetHoveredSlot();

            if (hovered != null)
                HandleDrop(draggedSlot, hovered);

            // always reset drag state on mouse up
            dragIcon.enabled = false;
            draggedSlot = null;
            isDragging = false;
        }
    }

    private Slot GetHoveredSlot()
    {
        foreach (Slot s in inventorySlots)
        {
            if (s.hovering)
                return s;
        }
        return null;
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;

        // stacking
        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if (space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if (from.GetAmount() <= 0)
                    from.ClearSlot();

                return;
            }
        }

        // different item
        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        // empty slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            dragIcon.transform.position = Mouse.current.position.ReadValue();
        }
    }

    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoveredSlot();

        if(hoveredSlot != null)
        {
            ItemSO hoveredItem = hoveredSlot.GetItem();

            if(hoveredItem != null)
            {
                itemDescriptionParent.SetActive(true);
                itemDescriptionImage.sprite = hoveredItem.icon;
                itemDescriptionTxt.text = hoveredItem.description;
                descriptionItemNameTxt.text = hoveredItem.name;
                return;
            }
        }
        itemDescriptionParent.SetActive(false);
    }
}