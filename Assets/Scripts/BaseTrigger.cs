using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EnemyAI[] yourObjects = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI obj in yourObjects)
            {
                obj.gameObject.transform.position = obj.initialPos;
                obj.targetPos = obj.initialPos;
                obj.health = 100.0f;
                obj.dead = false;
                obj.alerted = false;
                obj.isMoving = false;
                obj.idle = false;
                obj.attacking = false;
                obj.gameObject.GetComponent<Animator>().Play("Base Layer.Idle", 0, 0.0f);
                obj.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
                obj.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
            }
            AudioManager.instance.SwapTrackInside();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AudioManager.instance.SwapTrackAmbient();
        }
    }
}
