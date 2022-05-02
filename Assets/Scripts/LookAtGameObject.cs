using UnityEngine;

public class LookAtGameObject : MonoBehaviour
{
    public GameObject LookAtTarget;

    // Update is called once per frame
    void Update()
    {
        if (LookAtTarget != null)
        {
            this.transform.LookAt(LookAtTarget.transform);
        }
    }
}
