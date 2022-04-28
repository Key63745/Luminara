using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{

    public GameObject fade;

    public void LoadScene(string sceneName){
       SceneManager.LoadScene(sceneName); 
   }

    public void Load()
    {
        StartCoroutine(FadeDelay());
    }

    IEnumerator FadeDelay()
    {
        fade.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        LoadScene("Prologue");
    }
}