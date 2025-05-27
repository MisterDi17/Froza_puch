using UnityEngine;

public class NPSTargetManager : MonoBehaviour
{
    public PatrolRoute currentRoute;
    public int currentIndex;
    private ControlNPS npc;

    private void Start()
    {
        npc = GetComponentInParent<ControlNPS>();
        if (currentRoute != null && currentRoute.points.Count > 0)
        {
            npc.SetTargetPosition(currentRoute.points[0].position);
        }
    }

    private void Update()
    {
        // простая логика патрулирования
        if (currentRoute == null || currentRoute.points.Count == 0) return;

        Vector2 npcPos = npc.transform.position;
        Vector2 targetPos = currentRoute.points[currentIndex].position;

        if (Vector2.Distance(npcPos, targetPos) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % currentRoute.points.Count;
            npc.SetTargetPosition(currentRoute.points[currentIndex].position);
        }
    }

    public void AssignRoute(PatrolRoute route)
    {
        currentRoute = route;
        currentIndex = 0;
    }
}
