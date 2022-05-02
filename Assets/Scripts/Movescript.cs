
using UnityEngine;
using UnityEngine.AI;

public class Movescript : MonoBehaviour
{
    public Camera cam;
    public int ArmyPower = 0;
    public NavMeshAgent player;
    public Animator playerAnimator;
    public GameObject targetDest;
    public GameObject GameController;
    public bool IsFleeing;

    public float FleeingTimer = 0f;
    public float FleeingTimeout = 10f;

    void Start()
    {
        GameController = GameObject.Find("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        var stopPlayer = GameController.GetComponent<GameState>().StopAiMovement;
        if (Input.GetMouseButtonDown(0) && GameController != null && !stopPlayer)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitpoint;
            
            if(Physics.Raycast(ray, out hitpoint))
            {
                //// targetDest.transform.position = hitpoint.point;
                player.SetDestination(hitpoint.point);
            }
        }

        // If we have been told to stop.
        if (GetComponent<NavMeshAgent>().remainingDistance > 0 && stopPlayer)
        {
            // Set the current destination to my current location
            GetComponent<NavMeshAgent>().SetDestination(gameObject.transform.position);
        }

        // If we are fleeing then we are not seen as a target 
        if (IsFleeing)
        {
            // Increment the fleeing timer.
            FleeingTimer += Time.deltaTime;
            if (FleeingTimer >= FleeingTimeout)
            {
                // Remove fleeing status after FleeingTimeout time.
                IsFleeing = false;
                FleeingTimer = 0f;
            }
        }

        if (ArmyPower <= 0)
        {
            GameController.GetComponent<GameState>().EndGame();
        }
    }
}
