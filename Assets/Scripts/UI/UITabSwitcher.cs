using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class TabItem
    {
        public Button button;           // Кнопка (иконка)
        public GameObject contentPanel; // Панель, которую нужно показывать
    }

    public List<TabItem> tabs;          // Все вкладки
    public GameObject contentRoot;      // Контейнер с контентом (ContentPanel)

    private TabItem currentTab;

    void Start()
    {
        foreach (var tab in tabs)
        {
            tab.button.onClick.AddListener(() => ShowTab(tab));
        }

        if (tabs.Count > 0)
            ShowTab(tabs[0]); // Открываем первую вкладку по умолчанию
    }

    public void ShowTab(TabItem tab)
    {
        if (tab == currentTab) return;

        foreach (var t in tabs)
        {
            t.contentPanel.SetActive(false);
        }

        tab.contentPanel.SetActive(true);
        currentTab = tab;

        // Убедимся, что панель с контентом видимая
        if (contentRoot != null && !contentRoot.activeSelf)
            contentRoot.SetActive(true);
    }

    public void HideContentPanel()
    {
        if (contentRoot != null)
            contentRoot.SetActive(false);
    }
}
