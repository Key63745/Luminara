using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactable
{
    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public Sprite icon;
        [HideInInspector] public UltimateRadialButtonInfo buttonInfo;
    }

    [SerializeField] string _key;
    [SerializeField] ItemManager _itemManager;

    // Start is called before the first frame update
    void Start()
    {
        interact += Pickup;
    }

    void Pickup()
    {
        UltimateRadialMenu.RegisterToRadialMenu("Inventory", Equip, _itemManager.itemDictionary[_key].buttonInfo);
        Destroy(gameObject);
    }

    public abstract void Equip();
}
