using UnityEngine;

public interface IAbility
{
    bool IsEnabled { get; set; }
    void Execute(Vector2? direction = null);
}
