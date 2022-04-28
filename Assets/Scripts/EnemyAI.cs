using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public PlayerStateMachine player;
    public int type = 0;
    public float health = 100.0f;
    private bool dead;
    AudioSource audioSource;
    public AudioClip s1;
    public AudioClip s2;
    public AudioClip s3;
    public AudioClip s4;
    public AudioClip test;
    bool chaseAudioPrevented = true;

    Animator animator;
	UnityEngine.AI.NavMeshAgent navMeshAgent;
	GameObject Player;
    public bool alerted;
    public bool isMoving;
    public bool idle;
    public bool attacking;
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

        if (health > 0)
        {
            //Wander when not aware of player
            if (!alerted)
            {
                if (!isMoving && !idle)
                {
                    targetPos = GetRandomLocationNearby();
                    //Move to selected location
                    navMeshAgent.SetDestination(targetPos);
                    isMoving = true;
                }
                else
                {

                    //When path is finished, idle, wait, then repeat
                    if (pathComplete() && isMoving)
                    {
                        audioSource.PlayOneShot(s4, 1.0f);
                        isMoving = false;
                        //animator.SetBool ("isIdle", true);
                        StartCoroutine(DelayBeforeMovement());
                    }

                }
            }

            //Become aware of player if they get too close (Larger range if they are running)
            float dist = Vector3.Distance(Player.transform.position, transform.position);
            if (dist <= walkAwareDistance)
            {
                if (!alerted)
                {
                    if (type == 1)
                    {
                        AudioManager.instance.SwapTrack();
                    }
                    audioSource.PlayOneShot(s3, 1.0f);
                    animator.Play("Base Layer.Walk", 0, 0.0f);
                    StartCoroutine(DelayAudio());
                }
                alerted = true;
            }

            //Chase player when alerted
            if (alerted)
            {
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

                if (!attacking)
                {
                    navMeshAgent.SetDestination(Player.transform.position);
                    if (pathComplete())
                    {
                        attacking = true;
                        StartCoroutine(DelayAttack());
                    }
                }

            }


        }
        else
        {
            if (!dead)
            {
                audioSource.PlayOneShot(s3, 1.0f);
                alerted = false;
                isMoving = false;
                attacking = false;
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                animator.Play("Base Layer.Death", 0, 0.0f);
                dead = true;
            }
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
        animator.SetBool("isIdle", true);
		yield return new WaitForSeconds (4.5f);
        idle = false;
        navMeshAgent.isStopped = false;
        animator.SetBool("isIdle", false);
    }

    IEnumerator DelayAudio()
    {
        yield return new WaitForSeconds(3.5f);
        chaseAudioPrevented = false;
    }

    IEnumerator DelayAttack()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        animator.Play("Base Layer.Attack", 0, 0.0f);
        player.DamagePlayer(20);
        //audioSource.PlayOneShot(test, 1.0f);
        yield return new WaitForSeconds(1.5f);
        attacking = false;
    }

    protected bool pathComplete() {
		if ( Vector3.Distance( navMeshAgent.destination, navMeshAgent.transform.position) <= navMeshAgent.stoppingDistance && navMeshAgent.hasPath) {	
			return true;
		}
		return false;
	}
}
