using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    #region Script Variables
    [Header("ShopKeeper Settings")]
    [SerializeField] private string[] shopkeeperDialog;
    [SerializeField] private AudioClip shopkeeperVoice;
    [SerializeField] private GameObject speechbubble;
    [SerializeField] private TextMeshProUGUI shopkeeperDialogArea;

    [Header("Shop Items")]
    [SerializeField] private Image[] itemIcons;
    [SerializeField] private Item[] items;
    [SerializeField] private TextMeshProUGUI itemDescriptionDisplayArea;
    [SerializeField] private TextMeshProUGUI itemNameDisplayArea;
    [SerializeField] private TextMeshProUGUI itemPriceDisplayArea;

    [Header("Buy Panel")]
    [SerializeField] private GameObject buyAmountPanel;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private TextMeshProUGUI amountText;

    private Animator ShopkeeperAnimator;
    private Item selectedItem;

    private SceneTransition sceneTransition;
    private bool isWriting = false;
    #endregion

    #region Unity functions
    private void Start()
    {
        InitalizeUI();

        sceneTransition = FindFirstObjectByType<SceneTransition>();
        ShopkeeperAnimator = GetComponent<Animator>();
        DisplayRandomItem();

        SetupItemIcons();

        InvokeRepeating(nameof(DisplayShopkeeperDialog), 5f, 5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (sceneTransition)
                StartCoroutine(sceneTransition.LoadScene());
        }

        UpdateItems();
    }
    #endregion

    #region Public Methods
    public void OnAmountIncrease()
    {
        int amount = int.Parse(amountText.text);
        amount = Mathf.Min(amount + 1, selectedItem.ItemCount);
        amountText.text = amount.ToString();
    }

    public void OnAmountDecrease()
    {
        int amount = int.Parse(amountText.text);
        amount = Mathf.Min(amount - 1, 1);
        amountText.text = amount.ToString();
    }

    public void OnBuyAmount()
    {
        if (selectedItem)
        {
            ProcessPurchase();
        }
        else
        {

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
                DisplayMessage("Max Capacity! You cannot buy anymore of this item!");
                return;
            }
            else
            {
                ShowBuyPanel();
            }
        }
    }

    public void CloseBuyPanel()
    {
        buyAmountPanel.SetActive(false);
        cancelButton.SetActive(false);
    }

    #endregion

    #region Private Methods
    private void InitalizeUI()
    {
        shopkeeperDialogArea.text = "";
        speechbubble.SetActive(false);
        itemNameDisplayArea.text = "";
        itemDescriptionDisplayArea.text = "";
        itemPriceDisplayArea.text = "";
        buyAmountPanel.SetActive(false);
        cancelButton.SetActive(false);
    }

    private void UpdateItems()
    {
        foreach (var item in items)
        {
            if (item.IsSoldOut)
            {
                item.Cooldown -= Time.deltaTime;
                if (item.Cooldown <= 0)
                {
                    item.IsSoldOut = false;
                    item.Cooldown = 0;
                    itemIcons[System.Array.IndexOf(items, item)].GameObject().SetActive(true);
                }
            }
        }
    }

    private void SetupItemIcons()
    {
        foreach (var itemIcon in itemIcons)
        {
            var eventTrigger = itemIcon.gameObject.AddComponent<EventTrigger>();
            AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, (data) => OnItemHovered(itemIcon));
            AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, (data) => OnItemUnhovered());
        }
    }

    private void AddEventTrigger(EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
    }

    private void OnItemHovered(Image itemIcon)
    {
        var item = GetItemFromIcon(itemIcon);
        if (item != null)
        {
            itemNameDisplayArea.text = item.ItemName;
            itemDescriptionDisplayArea.text = item.ItemDescription;
            itemPriceDisplayArea.text = $"Cost: { item.ItemCost } Credix\nIn Inventory: { GameManager.Instance.Currenthero.inventory.GetItemCount(item) }";
        }
    }

    private void OnItemUnhovered()
    {
        itemNameDisplayArea.text = "";
        itemDescriptionDisplayArea.text = "";
        itemPriceDisplayArea.text = "";
    }

    private Item GetItemFromIcon(Image itemIcon)
    {
        int index = System.Array.IndexOf(itemIcons, itemIcon);
        return index >= 0 && index < items.Length ? items[index] : null;
    }

    private void DisplayRandomItem()
    {
        items = items.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < itemIcons.Length && i < items.Length; i++)
        {
            itemIcons[i].sprite = items[i].ItemIcon;
        }
    }

    private void DisplayMessage(string message)
    {
        itemNameDisplayArea.text = "";
        itemDescriptionDisplayArea.text = message;
        itemPriceDisplayArea.text = "";
    }

    private void ShowBuyPanel()
    {
        buyAmountPanel.SetActive(true);
        cancelButton.SetActive(true);
        amountText.text = "1";
        buyButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    private void ProcessPurchase()
    {
        var amount = int.Parse(amountText.text);
        var totalCost = selectedItem.ItemCost * amount;
        var currentHero = GameManager.Instance.Currenthero;

        if (totalCost <= currentHero.credixAmount)
        {
            CompletePurchase(amount, totalCost, currentHero);
        }
        else
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
    }

    private void CompletePurchase(int amount, int totalCost, HeroStats currentHero)
    {
        currentHero.credixAmount -= totalCost;
        currentHero.inventory.AddItem(selectedItem, amount);
        selectedItem.ItemCount -= amount;

        currentHero.SaveHeroData();
        buyAmountPanel.SetActive(false);
        cancelButton.SetActive(false);

        if (selectedItem.ItemCount <= 0)
        {
            selectedItem.IsSoldOut = true;
            selectedItem.Cooldown = 10;
            itemIcons[System.Array.IndexOf(items, selectedItem)].GameObject().SetActive(false);
        }
    }

    private void DisplayShopkeeperDialog()
    {
        if(shopkeeperDialog.Length == 0 || isWriting)
            return;

        shopkeeperDialogArea.text = "";
        speechbubble.SetActive(true);
        var dialog = shopkeeperDialog[Random.Range(0, shopkeeperDialog.Length)];
        StartCoroutine(WriteText(dialog, shopkeeperDialogArea, 0.09f, shopkeeperVoice));
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
        speechbubble.SetActive(false);
        isWriting = false;
        ShopkeeperAnimator.SetBool("isTalking", false);
    }

    #endregion
}