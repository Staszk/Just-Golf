using UnityEngine;
using System.Collections.Generic;

public class ItemBoxController : MonoBehaviour
{
    private static ItemBoxController instance;

    public enum Items
    {
        None,
        WaterBottle,
        SwingFuel,
        RangeFinder,
        Coin,
        BearTrap,
        SpringPlatform,
        KillingEfficiency,
        GolfingEfficiency, 
        ConfusedControls
    }

    [SerializeField] private ItemBox[] itemBoxes = null;
    [SerializeField] private UsableItem[] prefabItems = null;
    private UsableItem[] allItems;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            return;

        PrepareItems();
        PrepareItemBoxes();
    }

    private void OnEnable()
    {
        NetworkItemManager.EventBoxPickup += BoxPickup;
    }

    private void OnDisable()
    {
        NetworkItemManager.EventBoxPickup -= BoxPickup;
    }

    private void Update()
    {
        for (int i = 0; i < itemBoxes.Length; i++)
        {
            itemBoxes[i].UpdateFromParent();
        }
    }

    private void PrepareItems()
    {
        allItems = new UsableItem[prefabItems.Length];

        for (int i = 0; i < prefabItems.Length; i++)
        {
            allItems[i] = Instantiate(prefabItems[i], transform);
            allItems[i].Init(this);
        }
    }

    private void PrepareItemBoxes()
    {
        for (int i = 0; i < itemBoxes.Length; i++)
        {
            itemBoxes[i].Init(this, i);
        }
    }

    /// <summary>
    /// To be called from the Network to deactive item boxes for each client
    /// </summary>
    /// <param name="id">The ID of the item box to deactivate.</param>
    private void DeactivateBox(int id)
    {
        itemBoxes[id].Use();
    }

    private void BoxPickup(NetworkBoxPickup box)
    {
        if (box.isMine) { itemBoxes[box.boxId].GainItemFromNetwork(); }
        else { DeactivateBox(box.boxId); }
       
    }

    public UsableItem GetItem(Items itemToGet)
    {
        return System.Array.Find(allItems, item => item.ItemType == itemToGet);
    }

    public static Sprite GetItemSprite(Items itemToGet)
    {
        return System.Array.Find(instance.allItems, item => item.ItemType == itemToGet).ItemSprite;
    }

    public void TakeItem(UsableItem item)
    {
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
    }
}
