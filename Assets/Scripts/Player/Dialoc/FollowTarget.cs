using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, 0);

    void Update()
    {
        if (target)
        {
            transform.position = target.position + offset;
        }
    }
}
