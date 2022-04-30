using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
public class ImprovedAiScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;

   
    public bool OutputDebug = false;

    public float ViewRadius = 15;
    public float ViewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float EdgeAvoidanceDist = 0.5f;

    public Transform[] waypoints;

    [Range(1, 500)] public float walkRadius;

    int m_CurrentWaypointIndex;

    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_playerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_playerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        NextPoint();
    }

    
    void Update()
    {
        // Have I seen the player
        EnviromentView();

        // Yes
        if (!m_IsPatrol)
        {
            // Chase him
            Chasing();
        }
        else
        {
            // No: Get random location
            Patroling();
        }
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);
        }
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            else
            {
                if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position)>= 2.5f)
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            if(m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if(m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }
    void Stop()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = 0;
    }
  public void NextPoint()
    {
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomposition = Random.insideUnitSphere * walkRadius;
        randomposition += transform.position;
        if (NavMesh.SamplePosition(randomposition, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }

        if (OutputDebug)
        {
            UnityEngine.Debug.LogFormat("Nme: {0}", finalPosition);
        }

        NavMeshHit nmHit;
        if (NavMesh.FindClosestEdge(new Vector3(finalPosition.x, this.transform.position.y, finalPosition.z), out nmHit, NavMesh.AllAreas))
        { 
            if (nmHit.distance >= EdgeAvoidanceDist)
            {
                navMeshAgent.SetDestination(new Vector3(finalPosition.x, this.transform.position.y, finalPosition.z));
            }
        } else
        {
            navMeshAgent.SetDestination(new Vector3(finalPosition.x, this.transform.position.y, finalPosition.z));
        }
    }

    void Caughtplayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if(Vector3.Distance(transform.position, player)<= 0.3)
        {
           if(m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(new Vector3(waypoints[m_CurrentWaypointIndex].position.x, this.transform.position.y, waypoints[m_CurrentWaypointIndex].position.z));
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }
    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, ViewRadius, playerMask);

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < ViewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_playerInRange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > ViewRadius)
            {
                m_playerInRange = false;
            }


            if (m_playerInRange)
            {
                m_PlayerPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
            }
        }
    }

}
