using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int itemId, wearId;
    public int price;


    public GameObject itemPrefab;
    public TMP_Text priceText;
    public Button buyButton, equipButton, unequipButton;


    public bool HasItem()
    {
        // 0: satın almamış
        // 1: satın almış ama giymemiş
        // 2: satın almış ve giyiyior.

        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0; //0a eşit değilse true eşitse false;
        return hasItem;
    }


    public bool IsEquppied()
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2; //0a eşit değilse true eşitse false;
        return equippedItem;
    }


    public void InitializeItem()
    {
        priceText.text = price.ToString();
        if (HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if (IsEquppied())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }


    public void BuyItem()
    {
        if (!HasItem())
        {
            int gold = PlayerPrefs.GetInt("gold");
            if (gold >= price)
            {
                PlayerController.CurrentPlayerController.itemSound.PlayOneShot(PlayerController.CurrentPlayerController.buyClip, 0.1f);
                LevelController.Current.GiveGoldToPlayer(-price);
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem()
    {
        UnequipItem();
        MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerController.CurrentPlayerController.wearSpots[wearId].transform).GetComponent<Item>();
        MarketController.Current.equippedItems[wearId].itemId = itemId;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);
    }

    public void UnequipItem()
    {
        Item equippedItem = MarketController.Current.equippedItems[wearId];
        if (equippedItem != null)
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemId];
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }
    }


    public void EquipItemButton()
    {
        PlayerController.CurrentPlayerController.itemSound.PlayOneShot(PlayerController.CurrentPlayerController.equipItemClip, 0.1f);
        EquipItem();
    }

    public void UnequipItemButton()
    {
        PlayerController.CurrentPlayerController.itemSound.PlayOneShot(PlayerController.CurrentPlayerController.unequipItemClip, 0.1f);
        UnequipItem();
    }
}
