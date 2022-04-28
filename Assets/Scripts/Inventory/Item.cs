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
    [SerializeField] bool _collectable;

    // Start is called before the first frame update
    void Start()
    {
        interact += Pickup;
    }

    void Pickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform attachment = player.transform.Find("Attachment");
        GameObject origParent = gameObject.transform.parent.gameObject;
        gameObject.transform.SetParent(player.transform);
        Destroy(origParent);
        gameObject.transform.position = attachment.position;
        gameObject.transform.localRotation = attachment.localRotation;
        gameObject.SetActive(false);
        gameObject.tag = "Untagged";

        if (!_collectable)
            UltimateRadialMenu.RegisterToRadialMenu("Inventory", Equip, _itemManager.itemDictionary[_key].buttonInfo);
        else
        {
            player.GetComponent<FuelManager>().numOfFuel++;
            Equip();
        }
    }

    public abstract void Equip();

    public bool HandleEquip()
    {
        if (gameObject.GetComponentInParent<PlayerStateMachine>().heldItem != null)
        {
            if (gameObject.GetComponentInParent<PlayerStateMachine>().heldItem != gameObject.GetComponent<Item>())
            {
                GameObject alreadyHeldObject = gameObject.GetComponentInParent<PlayerStateMachine>().heldItem.gameObject;
                alreadyHeldObject.SetActive(false);
                alreadyHeldObject = null;
                if (gameObject.GetComponentInParent<PlayerStateMachine>().heldItem.gameObject.GetComponent<Collectable>())
                {
                    return false;
                }
            }

            return true;
        }
        return true;
    }
}
