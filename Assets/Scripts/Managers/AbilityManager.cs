using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField]
    private List<MonoBehaviour> abilitySources = new List<MonoBehaviour>();

    private List<IAbility> abilities = new List<IAbility>();

    private float lastDashTapTime;  
    private const float doubleTapThreshold = 0.3f;

    private void Awake()
    {
        abilities = abilitySources.OfType<IAbility>().ToList();

        if (abilities.Count == 0)
            Debug.LogWarning("AbilityManager: не найдено ни одной IAbility.");
    }

    private void Update()
    {
        var input = GameInput.Instance;
        if (input == null) return;

        if (input.IsDashPressed())
        {
            var dash = abilities.OfType<DashAbility>().FirstOrDefault();
            if (dash != null && dash.IsEnabled)
            {
                float now = Time.time;
                if (now - lastDashTapTime <= doubleTapThreshold)
                {
                    Vector2 dir = input.GetMovementVector();
                    dash.Execute(dir.magnitude > 0.1f ? (Vector2?)dir : null);
                }
                lastDashTapTime = now;
            }
        }
    }

    public void AddAbility(MonoBehaviour mb)
    {
        if (mb is IAbility ability && !abilities.Contains(ability))
        {
            abilities.Add(ability);
            abilitySources.Add(mb);
        }
    }

    public void RemoveAbility(MonoBehaviour mb)
    {
        if (mb is IAbility ability && abilities.Contains(ability))
        {
            abilities.Remove(ability);
            abilitySources.Remove(mb);
        }
    }

    public void ToggleAbility(string abilityName, bool enable)
    {
        var ability = abilities.FirstOrDefault(a => a.GetType().Name == abilityName);
        if (ability != null)
            ability.IsEnabled = enable;
    }

    public bool IsAbilityEnabled(string abilityName)
    {
        var ability = abilities.FirstOrDefault(a => a.GetType().Name == abilityName);
        return ability?.IsEnabled ?? false;
    }

    public void UseAbility(string abilityName, Vector2? dir = null)
    {
        var ability = abilities.FirstOrDefault(a => a.GetType().Name == abilityName);
        if (ability != null && ability.IsEnabled)
            ability.Execute(dir);
    }

    public List<string> GetAbilityNames() =>
        abilities.Select(a => a.GetType().Name).ToList();
}
