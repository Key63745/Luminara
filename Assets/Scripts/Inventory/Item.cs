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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform attachment = player.transform.Find("Attachment");
        gameObject.transform.SetParent(player.transform);
        gameObject.transform.position = attachment.position;
        gameObject.transform.localRotation = attachment.localRotation;
        gameObject.SetActive(false);
        gameObject.tag = "Untagged";
    }

    public abstract void Equip();
}
