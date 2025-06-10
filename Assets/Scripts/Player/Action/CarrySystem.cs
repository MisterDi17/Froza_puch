using UnityEngine;

public class CarrySystem : MonoBehaviour
{
    [SerializeField] private Transform carryPoint; // точка, куда притягивается предмет
    private GameObject carriedObject;

    void Update()
    {
        if (carriedObject != null)
        {
            carriedObject.transform.position = Vector3.Lerp(
                carriedObject.transform.position,
                carryPoint.position,
                Time.deltaTime * 10f
            );

            if (Input.GetKeyDown(KeyCode.E))
            {
                Drop();
            }
        }
    }

    public void PickUp(GameObject target)
    {
        if (carriedObject != null) Drop(); // если уже несем что-то, сбросить
        carriedObject = target;
        var rb = carriedObject.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false; // отключим физику, если есть
    }

    public void Drop()
    {
        if (carriedObject != null)
        {
            var rb = carriedObject.GetComponent<Rigidbody2D>();
            if (rb) rb.simulated = true; // включим обратно физику
            carriedObject = null;
        }
    }
}
