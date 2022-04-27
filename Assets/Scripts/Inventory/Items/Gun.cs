using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Gun : Item
{
    public GameObject mainCamera;
    public Recoil RecoilObject;
    public ParticleSystem muzzle;

    [SerializeField]
    LayerMask mask;

    public override void Equip()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        gameObject.transform.parent.Find("FlashlightRig").GetComponent<Rig>().weight = gameObject.activeSelf ? 1 : 0;
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        RecoilObject.recoil += 0.1f;
        muzzle.Play();
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 2500, mask))
        {
            Debug.Log(hit.collider.name + "has been shot");
        }
    }
}
