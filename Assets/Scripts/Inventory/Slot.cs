using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering;
    private ItemSO heldItem;
    private int itemAmount;
    private Image iconImage;
    private TextMeshProUGUI amountTxt;

    [Header("Selection")]
    public Image slotBackground; // assign the slot's background Image in Inspector
    public Color normalColor = new Color(1, 1, 1, 0.5f);
    public Color selectedColor = new Color(1, 1, 0, 1f);
    private bool isSelected = false;

    private void Awake()
    {
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amountTxt = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if (slotBackground != null)
            slotBackground.color = normalColor;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (slotBackground != null)
            slotBackground.color = isSelected ? selectedColor : normalColor;
    }

    public ItemSO GetItem()
    {
        return heldItem;
    }

    public int GetAmount()
    {
        return itemAmount;
    }

    public void SetItem(ItemSO item, int amount = 1)
    {
        heldItem = item;
        itemAmount = amount;

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (heldItem != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = heldItem.icon;
            amountTxt.text = itemAmount.ToString();
        }
        else
        {
            iconImage.enabled = false;
            amountTxt.text = "";
        }
    }

    public int AddAmount(int amount)
    {
        itemAmount += amount;
        UpdateSlot();
        return itemAmount;
    }

    public int RemoveAmount(int amount)
    {
        itemAmount -= amount;
        if (itemAmount <= 0)
            ClearSlot();
        else
            UpdateSlot();
        return itemAmount;
    }

    public void ClearSlot()
    {
        heldItem = null;
        itemAmount = 0;
        UpdateSlot();
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}