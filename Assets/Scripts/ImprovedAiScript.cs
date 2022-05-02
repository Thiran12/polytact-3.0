using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ImprovedAiScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public int ArmyPower = 0;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;
    public float playerEscapeRadius = 6f;
    public float playerCombatRange = .5f;
    public GameObject GameController;
    public GameObject ArmyStrIndicator;
    public GameObject DeadPrefab;
    public GameObject AliveTarget;

    public bool OutputDebug = false;

    public float ViewRadius = 15;
    public float ViewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float EdgeAvoidanceDist = 0.5f;
    public GameObject GuardPoint;
    public Transform[] waypoints;
    public bool IsDead = false;

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
        GameController = GameObject.Find("GameController");
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
        if (!IsDead)
        {
            if (ArmyStrIndicator != null)
            {
                var tmp = ArmyStrIndicator.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.SetText(string.Format("{0}", ArmyPower));
                }
            }

            // If we have a game controller.
            if (GameController != null)
            {
                // And the game state is telling us to stop movement.
                if (GameController.GetComponent<GameState>().StopAiMovement)
                {
                    // Return
                    return;
                }
            }

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
                if (GuardPoint == null)
                {
                    Patroling();
                }
                else
                {
                    navMeshAgent.SetDestination(GuardPoint.transform.position);
                    Move(speedWalk);
                }
            }
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

        var myPos = new Vector3 { x = transform.position.x, y = 0, z = transform.position.z };
        var distToPlayer = Vector2.Distance(new Vector2(myPos.x, myPos.z), new Vector2(m_PlayerPosition.x, m_PlayerPosition.z));
        if (m_WaitTime <= 0 && distToPlayer >= playerEscapeRadius)
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
            if(distToPlayer <= playerCombatRange)
            {
                Stop();
                Caughtplayer();
                m_WaitTime -= Time.deltaTime;
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
        navMeshAgent.isStopped = true;
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
        if (GameController != null)
        {
            var gameState = GameController.GetComponent<GameState>();
            if (gameState != null)
            {
                gameState.SetCurrentEnemy(gameObject);
            }
        }
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

        if (playerInRange.Length > 0)
        {
            for (int i = 0; i < playerInRange.Length; i++)
            {
                Transform player = playerInRange[i].transform;
                var playerArmy = playerInRange[i].GetComponent<Movescript>();
                var canPersue = true;
                if (playerArmy != null)
                {
                    canPersue = !playerArmy.IsFleeing;
                }

                if (canPersue)
                {
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
                        m_PlayerPosition = player.transform.position;
                    }
                } else
                {
                    m_IsPatrol = true;
                }
            }
        } else
        {
            m_playerInRange = false;
            //// m_IsPatrol = true;
        }
    }

    public void SetIsDead()
    {
        IsDead = true;
        GameObject.Destroy(AliveTarget);
        if (DeadPrefab != null)
        {
            GameObject.Instantiate(DeadPrefab, this.gameObject.transform);
        }

        if (ArmyStrIndicator != null)
        {
            ArmyStrIndicator.SetActive(false);
        }
    }
}
