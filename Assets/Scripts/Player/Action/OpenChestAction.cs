using UnityEngine;

public class OpenChestAction : MonoBehaviour, IInteractionAction
{
    public string ActionName => "Открыть сундук";
    public float Duration => 10f;

    public void Execute(GameObject player)
    {
        Debug.Log("Сундук открыт!");
        // логика: проиграть анимацию, выдать лут и т.д.
    }
}
