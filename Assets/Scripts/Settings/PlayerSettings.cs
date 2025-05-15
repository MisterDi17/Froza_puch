using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
    [Header("Components")]
    private GameMovement movement;
    private GameRotation rotation;
    private Animator animator;

    [Header("Aiming")]
    [SerializeField] private float aimSlowdown = 0.3f;
    [SerializeField] private float mouseDeadzone = 0.1f;

    // Методы инициализации
    public void SetMovement(GameMovement m) => movement = m;
    public void SetRotation(GameRotation r) => rotation = r;
    public void SetAnimator(Animator a) => animator = a;

    // Публичные свойства
    public GameMovement Movement => movement;
    public GameRotation Rotation => rotation;
    public Animator Animator => animator;
    public float AimSlowdown => aimSlowdown;
    public float MouseDeadzone => mouseDeadzone;

    /// <summary>
    /// Находит все IAbility в дочерних объектах Player.
    /// </summary>
    public List<IAbility> GetAbilities(MonoBehaviour parent)
    {
        var list = new List<IAbility>();
        foreach (var mb in parent.GetComponentsInChildren<MonoBehaviour>())
            if (mb is IAbility ia)
                list.Add(ia);
        return list;
    }
}
