using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] private Image[] ItemIcons;
    [SerializeField] private Item[] Items;
    [SerializeField] private string[] ShopkeeperDialog;
    [SerializeField] private AudioClip ShopkeeperVoice;
    [SerializeField] private TextMeshProUGUI ItemDescriptionDisplayArea;
    [SerializeField] private TextMeshProUGUI ItemNameDisplayArea;
    [SerializeField] private TextMeshProUGUI ItemPriceDisplayArea;
    [SerializeField] private TextMeshProUGUI ShopkeeperDialogArea;
    [SerializeField] private GameObject Speechbubble;
    [SerializeField] private Sprite soldOutImage;
    [SerializeField] private GameObject BuyAmountPanel;
    [SerializeField] private Button PlusButton;
    [SerializeField] private Button MinusButton;
    [SerializeField] private Button BuyButton;
    [SerializeField] private GameObject CancelButton;
    [SerializeField] private TextMeshProUGUI AmountText;

    private Animator ShopkeeperAnimator;
    private Item selectedItem;

    private SceneTransition sceneTransition;
    private bool isWriting = false;
    private void Start()
    {
        ItemNameDisplayArea.text = "";
        ItemDescriptionDisplayArea.text = "";
        ItemPriceDisplayArea.text = "";
        Speechbubble.SetActive(false);

        sceneTransition = FindFirstObjectByType<SceneTransition>();
        ShopkeeperAnimator = GetComponent<Animator>();
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

        InvokeRepeating(nameof(DisplayShopkeeperDialog), 5f, 5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (sceneTransition)
                StartCoroutine(sceneTransition.LoadScene());
        }

        foreach(var item in Items)
        {
            if (item.IsSoldOut)
            {
                item.Cooldown -= Time.deltaTime;
                if (item.Cooldown <= 0)
                {
                    item.IsSoldOut = false;
                    item.Cooldown = 0;
                    ItemIcons[System.Array.IndexOf(Items, item)].GameObject().SetActive(true);
                }
            }
        }
    }

    private void OnItemHovered(Image itemIcon)
    {
        var item = GetItemFromIcon(itemIcon);
        if (item != null)
        {
            ItemNameDisplayArea.text = item.ItemName;
            ItemDescriptionDisplayArea.text = item.ItemDescription;
            ItemPriceDisplayArea.text = "Cost: " + item.ItemCost + " Credix\n" +
                "In Inventory: " + GameManager.Instance.Currenthero.inventory.GetItemCount(item);
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

    public void OnItemClick(Image itemIcon)
    {
        selectedItem = GetItemFromIcon(itemIcon);
        if (selectedItem && !selectedItem.IsSoldOut)
        {
            var currentHero = GameManager.Instance.Currenthero;
            int currentItemCount = currentHero.inventory.GetItemCount(selectedItem);
            if (selectedItem.ItemCount <= currentItemCount)
            {
                ItemNameDisplayArea.text = "";
                ItemDescriptionDisplayArea.text = "Max Capacity! You cannot buy anymore of this item!";
                ItemPriceDisplayArea.text = "";
                return;
            }
            else
            {
                BuyAmountPanel.SetActive(true);
                CancelButton.SetActive(true);
                AmountText.text = "1";
                BuyButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    public void OnAmountIncrease()
    {
        int amount = int.Parse(AmountText.text);
        if(selectedItem && amount < selectedItem.ItemCount)
            amount++;
        else
            amount = selectedItem.ItemCount;
        AmountText.text = amount.ToString();
    }

    public void OnAmountDecrease()
    {
        int amount = int.Parse(AmountText.text);
        amount--;
        if (amount < 1)
            amount = 1;
        AmountText.text = amount.ToString();
    }

    public void OnBuyAmount()
    {
        if (selectedItem)
        {
            var amount = int.Parse(AmountText.text);
            var totalCost = selectedItem.ItemCost * amount;
            var currentHero = GameManager.Instance.Currenthero;

            if (totalCost <= currentHero.credixAmount)
            {
                currentHero.credixAmount -= totalCost;
                currentHero.inventory.AddItem(selectedItem, amount);
                selectedItem.ItemCount -= amount;

                currentHero.SaveHeroData();
                BuyAmountPanel.SetActive(false);
                CancelButton.SetActive(false);

                if (selectedItem.ItemCount <= 0)
                {
                    selectedItem.IsSoldOut = true;
                    selectedItem.Cooldown = 10;
                    ItemIcons[System.Array.IndexOf(Items, selectedItem)].GameObject().SetActive(false);
                }
            }
            else
            {
                BuyButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }
    }

    private void DisplayShopkeeperDialog()
    {
        if(ShopkeeperDialog.Length == 0 || isWriting)
            return;

        ShopkeeperDialogArea.text = "";
        Speechbubble.SetActive(true);
        var dialog = ShopkeeperDialog[Random.Range(0, ShopkeeperDialog.Length)];
        StartCoroutine(WriteText(dialog, ShopkeeperDialogArea, 0.09f, ShopkeeperVoice));
    }

    private IEnumerator WriteText(string text, TextMeshProUGUI textDisplay, float delay, AudioClip textSound)
    {
        isWriting = true;
        ShopkeeperAnimator.SetBool("isTalking", true);
        for (int i = 0; i < text.Length; i++)
        {
            textDisplay.text += text[i];
            if (textSound != null)
                AudioManager.Instance.PlaySFX(textSound);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(2f);
        textDisplay.text = "";
        Speechbubble.SetActive(false);
        isWriting = false;
        ShopkeeperAnimator.SetBool("isTalking", false);
    }
}