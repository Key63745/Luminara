using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipEndingCutscene : MonoBehaviour
{
    public float delay1 = 7.5f;
    public float delay2 = 4.0f;
    public float delay3 = 6.0f;
    public Transform target;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;
    public float speed;


    void Start()
    {
        StartCoroutine(SceneSwap1());
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        Vector3 targetDirection = target.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        
    }


    IEnumerator SceneSwap1()
    {
        yield return new WaitForSeconds(delay1);
        camera1.SetActive(false);
        camera2.SetActive(true);
        StartCoroutine(SceneSwap2());
    }

    IEnumerator SceneSwap2()
    {
        yield return new WaitForSeconds(delay2);
        camera2.SetActive(false);
        camera3.SetActive(true);
        StartCoroutine(SceneSwap3());
    }

    IEnumerator SceneSwap3()
    {
        yield return new WaitForSeconds(delay3);
        camera3.SetActive(false);
        camera4.SetActive(true);
    }



    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
