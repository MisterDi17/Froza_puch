using System.Collections.Generic;
using UnityEngine;
using TMPro; // обязательно для TMP_Dropdown

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown selectActorBX; // заменено Dropdown -> TMP_Dropdown
    [SerializeField] private Transform acterRoot; // объект "Acter", внутри которого находятся все актёры

    public GameObject CurrentActor { get; private set; }

    private void Start()
    {
        RefreshActorList();
        selectActorBX.onValueChanged.AddListener(OnActorSelected);
    }

    public void RefreshActorList()
    {
        selectActorBX.ClearOptions();
        List<string> options = new List<string>();
        foreach (Transform child in acterRoot)
        {
            options.Add(child.name);
        }
        selectActorBX.AddOptions(options);

        // По умолчанию выбираем первого
        if (acterRoot.childCount > 0)
            CurrentActor = acterRoot.GetChild(0).gameObject;
    }

    private void OnActorSelected(int index)
    {
        if (index >= 0 && index < acterRoot.childCount)
        {
            CurrentActor = acterRoot.GetChild(index).gameObject;
        }
    }
}
