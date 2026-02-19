using UnityEngine;
using UnityEngine.UI;
using LootOrLose.Enums;
using LootOrLose.Managers;
using LootOrLose.State;

namespace LootOrLose.UI
{
    /// <summary>
    /// Game over screen with score display, run stats, and play again button.
    /// Dark red theme with gold score highlight.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        private Text titleText;
        private Text scoreText;
        private Text statsText;
        private Text deathCauseText;

        public void Build(Transform parent)
        {
            // Dark background with red tint
            var bg = UIHelper.CreatePanel("GameOverBG", transform, new Color(0.08f, 0.04f, 0.04f, 1f));

            // Red vignette at top
            var topVignette = UIHelper.CreatePanel("TopVignette", bg.transform, new Color(0.3f, 0.05f, 0.05f, 0.5f));
            UIHelper.SetAnchors(topVignette, new Vector2(0, 0.75f), Vector2.one);

            // "GAME OVER" title
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(bg.transform, false);
            titleText = titleGo.AddComponent<Text>();
            titleText.text = "GAME\nOVER";
            titleText.font = UIHelper.DefaultFont;
            titleText.fontSize = 72;
            titleText.color = UIHelper.TextRed;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            titleText.lineSpacing = 0.85f;
            var titleShadow = titleGo.AddComponent<Shadow>();
            titleShadow.effectColor = new Color(0.5f, 0, 0, 0.4f);
            titleShadow.effectDistance = new Vector2(3, -3);
            UIHelper.SetAnchors(titleGo, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.95f));

            // Separator
            UIHelper.CreateSeparator("Sep1", bg.transform, new Vector2(0.705f, 0.71f), new Color(0.5f, 0.15f, 0.1f, 0.6f));

            // Score card
            var scoreCard = UIHelper.CreateCardPanel("ScoreCard", bg.transform,
                new Color(0.12f, 0.08f, 0.06f, 0.95f), UIHelper.TextGold, 2f);
            UIHelper.SetAnchors(scoreCard, new Vector2(0.1f, 0.54f), new Vector2(0.9f, 0.7f));
            var scoreCardInner = scoreCard.transform.GetChild(0);

            var scoreLabelText = UIHelper.CreateText("ScoreLabel", scoreCardInner,
                "FINAL SCORE", 20, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(scoreLabelText.gameObject, new Vector2(0, 0.6f), new Vector2(1, 1f));

            scoreText = UIHelper.CreateText("Score", scoreCardInner,
                "0", 52, TextAnchor.MiddleCenter, UIHelper.TextGold);
            scoreText.fontStyle = FontStyle.Bold;
            UIHelper.SetAnchors(scoreText.gameObject, new Vector2(0, 0f), new Vector2(1, 0.65f));

            // Stats
            var statsCard = UIHelper.CreatePanel("StatsCard", bg.transform, new Color(0.1f, 0.08f, 0.08f, 0.8f));
            UIHelper.SetAnchors(statsCard, new Vector2(0.1f, 0.3f), new Vector2(0.9f, 0.52f));

            statsText = UIHelper.CreateText("Stats", statsCard.transform,
                "", 24, TextAnchor.MiddleCenter, UIHelper.TextWhite);

            // Death cause
            deathCauseText = UIHelper.CreateText("DeathCause", bg.transform,
                "", 18, TextAnchor.MiddleCenter, UIHelper.TextGray);
            UIHelper.SetAnchors(deathCauseText.gameObject, new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.3f));

            // Play Again Button
            UIHelper.CreateButton("PlayAgainBtn", bg.transform,
                new Vector2(0.15f, 0.1f), new Vector2(0.85f, 0.22f),
                "PLAY AGAIN", UIHelper.BtnBlue, OnPlayAgainClicked, 40);

            // Subscribe
            if (GameManager.Instance != null)
                GameManager.Instance.OnRunEnded.AddListener(OnRunEnded);
        }

        private void OnRunEnded(RunResult result)
        {
            scoreText.text = result.finalScore.ToString("N0");

            statsText.text = $"Rounds: {result.roundsCompleted}    " +
                             $"Bosses: {result.bossesDefeated}\n" +
                             $"Looted: {result.itemsLooted}    " +
                             $"Left: {result.itemsLeft}";

            if (!string.IsNullOrEmpty(result.deathCause))
            {
                string cause = result.deathCause;
                if (LocalizationManager.Instance != null)
                    cause = LocalizationManager.Instance.t(result.deathCause);
                deathCauseText.text = cause;
            }
        }

        private void OnPlayAgainClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
}
