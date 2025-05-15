using UnityEngine;
using UnityEngine.AI;

public class ControlNPS : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void Update()
    {
        if (target != null)
        {
            // NPC продолжает двигаться к цели, автоматически обходя препятствия
            agent.SetDestination(target.position);
        }
    }
}
