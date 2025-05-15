using UnityEngine;
using UnityEngine.UI;

public class UniversalSceneButton : MonoBehaviour
{
    [SerializeField] private bool useLoadingScreen = true;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadSceneFromName);
    }

    private void LoadSceneFromName()
    {
        string sceneName = gameObject.name.Replace("Button", "");
        SceneLoader.Instance.LoadScene(sceneName, useLoadingScreen);
    }
}
