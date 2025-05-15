using UnityEngine;
using UnityEngine.UI;

public class AbilityManagerUI : MonoBehaviour
{
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private GameObject abilityEntryPrefab;

    private bool isOpen;

    private void Update()
    {
        if (GameInput.Instance != null && GameInput.Instance.IsModMenuTogglePressed())
        {
            isOpen = !isOpen;
            panelRoot.gameObject.SetActive(isOpen);
            if (isOpen) RefreshUI();
        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in panelRoot)
            Destroy(child.gameObject);

        foreach (string name in abilityManager.GetAbilityNames())
        {
            var entry = Instantiate(abilityEntryPrefab, panelRoot);
            entry.transform.Find("Label").GetComponent<Text>().text = name;

            var toggle = entry.GetComponentInChildren<Toggle>();
            toggle.isOn = abilityManager.IsAbilityEnabled(name);
            toggle.onValueChanged.AddListener(on => abilityManager.ToggleAbility(name, on));

            var btn = entry.transform.Find("UseButton")?.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => abilityManager.UseAbility(name));
        }
    }
}
