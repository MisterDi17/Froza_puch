using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player.Action
{
    [RequireComponent(typeof(Collider))]
    public class InteractableObject : MonoBehaviour
    {
        [Header("Настройки взаимодействия")]
        [SerializeField] private float holdTimeThreshold = 0.5f; // держим Е, чтобы открыть все действия
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("UI Prefabs")]
        [SerializeField] private GameObject iconE_Prefab;       // Простая иконка "Е" над объектом
        [SerializeField] private GameObject actionsPanel_Prefab; // UI панель со списком действий

        // Список действий, которые этот объект поддерживает (можно заполнить в инспекторе или кодом)
        public List<MonoBehaviour> interactionActions = new List<MonoBehaviour>();
        // Мы считаем, что каждый добавленный MonoBehaviour реализует IInteractionAction

        // Внутренние поля
        private GameObject iconEInstance;
        private GameObject actionsPanelInstance;
        private bool playerInRange = false;
        private float holdTimer = 0f;
        private bool isHolding = false;
        private GameObject player; // ссылка на игрока, чтобы передавать в Execute
        [SerializeField] private UIActionSliderManager sliderManager;

        private void Awake()
        {
            // Опционально: проверить, что все элементы списка interactionActions реализуют IInteractionAction
            for (int i = 0; i < interactionActions.Count; i++)
            {
                if (!(interactionActions[i] is IInteractionAction))
                    Debug.LogWarning($"[InteractableObject] На объекте {name} элемент {interactionActions[i]} не реализует IInteractionAction");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                player = other.gameObject;
                ShowIconE(true);
            }
        }

        private void OnTriggerExit(Collider other)
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

            // Если находимся в зоне иконки, следим за нажатием E
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
                    // держим дольше threshold — открываем список всех действий, если он ещё не открыт
                    if (actionsPanelInstance == null)
                        OpenActionsPanel();
                }
            }

            if (Input.GetKeyUp(interactKey))
            {
                if (!IsActionsPanelOpen())
                {
                    // Это короткий тап
                    PerformPrimaryAction();
                }
                // Если панель открыта, тап E не закрывает её — мы её закрываем только программно
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
                    // допустим, мы хотим иконку над головой объекта. Можно настроить положение.
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
            // Предположим, у нас есть Canvas в сцене c именем "Canvas"
            // Внутри prefab-а лежит ScrollView или VerticalLayoutGroup для кнопок

            // Найдём внутри панели контент, куда будем добавлять кнопки
            Transform content = actionsPanelInstance.transform.Find("ScrollView/Viewport/Content");
            if (content == null)
            {
                Debug.LogError($"[InteractableObject] Не найден Content в {actionsPanelInstance.name}");
                return;
            }

            // Для каждого IInteractionAction создаём кнопку
            foreach (var mb in interactionActions)
            {
                IInteractionAction action = mb as IInteractionAction;
                if (action == null) continue;

                // Создаём UI-кнопку (можно заранее прописать prefab Button)
                GameObject btnGO = new GameObject("Btn_" + action.ActionName);
                btnGO.transform.SetParent(content);
                btnGO.AddComponent<RectTransform>();
                Button btn = btnGO.AddComponent<Button>();

                // Текст кнопки
                Text txt = btnGO.AddComponent<Text>();
                txt.text = action.ActionName;
                txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                txt.color = Color.black;
                txt.alignment = TextAnchor.MiddleLeft;

                // Добавляем обработчик нажатия
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
            // Если в списке действий есть хотя бы один — выполняем первый
            if (interactionActions.Count > 0 && player != null)
            {
                IInteractionAction primary = interactionActions[0] as IInteractionAction;
                if (primary != null)
                    StartCoroutine(StartActionRoutine(primary));
            }
        }

        private System.Collections.IEnumerator StartActionRoutine(IInteractionAction action)
        {
            // Тут мы запускаем UI-элемент SliderAction над игроком
            sliderManager.Show(player.transform.position, action.Duration);

            // Ждём, пока Slider "докатится"
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
