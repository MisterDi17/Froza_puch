using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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
        [SerializeField] private GameObject actionSliderPrefab; // Префаб ActionSlider
        [SerializeField] private Transform sliderContainer;     // Контейнер, куда будем его помещат
        [SerializeField] private Font testFont;

        public List<MonoBehaviour> interactionActions = new List<MonoBehaviour>();
        private UIActionSliderManager currentSliderManager;
        private GameObject iconEInstance;
        private GameObject actionsPanelInstance;
        private bool playerInRange = false;
        private float holdTimer = 0f;
        private bool isHolding = false;
        private GameObject player;
        private UIManager uiManager;
        private Coroutine currentActionCoroutine;
        private bool isInActionZone = true;


        private void Awake()
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
        }
        private void OnTriggerEnter2D(Collider2D other) // заменено на 2D
        {
            if (other.CompareTag("Player"))
            {
                isInActionZone = true;
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

                isInActionZone = false;

                if (currentActionCoroutine != null)
                {
                    StopCoroutine(currentActionCoroutine);
                    currentActionCoroutine = null;

                    // Удалим UI
                    if (currentSliderManager != null)
                        Destroy(currentSliderManager.gameObject.transform.parent.gameObject);
                }
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
                    iconEInstance.transform.localPosition = Vector3.zero;
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
            Debug.Log("[OpenActionsPanel] Вызов метода");

            if (actionsPanel_Prefab == null)
            {
                Debug.LogError("[OpenActionsPanel] actionsPanel_Prefab == NULL !");
                return;
            }

            if (player == null)
            {
                Debug.LogError("[OpenActionsPanel] player == null");
                return;
            }

            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("[OpenActionsPanel] Не найден Canvas");
                return;
            }

            // 👉 СНАЧАЛА создаём панель
            actionsPanelInstance = Instantiate(actionsPanel_Prefab, canvas.transform);
            Debug.Log($"[OpenActionsPanel] Панель создана: {actionsPanelInstance.name}");

            // 👉 ПОТОМ ищем Content уже внутри созданного
            Transform content = actionsPanelInstance.transform.Find("ScrollView/Viewport/Content");

            if (content == null)
            {
                Debug.LogError("[OpenActionsPanel] Не найден путь ScrollView/Viewport/Content в " + actionsPanelInstance.name);
                return;
            }

            // Далее лог по кнопкам:
            foreach (var mb in interactionActions)
            {
                if (mb == null) continue;

                IInteractionAction action = mb as IInteractionAction;
                if (action == null) continue;

                Debug.Log($"[OpenActionsPanel] Добавляется кнопка для действия: {action.ActionName}");

                // Создание кнопки
                GameObject btnGO = new GameObject("Btn_" + action.ActionName);
                btnGO.transform.SetParent(content, false);

                RectTransform rect = btnGO.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(350f, 50f); // Устанавливаем ширину кнопки

                Button btn = btnGO.AddComponent<Button>();

                GameObject textGO = new GameObject("Text");
                textGO.transform.SetParent(btnGO.transform, false);

                Text txt = textGO.AddComponent<Text>();
                txt.text = action.ActionName;
                txt.fontSize = 28;
                txt.font = testFont;
                txt.color = Color.white;
                txt.alignment = TextAnchor.MiddleLeft;

                btn.onClick.AddListener(() =>
                {
                    currentActionCoroutine = StartCoroutine(StartActionRoutine(action));
                    CloseActionsPanel();
                });

                // 👉 Устанавливаем нормальные позиции/размер
                RectTransform textRT = textGO.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = Vector2.zero;
                textRT.offsetMax = Vector2.zero;
                textRT.localScale = Vector3.one;
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
        private IEnumerator StartActionRoutine(IInteractionAction action)
        {
            if (actionSliderPrefab == null || sliderContainer == null || player == null)
            {
                Debug.LogError("[InteractableObject] Префаб или контейнер не назначены!");
                yield break;
            }

            // 👉 Удаляем предыдущий слайдер, если уже есть
            if (currentSliderManager != null)
            {
                Destroy(currentSliderManager.gameObject.transform.parent.gameObject); // Удаляем FollowTarget_UI
                currentSliderManager = null;
            }

            // 1. Создаём FollowTarget_UI с RectTransform
            GameObject followGO = new GameObject("FollowTarget_UI", typeof(RectTransform));
            followGO.transform.SetParent(sliderContainer, false);

            FollowTarget follow = followGO.AddComponent<FollowTarget>();
            follow.target = player.transform;
            follow.offset = new Vector3(0, 0.75f, 0); // Сдвиг над игроком

            // 2. Создаём слайдер внутри FollowTarget
            GameObject sliderGO = Instantiate(actionSliderPrefab, followGO.transform);
            currentSliderManager = sliderGO.GetComponent<UIActionSliderManager>();

            if (currentSliderManager == null)
            {
                Debug.LogError("[InteractableObject] Префаб не содержит UIActionSliderManager!");
                yield break;
            }

            currentSliderManager.Show(player.transform, action.Duration);

            // 3. Прогресс выполнения действия
            float t = 0f;
            while (t < action.Duration && isInActionZone)
            {
                t += Time.deltaTime;
                currentSliderManager.UpdateProgress(t / action.Duration);
                yield return null;
            }

            // 4. Удаление слайдера после завершения
            if (isInActionZone)
            {
                action.Execute(player);
            }

            Destroy(followGO);
            currentSliderManager = null;
            currentActionCoroutine = null;

        }
    }
}
