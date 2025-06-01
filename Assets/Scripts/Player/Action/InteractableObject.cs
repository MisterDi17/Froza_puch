using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player.Action
{
    [RequireComponent(typeof(Collider2D))] // Заменено на 2D
    public class InteractableObject : MonoBehaviour
    {
        [Header("Настройки взаимодействия")]
        [SerializeField] private float holdTimeThreshold = 0.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("UI Prefabs")]
        [SerializeField] private GameObject iconE_Prefab;
        [SerializeField] private GameObject actionsPanel_Prefab;

        public List<MonoBehaviour> interactionActions = new List<MonoBehaviour>();

        private GameObject iconEInstance;
        private GameObject actionsPanelInstance;
        private bool playerInRange = false;
        private float holdTimer = 0f;
        private bool isHolding = false;
        private GameObject player;
        [SerializeField] private UIActionSliderManager sliderManager;

        private void Awake()
        {
            for (int i = 0; i < interactionActions.Count; i++)
            {
                if (!(interactionActions[i] is IInteractionAction))
                    Debug.LogWarning($"[InteractableObject] На объекте {name} элемент {interactionActions[i]} не реализует IInteractionAction");
            }
        }

        private void OnTriggerEnter2D(Collider2D other) // заменено на 2D
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                player = other.gameObject;
                ShowIconE(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other) // заменено на 2D
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                player = null;
                ShowIconE(false);
                CloseActionsPanel();
                ResetHold();
            }
        }

        private void Update()
        {
            if (!playerInRange) return;

            if (Input.GetKeyDown(interactKey))
            {
                isHolding = true;
                holdTimer = 0f;
            }

            if (isHolding && Input.GetKey(interactKey))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTimeThreshold)
                {
                    if (actionsPanelInstance == null)
                        OpenActionsPanel();
                }
            }

            if (Input.GetKeyUp(interactKey))
            {
                if (!IsActionsPanelOpen())
                {
                    PerformPrimaryAction();
                }
                ResetHold();
            }
        }

        private void ResetHold()
        {
            isHolding = false;
            holdTimer = 0f;
        }

        private void ShowIconE(bool show)
        {
            if (show)
            {
                if (iconEInstance == null && iconE_Prefab != null)
                {
                    iconEInstance = Instantiate(iconE_Prefab, transform);
                    iconEInstance.transform.localPosition = Vector3.up * 2f;
                }
            }
            else
            {
                if (iconEInstance != null)
                    Destroy(iconEInstance);
            }
        }

        private bool IsActionsPanelOpen()
        {
            return actionsPanelInstance != null;
        }

        private void OpenActionsPanel()
        {
            if (actionsPanel_Prefab == null || player == null) return;

            actionsPanelInstance = Instantiate(actionsPanel_Prefab, GameObject.Find("Canvas").transform);

            Transform content = actionsPanelInstance.transform.Find("ScrollView/Viewport/Content");
            if (content == null)
            {
                Debug.LogError($"[InteractableObject] Не найден Content в {actionsPanelInstance.name}");
                return;
            }

            foreach (var mb in interactionActions)
            {
                IInteractionAction action = mb as IInteractionAction;
                if (action == null) continue;

                GameObject btnGO = new GameObject("Btn_" + action.ActionName);
                btnGO.transform.SetParent(content);
                btnGO.AddComponent<RectTransform>();
                Button btn = btnGO.AddComponent<Button>();

                GameObject textGO = new GameObject("Text");
                textGO.transform.SetParent(btnGO.transform);
                Text txt = textGO.AddComponent<Text>();
                txt.text = action.ActionName;
                txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                txt.color = Color.black;
                txt.alignment = TextAnchor.MiddleLeft;

                btn.onClick.AddListener(() =>
                {
                    StartCoroutine(StartActionRoutine(action));
                    CloseActionsPanel();
                });
            }
        }

        private void CloseActionsPanel()
        {
            if (actionsPanelInstance != null)
                Destroy(actionsPanelInstance);
        }

        private void PerformPrimaryAction()
        {
            if (interactionActions.Count > 0 && player != null)
            {
                IInteractionAction primary = interactionActions[0] as IInteractionAction;
                if (primary != null)
                    StartCoroutine(StartActionRoutine(primary));
            }
        }

        private System.Collections.IEnumerator StartActionRoutine(IInteractionAction action)
        {
            sliderManager.Show(player.transform.position, action.Duration);

            float t = 0f;
            while (t < action.Duration)
            {
                t += Time.deltaTime;
                sliderManager.UpdateProgress(t / action.Duration);
                yield return null;
            }

            sliderManager.Hide();
            action.Execute(player);
        }
    }
}
