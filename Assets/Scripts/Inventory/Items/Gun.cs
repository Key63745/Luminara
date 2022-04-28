using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Gun : Item
{
    public float damage = 20.0f;
    public GameObject crosshair;
    public GameObject debris;
    public GameObject blood;
    public GameObject mainCamera;
    public Recoil RecoilObject;
    public ParticleSystem muzzle;
    private bool canShoot = true;
    public float fireRate = 3.0f;
    public float fireTimer = 0.0f;
    public AudioSource audioSource;
    public AudioClip s1;

    [SerializeField]
    LayerMask mask;

    public override void Equip()
    {
        HandleEquip();
        gameObject.GetComponentInParent<PlayerStateMachine>().heldItem = gameObject.GetComponent<Item>();
        gameObject.SetActive(!gameObject.activeSelf);
        crosshair.SetActive(!crosshair.activeSelf);
        gameObject.transform.parent.Find("FlashlightRig").GetComponent<Rig>().weight = gameObject.activeSelf ? 1 : 0;
        gameObject.GetComponent<PlayerInput>().enabled = gameObject.activeSelf;
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (canShoot)
        {
            RecoilObject.recoil += 0.1f;
            muzzle.Play();
            audioSource.PlayOneShot(s1, 1.0f);
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 2500, mask))
            {
                if (hit.collider.name == "AlienSmall")
                {
                    GameObject vfx = Instantiate(blood, hit.point, Quaternion.identity) as GameObject;
                    hit.transform.GetComponent<EnemyAI>().health -= damage;
                    hit.transform.GetComponent<EnemyAI>().alerted = true;
                }
                else if (hit.collider.name == "Alien")
                {
                    GameObject vfx = Instantiate(blood, hit.point, Quaternion.identity) as GameObject;
                    if (hit.transform.GetComponent<EnemyAI>().alerted == false)
                    {
                        hit.transform.GetComponent<Animator>().Play("Base Layer.Walk", 0, 0.0f);
                        hit.transform.GetComponent<EnemyAI>().alerted = true;
                        AudioManager.instance.SwapTrack();
                    }
                }
                else
                {
                    GameObject vfx = Instantiate(debris, hit.point, Quaternion.identity) as GameObject;
                }
                Debug.Log(hit.collider.name + " has been shot");
            }
            fireTimer = fireRate;
            canShoot = false;
        }
    }

    public void Update()
    {
        if (fireTimer > 0.0f)
        {
            fireTimer -= Time.deltaTime;
            canShoot = false;
        }
        else
        {
            canShoot = true;
            fireTimer = 0.0f;
        }
    }
}
