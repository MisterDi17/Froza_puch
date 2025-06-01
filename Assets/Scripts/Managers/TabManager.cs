using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject contentSection;
    }

    public List<Tab> tabs;

    private void Start()
    {
        foreach (var tab in tabs)
        {
            tab.tabButton.onClick.AddListener(() => OnTabSelected(tab));
        }

        // Включаем первую вкладку по умолчанию
        if (tabs.Count > 0)
            OnTabSelected(tabs[0]);
    }

    public void OnTabSelected(Tab selectedTab)
    {
        foreach (var tab in tabs)
        {
            bool isActive = tab == selectedTab;
            tab.contentSection.SetActive(isActive);
        }
    }
}
