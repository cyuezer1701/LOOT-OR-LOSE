using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootOrLose.Data;
using LootOrLose.Managers;

namespace LootOrLose.UI
{
    /// <summary>
    /// Main menu screen with title, character/biome selection, and start button.
    /// Dark dungeon theme with gold accents.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        // Selection state
        private List<CharacterData> characters;
        private List<BiomeData> biomes;
        private int selectedCharIndex;
        private int selectedBiomeIndex;

        // Character card texts
        private Text charNameText;
        private Text charStatsText;
        private Text charPassiveText;
        private Image charCardBorder;

        // Biome card texts
        private Text biomeNameText;
        private Text biomeDescText;
        private Image biomeCardBorder;

        // Info text
        private Text charInfoText;

        public void Build(Transform parent)
        {
            // Background
            var bg = UIHelper.CreatePanel("MenuBG", transform, UIHelper.BgDark);

            // Decorative top gradient
            var topGrad = UIHelper.CreatePanel("TopGrad", bg.transform, new Color(0.15f, 0.1f, 0.25f, 0.6f));
            UIHelper.SetAnchors(topGrad, new Vector2(0, 0.82f), Vector2.one);

            // Title with shadow
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(bg.transform, false);
            var titleText = titleGo.AddComponent<Text>();
            titleText.text = "LOOT OR LOSE";
            titleText.font = UIHelper.DefaultFont;
            titleText.fontSize = 56;
            titleText.color = UIHelper.TextGold;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            var titleShadow = titleGo.AddComponent<Shadow>();
            titleShadow.effectColor = new Color(0.5f, 0.3f, 0f, 0.4f);
            titleShadow.effectDistance = new Vector2(3, -3);
            UIHelper.SetAnchors(titleGo, new Vector2(0.05f, 0.87f), new Vector2(0.95f, 0.97f));

            // Subtitle
            var subText = UIHelper.CreateText("Subtitle", bg.transform,
                "3 seconds to decide", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(subText.gameObject, new Vector2(0.1f, 0.83f), new Vector2(0.9f, 0.87f));

            // ─── Character Selection Card ───────────────
            var charLabel = UIHelper.CreateText("CharLabel", bg.transform,
                "CHARACTER", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(charLabel.gameObject, new Vector2(0.1f, 0.76f), new Vector2(0.9f, 0.82f));

            var charCard = UIHelper.CreateCardPanel("CharCard", bg.transform,
                UIHelper.BgCard, UIHelper.Border, 2f);
            UIHelper.SetAnchors(charCard, new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.76f));
            charCardBorder = charCard.GetComponent<Image>();
            var charInner = charCard.transform.GetChild(0);

            // Left arrow button
            UIHelper.CreateButton("CharPrev", charInner,
                new Vector2(0f, 0.1f), new Vector2(0.12f, 0.9f),
                "<", new Color(0.25f, 0.25f, 0.35f, 0.9f), () => CycleCharacter(-1), 32);

            // Right arrow button
            UIHelper.CreateButton("CharNext", charInner,
                new Vector2(0.88f, 0.1f), new Vector2(1f, 0.9f),
                ">", new Color(0.25f, 0.25f, 0.35f, 0.9f), () => CycleCharacter(1), 32);

            // Character name
            charNameText = UIHelper.CreateText("CharName", charInner,
                "Warrior", 34, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            charNameText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(charNameText.gameObject, new Vector2(0.12f, 0.5f), new Vector2(0.88f, 1f));

            // Character stats (HP / Slots)
            charStatsText = UIHelper.CreateText("CharStats", charInner,
                "HP: 100  |  Slots: 5", 22, TextAnchor.MiddleCenter, UIHelper.TextGold);
            UIHelper.SetAnchors(charStatsText.gameObject, new Vector2(0.12f, 0.2f), new Vector2(0.88f, 0.55f));

            // Character passive
            charPassiveText = UIHelper.CreateText("CharPassive", charInner,
                "", 16, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(charPassiveText.gameObject, new Vector2(0.12f, 0f), new Vector2(0.88f, 0.25f));

            // ─── Biome Selection Card ───────────────────
            var biomeLabel = UIHelper.CreateText("BiomeLabel", bg.transform,
                "DUNGEON", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(biomeLabel.gameObject, new Vector2(0.1f, 0.51f), new Vector2(0.9f, 0.57f));

            var biomeCard = UIHelper.CreateCardPanel("BiomeCard", bg.transform,
                UIHelper.BgCard, UIHelper.Border, 2f);
            UIHelper.SetAnchors(biomeCard, new Vector2(0.08f, 0.37f), new Vector2(0.92f, 0.51f));
            biomeCardBorder = biomeCard.GetComponent<Image>();
            var biomeInner = biomeCard.transform.GetChild(0);

            // Left arrow button
            UIHelper.CreateButton("BiomePrev", biomeInner,
                new Vector2(0f, 0.1f), new Vector2(0.12f, 0.9f),
                "<", new Color(0.25f, 0.25f, 0.35f, 0.9f), () => CycleBiome(-1), 32);

            // Right arrow button
            UIHelper.CreateButton("BiomeNext", biomeInner,
                new Vector2(0.88f, 0.1f), new Vector2(1f, 0.9f),
                ">", new Color(0.25f, 0.25f, 0.35f, 0.9f), () => CycleBiome(1), 32);

            // Biome name
            biomeNameText = UIHelper.CreateText("BiomeName", biomeInner,
                "Crypt", 30, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            biomeNameText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(biomeNameText.gameObject, new Vector2(0.12f, 0.45f), new Vector2(0.88f, 1f));

            // Biome description
            biomeDescText = UIHelper.CreateText("BiomeDesc", biomeInner,
                "", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(biomeDescText.gameObject, new Vector2(0.12f, 0f), new Vector2(0.88f, 0.5f));

            // ─── Start Button ───────────────────────────
            UIHelper.CreateButton("StartButton", bg.transform,
                new Vector2(0.12f, 0.2f), new Vector2(0.88f, 0.33f),
                "START RUN", UIHelper.BtnGreen, OnStartClicked, 42);

            // Selected info line
            charInfoText = UIHelper.CreateText("CharInfo", bg.transform,
                "", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(charInfoText.gameObject, new Vector2(0.1f, 0.15f), new Vector2(0.9f, 0.2f));

            // Separator
            UIHelper.CreateSeparator("Sep3", bg.transform, new Vector2(0.125f, 0.13f), new Color(0.2f, 0.2f, 0.3f, 0.5f));

            // Version
            var verText = UIHelper.CreateText("Version", bg.transform,
                "v0.2 Sprint 2", 16, TextAnchor.MiddleCenter, new Color(0.3f, 0.3f, 0.35f, 1f));
            UIHelper.SetAnchors(verText.gameObject, new Vector2(0, 0.01f), new Vector2(1, 0.04f));

            // Load data and refresh UI
            LoadSelectionData();
        }

        private void LoadSelectionData()
        {
            if (DataManager.Instance == null) return;

            characters = DataManager.Instance.AllCharacters;
            biomes = DataManager.Instance.AllBiomes;
            selectedCharIndex = 0;
            selectedBiomeIndex = 0;

            RefreshCharacterDisplay();
            RefreshBiomeDisplay();
        }

        private void CycleCharacter(int direction)
        {
            if (characters == null || characters.Count == 0) return;
            selectedCharIndex = (selectedCharIndex + direction + characters.Count) % characters.Count;
            RefreshCharacterDisplay();
        }

        private void CycleBiome(int direction)
        {
            if (biomes == null || biomes.Count == 0) return;
            selectedBiomeIndex = (selectedBiomeIndex + direction + biomes.Count) % biomes.Count;
            RefreshBiomeDisplay();
        }

        private void RefreshCharacterDisplay()
        {
            if (characters == null || characters.Count == 0) return;
            var ch = characters[selectedCharIndex];

            string name = GetLocalizedOrKey(ch.nameKey);
            charNameText.text = name;
            charStatsText.text = $"HP: {ch.baseHP}  |  Slots: {ch.inventorySlots}";

            string passive = GetLocalizedOrKey(ch.passiveAbilityKey);
            charPassiveText.text = passive;

            // Locked indicator
            if (!ch.isUnlocked)
            {
                charNameText.color = UIHelper.TextGray;
                charCardBorder.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            }
            else
            {
                charNameText.color = UIHelper.TextWhite;
                charCardBorder.color = UIHelper.Border;
            }

            UpdateInfoLine();
        }

        private void RefreshBiomeDisplay()
        {
            if (biomes == null || biomes.Count == 0) return;
            var bm = biomes[selectedBiomeIndex];

            string name = GetLocalizedOrKey(bm.nameKey);
            biomeNameText.text = name;

            string desc = GetLocalizedOrKey(bm.descriptionKey);
            biomeDescText.text = desc;

            // Use biome's theme color for border
            Color biomeColor = bm.GetPrimaryColor();
            if (!bm.isUnlocked)
            {
                biomeNameText.color = UIHelper.TextGray;
                biomeCardBorder.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            }
            else
            {
                biomeNameText.color = new Color(
                    Mathf.Lerp(biomeColor.r, 1f, 0.5f),
                    Mathf.Lerp(biomeColor.g, 1f, 0.5f),
                    Mathf.Lerp(biomeColor.b, 1f, 0.5f), 1f);
                biomeCardBorder.color = biomeColor;
            }

            UpdateInfoLine();
        }

        private void UpdateInfoLine()
        {
            if (characters == null || biomes == null) return;
            if (characters.Count == 0 || biomes.Count == 0) return;

            var ch = characters[selectedCharIndex];
            var bm = biomes[selectedBiomeIndex];

            string charName = GetLocalizedOrKey(ch.nameKey);
            string biomeName = GetLocalizedOrKey(bm.nameKey);

            bool locked = !ch.isUnlocked || !bm.isUnlocked;
            charInfoText.text = locked
                ? "LOCKED — Keep playing to unlock"
                : $"{charName} in {biomeName}";
            charInfoText.color = locked ? UIHelper.TextRed : UIHelper.TextGray;
        }

        private void OnStartClicked()
        {
            if (DataManager.Instance == null || GameManager.Instance == null)
            {
                Debug.LogError("[MainMenuUI] Managers not ready.");
                return;
            }

            if (characters == null || characters.Count == 0 || biomes == null || biomes.Count == 0)
            {
                Debug.LogError("[MainMenuUI] No character or biome data loaded.");
                return;
            }

            var selectedChar = characters[selectedCharIndex];
            var selectedBiome = biomes[selectedBiomeIndex];

            // For MVP: allow playing locked characters/biomes (unlock system comes later)
            GameManager.Instance.StartNewRun(selectedChar, selectedBiome);
        }

        private void OnEnable()
        {
            // Refresh data when screen becomes visible (returning from a run)
            LoadSelectionData();
        }

        private string GetLocalizedOrKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            if (LocalizationManager.Instance != null)
                return LocalizationManager.Instance.t(key);
            return key;
        }
    }
}
