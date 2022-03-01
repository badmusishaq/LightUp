using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class monster : MonoBehaviour
{
    public GameObject player;

    public Transform eyes;

    public Transform target;

    public AudioSource growl;

    public GameObject deathCam;

    public GameObject MainCamera;

    public Transform camPos;

    private NavMeshAgent nav;

    private string state = "idle";

    private bool alive = true;

    private bool highAlert = false;

    private float alertness = 20f;

    private float wait = 0f;

    // public int x = 0;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

    }

    public void checkSight()
    {
        if (alive)
        {
            RaycastHit rayHit;
            if (Physics.Linecast(eyes.position, player.transform.position, out rayHit))
            {
                // print("hit " + rayHit.collider.gameObject.name);
                if (rayHit.collider.gameObject.name == "Player")
                {
                    if (state != "kill")
                    {
                        state = "chase";
                        growl.pitch = 1.2f;
                        growl.Play();
                    }
                }
            }
        }
    }



    void Update()
    {

        //Debug.DrawLine(eyes.position, player.transform.position, Color.green);

        if (alive)
        {
            if (state == "idle")
            {
                Vector3 randomPos = Random.insideUnitSphere * alertness;
                NavMeshHit navHit;
                NavMesh.SamplePosition(transform.position + randomPos, out navHit, 20f, NavMesh.AllAreas);

                if (highAlert)
                {
                    NavMesh.SamplePosition(player.transform.position + randomPos, out navHit, 20f, NavMesh.AllAreas);
                    alertness += 5f;

                    if (alertness > 20f)
                    {
                        highAlert = false;
                    }
                }

                nav.SetDestination(navHit.position);
                state = "walk";
            }

            if (state == "walk")
            {
                if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending)
                {
                    state = "search";
                    wait = 5f;
                }
            }

            if (state == "search")
            {
                if (wait > 0f)
                {
                    wait -= Time.deltaTime;
                    transform.Rotate(0f, 120f * Time.deltaTime, 0f);
                    checkSight();
                }

                else
                {
                    state = "idle";
                }
            }

            if (state == "chase")
            {
                nav.destination = player.transform.position;

                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance > 20f)
                {
                    state = "hunt";
                }
                
                else if (nav.remainingDistance <= nav.stoppingDistance + 1f && !nav.pathPending)
                {
                    if(player.GetComponent<Player>().alive)
                    {
                        state = "kill";
                        player.GetComponent<Player>().alive = false;
                        player.GetComponent<FirstPersonAIO>().enabled = false;
                        deathCam.SetActive(true);
                        //deathCam.transform.position = Camera.main.transform.position;
                        //deathCam.transform.rotation = Camera.main.transform.rotation;
                        MainCamera.SetActive(false);
                        //Camera.main.gameObject.SetActive(false);
                        growl.pitch = 0.4f;
                        growl.Play();
                        Invoke("reset", 1f);
                    }
                }
            }

            if (state == "hunt")
            {
                if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending)
                {
                    state = "search";
                    wait = 5f;
                    highAlert = true;
                    alertness = 5f;
                    checkSight();
                }
            }

            if (state == "kill")
            {
              //  deathCam.transform.position = Vector3.Slerp(deathCam.transform.position, camPos.position, 10f * Time.deltaTime);
              //  deathCam.transform.rotation = Quaternion.Slerp(deathCam.transform.rotation, camPos.rotation, 10f * Time.deltaTime);
                nav.SetDestination(deathCam.transform.position);
            }

            // nav.SetDestination(player.transform.position);


            if (target != null)
            {
                transform.LookAt(target);
                transform.Rotate(0, 90, 0);
            }
        }
    }

    void reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ScoringSystem.theScore = 0;
    }
}
