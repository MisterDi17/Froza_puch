using System.Collections.Generic;
using UnityEngine;

public class PatrolRouteDatabase : MonoBehaviour
{
    public List<PatrolRoute> allRoutes = new();

    public PatrolRoute GetRouteByName(string name)
    {
        return allRoutes.Find(r => r.routeName == name);
    }
}
