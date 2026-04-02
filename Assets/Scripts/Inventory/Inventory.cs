using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public ItemSO fish;
    public ItemSO rod;

    public GameObject hotbarSlotParent;
    private List<Slot> hotbarSlots = new List<Slot>();

    public Image dragIcon;
    private Slot draggedSlot = null;
    private bool isDragging = false;

    public GameObject itemDescriptionParent;
    public Image itemDescriptionImage;
    public TextMeshProUGUI descriptionItemNameTxt;
    public TextMeshProUGUI itemDescriptionTxt;

    private int selectedSlotIndex = 0;
    private Slot selectedSlot = null;

    private KeyCode[] hotbarKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    private void Awake()
    {
        hotbarSlots.AddRange(hotbarSlotParent.GetComponentsInChildren<Slot>());

        if (dragIcon != null)
            dragIcon.enabled = false;
    }

    void Start()
    {
        AddItem(rod, 1);
        SelectSlot(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            AddItem(fish, 1);
        else if (Input.GetKeyDown(KeyCode.R))
            AddItem(rod, 1);

        // select hotbar slot by key
        for (int i = 0; i < hotbarKeys.Length && i < hotbarSlots.Count; i++)
        {
            if (Input.GetKeyDown(hotbarKeys[i]))
            {
                SelectSlot(i);
                break;
            }
        }

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();
        UpdateItemDescription();
    }

    private void SelectSlot(int index)
    {
        // don't select if slot is empty
        if (!hotbarSlots[index].HasItem()) return;

        // unequip item in previous slot
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
            if (selectedSlot.HasItem())
            {
                selectedSlot.GetItem().isEquipped = false;
                Debug.Log(selectedSlot.GetItem().itemName + " has been unequipped.");
            }
        }

        selectedSlotIndex = index;
        selectedSlot = hotbarSlots[index];
        selectedSlot.SetSelected(true);

        if (selectedSlot.HasItem())
        {
            selectedSlot.GetItem().isEquipped = true;
            Debug.Log(selectedSlot.GetItem().itemName + " has been equipped.");
        }
    }

    public Slot GetSelectedSlot()
    {
        return selectedSlot;
    }

    public ItemSO GetSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.HasItem())
            return selectedSlot.GetItem();
        return null;
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        // fill existing stacks
        foreach (Slot slot in hotbarSlots)
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
                    {
                        SelectSlot(selectedSlotIndex);
                        return;
                    }
                }
            }
        }

        // fill empty slots
        foreach (Slot slot in hotbarSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0)
                {
                    SelectSlot(selectedSlotIndex);
                    return;
                }
            }
        }

        if (remaining > 0)
            Debug.Log("Hotbar is full, could not add " + remaining + " of " + itemToAdd.itemName);

        SelectSlot(selectedSlotIndex);
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

            dragIcon.enabled = false;
            draggedSlot = null;
            isDragging = false;

            SelectSlot(selectedSlotIndex);
        }
    }

    private Slot GetHoveredSlot()
    {
        foreach (Slot s in hotbarSlots)
        {
            if (s.hovering)
                return s;
        }
        return null;
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;

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

        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
            dragIcon.transform.position = Mouse.current.position.ReadValue();
    }

    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoveredSlot();

        if (hoveredSlot != null)
        {
            ItemSO hoveredItem = hoveredSlot.GetItem();

            if (hoveredItem != null)
            {
                itemDescriptionParent.SetActive(true);
                itemDescriptionImage.sprite = hoveredItem.icon;
                itemDescriptionTxt.text = hoveredItem.description;
                descriptionItemNameTxt.text = hoveredItem.itemName;
                return;
            }
        }

        itemDescriptionParent.SetActive(false);
    }
}