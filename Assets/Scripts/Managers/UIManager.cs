using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using LootOrLose.Enums;
using LootOrLose.UI;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Manages all UI screens. Creates a Canvas and EventSystem programmatically,
    /// then instantiates MainMenuUI, GameplayHUD, and GameOverUI.
    /// Subscribes to GameManager.OnStateChanged to show/hide the correct screen.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas mainCanvas;
        private MainMenuUI mainMenu;
        private GameplayHUD gameplayHUD;
        private GameOverUI gameOverUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CreateCanvas();
            CreateEventSystem();
            CreateScreens();
            SubscribeToEvents();
            ShowScreen(GameState.MainMenu);
            Debug.Log("[UIManager] UI initialized. Showing MainMenu.");
        }

        private void CreateCanvas()
        {
            var canvasGo = new GameObject("UICanvas");
            canvasGo.transform.SetParent(transform);

            mainCanvas = canvasGo.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();
        }

        private void CreateEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;

            var esGo = new GameObject("EventSystem");
            esGo.transform.SetParent(transform);
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        private GameObject CreateScreenGO(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(mainCanvas.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }

        private void CreateScreens()
        {
            var menuGo = CreateScreenGO("MainMenuUI");
            mainMenu = menuGo.AddComponent<MainMenuUI>();
            mainMenu.Build(menuGo.transform);

            var hudGo = CreateScreenGO("GameplayHUD");
            gameplayHUD = hudGo.AddComponent<GameplayHUD>();
            gameplayHUD.Build(hudGo.transform);

            var overGo = CreateScreenGO("GameOverUI");
            gameOverUI = overGo.AddComponent<GameOverUI>();
            gameOverUI.Build(overGo.transform);
        }

        private void SubscribeToEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged.AddListener(OnStateChanged);
            }
        }

        private void OnStateChanged(GameState newState)
        {
            ShowScreen(newState);
        }

        private void ShowScreen(GameState state)
        {
            bool showMenu = state == GameState.MainMenu;
            bool showHUD = state == GameState.InRun || state == GameState.BossEncounter || state == GameState.Event;
            bool showGameOver = state == GameState.RunSummary || state == GameState.Death;

            if (mainMenu != null) mainMenu.gameObject.SetActive(showMenu);
            if (gameplayHUD != null) gameplayHUD.gameObject.SetActive(showHUD);
            if (gameOverUI != null) gameOverUI.gameObject.SetActive(showGameOver);
        }
    }
}
