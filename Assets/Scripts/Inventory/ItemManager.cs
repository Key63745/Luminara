using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class ItemManager : MonoBehaviour
{

    public InventoryItem[] items;
    [HideInInspector] public Dictionary<string, InventoryItem> itemDictionary = new Dictionary<string, InventoryItem>();

    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].buttonInfo.key = items[i].name;
            items[i].buttonInfo.name = items[i].name;
            items[i].buttonInfo.icon = items[i].icon;

            itemDictionary.Add(items[i].name, items[i]);
        }
    }
}
