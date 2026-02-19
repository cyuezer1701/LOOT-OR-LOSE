using UnityEngine;
using LootOrLose.Services.Analytics;
using LootOrLose.Services.Firebase;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Entry point for the game. Placed on a GameObject in the boot scene.
    /// Creates all manager GameObjects programmatically in Awake.
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("[Bootstrap] Initializing game managers...");

            CreateManagerIfMissing<GameManager>("GameManager");
            CreateManagerIfMissing<DataManager>("DataManager");
            CreateManagerIfMissing<RunManager>("RunManager");
            CreateManagerIfMissing<AudioManager>("AudioManager");
            CreateManagerIfMissing<LocalizationManager>("LocalizationManager");
            CreateManagerIfMissing<SaveManager>("SaveManager");
            CreateManagerIfMissing<UIManager>("UIManager");
            CreateManagerIfMissing<AnalyticsService>("AnalyticsService");
            CreateManagerIfMissing<FirebaseService>("FirebaseService");

            Debug.Log("[Bootstrap] All managers initialized.");
        }

        private void CreateManagerIfMissing<T>(string name) where T : MonoBehaviour
        {
            if (FindAnyObjectByType<T>() != null) return;

            var go = new GameObject(name);
            go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }
    }
}
