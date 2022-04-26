using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipMovementCutscene : MonoBehaviour
{
    public float delay1 = 7.5f;
    public float delay2 = 4.0f;
    public float delay1b = 5.0f;
    public float delay3 = 6.0f;
    public float delayFinal = 6.0f;
    public Transform target;
    public GameObject camera1;
    public GameObject camera4;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject cameraFinal;
    public Transform man;
    public Transform machine;
    public Transform dummyPod;
    public Transform machineDupe;
    public Transform targetPod;
    public bool moveMan;
    public bool movePod;
    public bool moveDummyPod;
    public float speed;
    public float speed2;

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

        if (moveMan)
        {
            Vector3 newYPos = new Vector3(machine.position.x, man.position.y, man.position.z);
            man.position = Vector3.MoveTowards(man.position, newYPos, step);
        }

        if (moveDummyPod)
        {
            Vector3 newYPos = new Vector3(dummyPod.position.x, -1000, dummyPod.position.z);
            dummyPod.position = Vector3.MoveTowards(dummyPod.position, newYPos, step);
        }

        if (movePod)
        {
            float step2 = speed2 * Time.deltaTime;
            machineDupe.position = Vector3.MoveTowards(machineDupe.position, targetPod.position, step2);
            float frequency = 10;
            camera3.transform.localPosition = new Vector3(
                Mathf.PerlinNoise(0, Time.time * frequency) * 2 - 1,
                Mathf.PerlinNoise(1, Time.time * frequency) * 2 - 1,
                Mathf.PerlinNoise(2, Time.time * frequency) * 2 - 1
            ) * 0.1f;
        }

    }


    IEnumerator SceneSwap1()
    {
        yield return new WaitForSeconds(delay1);
        camera1.SetActive(false);
        camera2.SetActive(true);
        moveMan = true;
        StartCoroutine(SceneSwap1A());
    }

    IEnumerator SceneSwap1A()
    {
        yield return new WaitForSeconds(delay1b);
        camera2.SetActive(false);
        camera4.SetActive(true);
        moveDummyPod = true;
        StartCoroutine(SceneSwap2());
    }

    IEnumerator SceneSwap2()
    {
        yield return new WaitForSeconds(delay2);
        camera4.SetActive(false);
        camera3.SetActive(true);
        movePod = true;
        StartCoroutine(SceneSwap3());
    }

    IEnumerator SceneSwap3()
    {
        yield return new WaitForSeconds(delayFinal);
        camera3.SetActive(false);
        cameraFinal.SetActive(true);
        cameraFinal.transform.parent = null;
        StartCoroutine(SceneSwapFinal());
    }

    IEnumerator SceneSwapFinal()
    {
        yield return new WaitForSeconds(delay3);
        LoadScene("Story");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
