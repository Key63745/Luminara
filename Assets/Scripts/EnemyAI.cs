using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip s1;
    public AudioClip s2;
    public AudioClip s3;
    public AudioClip s4;
    bool chaseAudioPrevented = true;

    Animator animator;
	UnityEngine.AI.NavMeshAgent navMeshAgent;
	GameObject Player;
    public bool alerted;
    public bool isMoving;
    public bool idle;
    public Vector3 targetPos;
    public float wanderDistance = 5.0f;
    public float walkAwareDistance = 20.0f;
    public float runAwareDistance = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
		animator = GetComponent<Animator> ();
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        audioSource = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {

        //Wander when not aware of player
        if (!alerted) {
            if (!isMoving && !idle) {
                targetPos = GetRandomLocationNearby();
                //Move to selected location
                navMeshAgent.SetDestination (targetPos);
                isMoving = true;
            }
            else {

                //When path is finished, idle, wait, then repeat
                if (pathComplete() && isMoving) {
                    audioSource.PlayOneShot(s4, 1.0f);
                    Debug.Log(targetPos);
                    isMoving = false;
				    //animator.SetBool ("isIdle", true);
                    StartCoroutine (DelayBeforeMovement ());
                }
                
            }
        }

        //Become aware of player if they get too close (Larger range if they are running)
        float dist = Vector3.Distance(Player.transform.position, transform.position);
        if (dist <= walkAwareDistance) {
            if (!alerted)
            {
                audioSource.PlayOneShot(s3, 1.0f);
                StartCoroutine(DelayAudio());
            }
            alerted = true;
        }

        //Chase player when alerted
        if (alerted) {
            if (!chaseAudioPrevented)
            {
                int soundToPlay = Random.Range(1, 3);
                if (soundToPlay == 1)
                {
                    audioSource.PlayOneShot(s1, 1.0f);
                }
                else
                {
                    audioSource.PlayOneShot(s2, 1.0f);
                }
                chaseAudioPrevented = true;
                StartCoroutine(DelayAudio());
            }
            navMeshAgent.SetDestination (Player.transform.position);
        }
    }

    Vector3 GetRandomLocationNearby() {
        Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderDistance, 1);
        return hit.position;
    }

    IEnumerator DelayBeforeMovement() {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        idle = true;
		yield return new WaitForSeconds (4.5f);
        idle = false;
        navMeshAgent.isStopped = false;
    }

    IEnumerator DelayAudio()
    {
        yield return new WaitForSeconds(3.5f);
        chaseAudioPrevented = false;
    }

    protected bool pathComplete() {
		if ( Vector3.Distance( navMeshAgent.destination, navMeshAgent.transform.position) <= navMeshAgent.stoppingDistance && navMeshAgent.hasPath) {	
			return true;
		}
		return false;
	}
}