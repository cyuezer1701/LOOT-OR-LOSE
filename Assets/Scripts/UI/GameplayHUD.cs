using UnityEngine;
using UnityEngine.UI;
using LootOrLose.Data;
using LootOrLose.Enums;
using LootOrLose.Managers;
using LootOrLose.State;

namespace LootOrLose.UI
{
    /// <summary>
    /// In-game HUD: HP bar, round counter, item card with rarity border,
    /// animated timer bar, and LOOT/LEAVE action buttons.
    /// </summary>
    public class GameplayHUD : MonoBehaviour
    {
        // Top bar
        private Text hpText;
        private Image hpBar;
        private Text roundText;
        private Text invText;
        private Text scoreText;

        // Item card
        private GameObject itemCard;
        private Image itemCardBorder;
        private Text itemNameText;
        private Text itemCategoryText;
        private Text itemStatsText;
        private Text slotSizeText;

        // Timer
        private Text timerText;
        private Image timerBarFill;
        private Image timerBarBg;

        // Buttons
        private Button lootButton;
        private Button leaveButton;

        // Boss panel
        private GameObject bossPanel;
        private Text bossNameText;
        private Text bossResultText;

        // Background
        private Image bgImage;

        public void Build(Transform parent)
        {
            // Dark background
            var root = UIHelper.CreatePanel("HUDRoot", transform, UIHelper.BgDark);
            bgImage = root.GetComponent<Image>();

            // ─── Top Stats Bar ───────────────────────────────
            var topBar = UIHelper.CreatePanel("TopBar", root.transform, UIHelper.BgTopBar);
            UIHelper.SetAnchors(topBar, new Vector2(0, 0.91f), Vector2.one);

            // HP Bar background
            var hpBarBg = UIHelper.CreatePanel("HPBarBG", topBar.transform, new Color(0.2f, 0.05f, 0.05f, 0.8f));
            UIHelper.SetAnchors(hpBarBg, new Vector2(0.02f, 0.15f), new Vector2(0.4f, 0.45f));

            // HP Bar fill
            var hpBarFill = UIHelper.CreatePanel("HPBarFill", hpBarBg.transform, UIHelper.HpGreen);
            hpBar = hpBarFill.GetComponent<Image>();
            hpBar.type = Image.Type.Filled;
            hpBar.fillMethod = Image.FillMethod.Horizontal;

            // HP Text
            hpText = UIHelper.CreateText("HPText", topBar.transform, "100/100", 22, TextAnchor.MiddleLeft, UIHelper.TextWhite);
            UIHelper.SetAnchors(hpText.gameObject, new Vector2(0.03f, 0.5f), new Vector2(0.4f, 0.95f));

            // Round
            roundText = UIHelper.CreateText("Round", topBar.transform, "ROUND 1", 26, TextAnchor.MiddleCenter, UIHelper.TextGold);
            roundText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(roundText.gameObject, new Vector2(0.35f, 0.1f), new Vector2(0.65f, 0.95f));

            // Inventory
            invText = UIHelper.CreateText("Inv", topBar.transform, "0/5", 22, TextAnchor.MiddleRight, UIHelper.TextWhite);
            UIHelper.SetAnchors(invText.gameObject, new Vector2(0.65f, 0.5f), new Vector2(0.98f, 0.95f));

            // Score
            scoreText = UIHelper.CreateText("Score", topBar.transform, "Score: 0", 18, TextAnchor.MiddleRight, UIHelper.TextGray);
            UIHelper.SetAnchors(scoreText.gameObject, new Vector2(0.65f, 0.05f), new Vector2(0.98f, 0.5f));

            // ─── Item Card ───────────────────────────────────
            itemCard = UIHelper.CreateCardPanel("ItemCard", root.transform, UIHelper.BgCard, UIHelper.Border, 3f);
            UIHelper.SetAnchors(itemCard, new Vector2(0.06f, 0.42f), new Vector2(0.94f, 0.88f));
            itemCardBorder = itemCard.GetComponent<Image>();

            var cardInner = itemCard.transform.GetChild(0);

            // Rarity banner at top of card
            var rarityBanner = UIHelper.CreatePanel("RarityBanner", cardInner, new Color(0.15f, 0.12f, 0.25f, 0.9f));
            UIHelper.SetAnchors(rarityBanner, new Vector2(0, 0.82f), Vector2.one);

            itemCategoryText = UIHelper.CreateText("Category", rarityBanner.transform,
                "WEAPON | COMMON", 22, TextAnchor.MiddleCenter, UIHelper.TextGray);

            // Item name (big, centered)
            itemNameText = UIHelper.CreateText("ItemName", cardInner,
                "Item Name", 44, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            itemNameText.fontStyle = FontStyle.Bold;
            var nameShadow = itemNameText.gameObject.AddComponent<Shadow>();
            nameShadow.effectColor = new Color(0, 0, 0, 0.6f);
            nameShadow.effectDistance = new Vector2(2, -2);
            UIHelper.SetAnchors(itemNameText.gameObject, new Vector2(0.05f, 0.45f), new Vector2(0.95f, 0.82f));

            // Separator
            UIHelper.CreateSeparator("CardSep", cardInner, new Vector2(0.43f, 0.44f), UIHelper.Border);

            // Stats
            itemStatsText = UIHelper.CreateText("Stats", cardInner,
                "ATK: +0  DEF: +0  HP: +0", 28, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            UIHelper.SetAnchors(itemStatsText.gameObject, new Vector2(0.05f, 0.15f), new Vector2(0.95f, 0.43f));

            // Slot size indicator
            slotSizeText = UIHelper.CreateText("SlotSize", cardInner,
                "", 18, TextAnchor.LowerRight, UIHelper.TextGray);
            UIHelper.SetAnchors(slotSizeText.gameObject, new Vector2(0.7f, 0.02f), new Vector2(0.97f, 0.15f));

            // ─── Timer ───────────────────────────────────────
            var timerBgGo = UIHelper.CreatePanel("TimerBG", root.transform, new Color(0.15f, 0.15f, 0.2f, 0.9f));
            UIHelper.SetAnchors(timerBgGo, new Vector2(0.06f, 0.36f), new Vector2(0.94f, 0.4f));
            timerBarBg = timerBgGo.GetComponent<Image>();

            var timerFillGo = UIHelper.CreatePanel("TimerFill", timerBgGo.transform, UIHelper.TimerFull);
            timerBarFill = timerFillGo.GetComponent<Image>();
            timerBarFill.type = Image.Type.Filled;
            timerBarFill.fillMethod = Image.FillMethod.Horizontal;

            timerText = UIHelper.CreateText("TimerText", root.transform,
                "3.0", 28, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            timerText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(timerText.gameObject, new Vector2(0.4f, 0.31f), new Vector2(0.6f, 0.36f));

            // ─── Action Buttons ──────────────────────────────
            lootButton = UIHelper.CreateButton("LootBtn", root.transform,
                new Vector2(0.06f, 0.12f), new Vector2(0.47f, 0.28f),
                "LOOT", UIHelper.BtnGreen, () => OnDecision(DecisionResult.Loot), 44);

            leaveButton = UIHelper.CreateButton("LeaveBtn", root.transform,
                new Vector2(0.53f, 0.12f), new Vector2(0.94f, 0.28f),
                "LEAVE", UIHelper.BtnRed, () => OnDecision(DecisionResult.Leave), 44);

            // ─── Loot/Leave labels ───────────────────────────
            var lootHint = UIHelper.CreateText("LootHint", root.transform,
                "Add to inventory", 16, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(lootHint.gameObject, new Vector2(0.06f, 0.08f), new Vector2(0.47f, 0.12f));

            var leaveHint = UIHelper.CreateText("LeaveHint", root.transform,
                "Skip this item", 16, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(leaveHint.gameObject, new Vector2(0.53f, 0.08f), new Vector2(0.94f, 0.12f));

            // ─── Boss Panel (hidden) ─────────────────────────
            bossPanel = UIHelper.CreateCardPanel("BossPanel", root.transform,
                new Color(0.2f, 0.05f, 0.05f, 0.97f), new Color(0.6f, 0.15f, 0.1f, 0.9f), 4f);
            UIHelper.SetAnchors(bossPanel, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.85f));
            var bossInner = bossPanel.transform.GetChild(0);

            var bossLabel = UIHelper.CreateText("BossLabel", bossInner,
                "BOSS ENCOUNTER", 22, TextAnchor.MiddleCenter, UIHelper.TextRed);
            UIHelper.SetAnchors(bossLabel.gameObject, new Vector2(0.05f, 0.8f), new Vector2(0.95f, 0.97f));

            bossNameText = UIHelper.CreateText("BossName", bossInner,
                "Boss", 48, TextAnchor.MiddleCenter, UIHelper.TextWhite);
            bossNameText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(bossNameText.gameObject, new Vector2(0.05f, 0.4f), new Vector2(0.95f, 0.8f));

            bossResultText = UIHelper.CreateText("BossResult", bossInner,
                "", 26, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(bossResultText.gameObject, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.4f));

            bossPanel.SetActive(false);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnItemPresented.AddListener(OnItemPresented);
            GameManager.Instance.OnBossEncounter.AddListener(OnBossEncounter);
            GameManager.Instance.OnDecisionMade.AddListener(OnDecisionMade);
            GameManager.Instance.OnStateChanged.AddListener(OnStateChanged);
        }

        private void OnItemPresented(ItemData item)
        {
            if (item == null) return;

            itemCard.SetActive(true);
            bossPanel.SetActive(false);

            string name = GetLocalizedOrKey(item.nameKey);
            itemNameText.text = name;
            itemCategoryText.text = $"{item.category}  |  {item.rarity}".ToUpper();

            // Stats line
            string stats = "";
            if (item.attackPower > 0) stats += $"ATK +{item.attackPower}   ";
            if (item.defensePower > 0) stats += $"DEF +{item.defensePower}   ";
            if (item.healAmount > 0) stats += $"HP +{item.healAmount}   ";
            if (item.isCursed) stats += "CURSED";
            if (string.IsNullOrEmpty(stats)) stats = "No combat stats";
            itemStatsText.text = stats;

            // Slot size
            slotSizeText.text = item.slotSize > 1 ? $"{item.slotSize} SLOTS" : "1 SLOT";

            // Rarity colors
            Color rarityColor;
            switch (item.rarity)
            {
                case ItemRarity.Legendary: rarityColor = UIHelper.RarityLegendary; break;
                case ItemRarity.Rare: rarityColor = UIHelper.RarityRare; break;
                case ItemRarity.Uncommon: rarityColor = UIHelper.RarityUncommon; break;
                default: rarityColor = UIHelper.RarityCommon; break;
            }

            itemNameText.color = rarityColor;
            itemCardBorder.color = rarityColor * 0.6f;
            itemCategoryText.color = rarityColor * 0.8f;

            SetButtonsInteractable(true);
        }

        private void OnBossEncounter(BossData boss)
        {
            itemCard.SetActive(false);
            bossPanel.SetActive(true);
            SetButtonsInteractable(false);
            bossNameText.text = GetLocalizedOrKey(boss.nameKey);
            bossResultText.text = "Resolving combat...";
        }

        private void OnDecisionMade(DecisionResult decision, ItemData item)
        {
            SetButtonsInteractable(false);
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.InRun)
                bossPanel.SetActive(false);
        }

        private void OnDecision(DecisionResult decision)
        {
            if (RunManager.Instance != null)
                RunManager.Instance.ProcessDecision(decision);
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var run = GameManager.Instance.CurrentRun;
            if (run == null) return;

            // HP
            float hpPct = (float)run.playerHP / run.maxHP;
            hpText.text = $"{run.playerHP} / {run.maxHP}";
            hpBar.fillAmount = hpPct;
            hpBar.color = hpPct > 0.5f ? UIHelper.HpGreen : hpPct > 0.25f ? UIHelper.HpYellow : UIHelper.HpRed;

            // Round
            roundText.text = $"ROUND {run.currentRound}";

            // Inventory
            invText.text = $"{run.GetInventorySlotUsage()} / {run.maxInventorySlots}";

            // Score
            scoreText.text = $"Score: {run.score}";

            // Timer
            if (RunManager.Instance != null && RunManager.Instance.IsTimerActive)
            {
                float remaining = RunManager.Instance.TimerRemaining;
                float pct = remaining / 3f;
                timerText.text = $"{remaining:F1}s";
                timerBarFill.fillAmount = pct;

                if (pct > 0.6f)
                {
                    timerBarFill.color = UIHelper.TimerFull;
                    timerText.color = UIHelper.TextWhite;
                }
                else if (pct > 0.3f)
                {
                    timerBarFill.color = UIHelper.TimerMid;
                    timerText.color = UIHelper.TimerMid;
                }
                else
                {
                    timerBarFill.color = UIHelper.TimerLow;
                    timerText.color = UIHelper.TimerLow;
                }
            }
            else
            {
                timerText.text = "";
                timerBarFill.fillAmount = 0f;
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            if (lootButton != null) lootButton.interactable = interactable;
            if (leaveButton != null) leaveButton.interactable = interactable;
        }

        private string GetLocalizedOrKey(string key)
        {
            if (LocalizationManager.Instance != null)
                return LocalizationManager.Instance.t(key);
            return key;
        }
    }
}
