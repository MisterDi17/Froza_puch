using System.Collections.Generic;

[System.Serializable]
public class PatrolRoute
{
    public string routeName;
    public List<RoutePoint> points = new();
}