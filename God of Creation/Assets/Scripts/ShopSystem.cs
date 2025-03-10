using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] private Image[] ItemIcons;
    [SerializeField] private Item[] Items;
    [SerializeField] private TextMeshProUGUI ItemDescriptionDisplayArea;
    [SerializeField] private TextMeshProUGUI ItemNameDisplayArea;
    [SerializeField] private TextMeshProUGUI ItemPriceDisplayArea;

    private SceneTransition sceneTransition;

    private void Start()
    {
        sceneTransition = FindFirstObjectByType<SceneTransition>();
        DisplayRandomItem();
        foreach (var itemIcon in ItemIcons)
        {
            var eventTrigger = itemIcon.gameObject.AddComponent<EventTrigger>();
            var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((data) => { OnItemHovered(itemIcon); });
            eventTrigger.triggers.Add(pointerEnter);
            var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => { OnItemUnhovered(); });
            eventTrigger.triggers.Add(pointerExit);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(sceneTransition.LoadScene());
        }
    }

    private void OnItemHovered(Image itemIcon)
    {
        var item = GetItemFromIcon(itemIcon);
        if (item != null)
        {
            ItemNameDisplayArea.text = item.ItemName;
            ItemDescriptionDisplayArea.text = item.ItemDescription;
            ItemPriceDisplayArea.text = "Cost: " + item.ItemCost + " Credix";
        }
    }

    private void OnItemUnhovered()
    {
        ItemNameDisplayArea.text = "";
        ItemDescriptionDisplayArea.text = "";
        ItemPriceDisplayArea.text = "";
    }

    private Item GetItemFromIcon(Image itemIcon)
    {
        int index = System.Array.IndexOf(ItemIcons, itemIcon);
        if (index >= 0 && index < Items.Length)
        {
            return Items[index];
        }
        return null;
    }

    private void DisplayRandomItem()
    {
        Items = Items.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < ItemIcons.Length && i < Items.Length; i++)
        {
            ItemIcons[i].sprite = Items[i].ItemIcon;
        }
    }

    public void OnItemBuy(Image itemIcon)
    {
        var item = GetItemFromIcon(itemIcon);
        if (item != null)
        {
            if (item.ItemCost <= GameManager.Instance.Currenthero.credixAmount)
                GameManager.Instance.Currenthero.credixAmount -= item.ItemCost;
            else
            {
                ItemNameDisplayArea.text = "";
                ItemDescriptionDisplayArea.text = "Not enough Credix!";
            }
        }
    }
}