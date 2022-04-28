using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipFuel : MonoBehaviour
{
    public GameObject fade;
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;
    bool slot1;
    bool slot2;
    bool slot3;
    bool gameEnd;
    public FuelManager fuel;

    private void OnTriggerEnter(Collider other)
    {
        if (fuel.numOfFuel > 0)
        {
            if (!slot1)
            {
                slot1 = true;
                Slot1.SetActive(true);
            }
            else if (!slot2)
            {
                slot2 = true;
                Slot2.SetActive(true);
            }
            else if (!slot3)
            {
                slot3 = true;
                Slot3.SetActive(true);
            }
            fuel.numOfFuel--;
            if (slot1 && slot2 && slot3)
            {
                if (!gameEnd)
                {
                    StartCoroutine(FadeDelay());
                    gameEnd = true;

                }
            }
        }
    }

    IEnumerator FadeDelay()
    {
        fade.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        LoadScene("Epilogue");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}
