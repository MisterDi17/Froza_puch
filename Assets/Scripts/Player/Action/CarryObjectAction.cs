using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class CarryObjectAction : MonoBehaviour, IInteractionAction
{
    public string ActionName => "Взять предмет";
    public float Duration => 0f; // моментально

    public void Execute(GameObject player)
    {
        CarrySystem carrySystem = player.GetComponent<CarrySystem>();
        if (carrySystem != null)
        {
            carrySystem.PickUp(gameObject);
        }
        else
        {
            Debug.LogWarning("На игроке отсутствует CarrySystem");
        }
    }
}

