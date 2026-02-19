using UnityEngine;
using UnityEngine.UI;
using LootOrLose.Managers;

namespace LootOrLose.UI
{
    /// <summary>
    /// Main menu screen with title, subtitle, and start button.
    /// Dark dungeon theme with gold accents.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        public void Build(Transform parent)
        {
            // Background
            var bg = UIHelper.CreatePanel("MenuBG", transform, UIHelper.BgDark);

            // Decorative top gradient
            var topGrad = UIHelper.CreatePanel("TopGrad", bg.transform, new Color(0.15f, 0.1f, 0.25f, 0.6f));
            UIHelper.SetAnchors(topGrad, new Vector2(0, 0.7f), Vector2.one);

            // Title with shadow
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(bg.transform, false);
            var titleText = titleGo.AddComponent<Text>();
            titleText.text = "LOOT\nOR\nLOSE";
            titleText.font = UIHelper.DefaultFont;
            titleText.fontSize = 80;
            titleText.color = UIHelper.TextGold;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.lineSpacing = 0.8f;
            var titleShadow = titleGo.AddComponent<Shadow>();
            titleShadow.effectColor = new Color(0.5f, 0.3f, 0f, 0.4f);
            titleShadow.effectDistance = new Vector2(3, -3);
            UIHelper.SetAnchors(titleGo, new Vector2(0.05f, 0.55f), new Vector2(0.95f, 0.92f));

            // Separator
            UIHelper.CreateSeparator("Sep1", bg.transform, new Vector2(0.53f, 0.535f), UIHelper.Border);

            // Subtitle
            var subText = UIHelper.CreateText("Subtitle", bg.transform,
                "3 seconds to decide.\nLoot or lose it all.",
                24, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(subText.gameObject, new Vector2(0.1f, 0.43f), new Vector2(0.9f, 0.53f));

            // Start Button
            UIHelper.CreateButton("StartButton", bg.transform,
                new Vector2(0.15f, 0.28f), new Vector2(0.85f, 0.39f),
                "START RUN", UIHelper.BtnGreen, OnStartClicked, 42);

            // Separator
            UIHelper.CreateSeparator("Sep2", bg.transform, new Vector2(0.235f, 0.24f), new Color(0.2f, 0.2f, 0.3f, 0.5f));

            // Character info
            var charText = UIHelper.CreateText("CharInfo", bg.transform,
                "Warrior  |  Crypt Dungeon",
                20, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(charText.gameObject, new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.235f));

            // Version
            var verText = UIHelper.CreateText("Version", bg.transform,
                "v0.1 MVP", 16, TextAnchor.MiddleCenter, new Color(0.3f, 0.3f, 0.35f, 1f));
            UIHelper.SetAnchors(verText.gameObject, new Vector2(0, 0.01f), new Vector2(1, 0.04f));
        }

        private void OnStartClicked()
        {
            if (DataManager.Instance == null || GameManager.Instance == null)
            {
                Debug.LogError("[MainMenuUI] Managers not ready.");
                return;
            }

            var characters = DataManager.Instance.AllCharacters;
            var biomes = DataManager.Instance.AllBiomes;

            if (characters == null || characters.Count == 0 || biomes == null || biomes.Count == 0)
            {
                Debug.LogError("[MainMenuUI] No character or biome data loaded.");
                return;
            }

            GameManager.Instance.StartNewRun(characters[0], biomes[0]);
        }
    }
}
