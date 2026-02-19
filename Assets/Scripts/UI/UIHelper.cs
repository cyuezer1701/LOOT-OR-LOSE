using UnityEngine;
using UnityEngine.UI;

namespace LootOrLose.UI
{
    /// <summary>
    /// Shared UI factory methods for building programmatic UI elements
    /// with consistent styling across all screens.
    /// </summary>
    public static class UIHelper
    {
        // ─── Color Palette (Dark Dungeon Theme) ──────────────────
        public static readonly Color BgDark = new Color(0.06f, 0.06f, 0.1f, 1f);
        public static readonly Color BgMedium = new Color(0.1f, 0.1f, 0.16f, 1f);
        public static readonly Color BgCard = new Color(0.12f, 0.12f, 0.2f, 0.97f);
        public static readonly Color BgTopBar = new Color(0.04f, 0.04f, 0.08f, 0.9f);
        public static readonly Color Border = new Color(0.3f, 0.25f, 0.4f, 0.8f);

        public static readonly Color TextWhite = new Color(0.95f, 0.93f, 0.9f, 1f);
        public static readonly Color TextGray = new Color(0.6f, 0.58f, 0.55f, 1f);
        public static readonly Color TextGold = new Color(1f, 0.84f, 0f, 1f);
        public static readonly Color TextRed = new Color(0.95f, 0.25f, 0.2f, 1f);

        public static readonly Color BtnGreen = new Color(0.15f, 0.6f, 0.25f, 1f);
        public static readonly Color BtnRed = new Color(0.65f, 0.15f, 0.15f, 1f);
        public static readonly Color BtnBlue = new Color(0.15f, 0.4f, 0.65f, 1f);
        public static readonly Color BtnDisabled = new Color(0.25f, 0.25f, 0.3f, 1f);

        public static readonly Color RarityCommon = TextWhite;
        public static readonly Color RarityUncommon = new Color(0.3f, 0.85f, 0.35f, 1f);
        public static readonly Color RarityRare = new Color(0.35f, 0.55f, 1f, 1f);
        public static readonly Color RarityLegendary = TextGold;

        public static readonly Color TimerFull = new Color(0.2f, 0.75f, 0.3f, 1f);
        public static readonly Color TimerMid = new Color(0.95f, 0.6f, 0.1f, 1f);
        public static readonly Color TimerLow = new Color(0.95f, 0.2f, 0.15f, 1f);

        public static readonly Color HpGreen = new Color(0.2f, 0.8f, 0.3f, 1f);
        public static readonly Color HpYellow = new Color(0.9f, 0.8f, 0.2f, 1f);
        public static readonly Color HpRed = new Color(0.9f, 0.2f, 0.15f, 1f);

        // ─── Font ────────────────────────────────────────────────
        private static Font _cachedFont;
        public static Font DefaultFont
        {
            get
            {
                if (_cachedFont == null)
                    _cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return _cachedFont;
            }
        }

        // ─── Panel ───────────────────────────────────────────────
        public static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }

        // ─── Bordered Panel (card-like) ──────────────────────────
        public static GameObject CreateCardPanel(string name, Transform parent, Color bgColor, Color borderColor, float borderWidth = 3f)
        {
            // Outer border
            var outer = CreatePanel(name, parent, borderColor);

            // Inner content
            var inner = CreatePanel(name + "_Inner", outer.transform, bgColor);
            var innerRect = inner.GetComponent<RectTransform>();
            innerRect.offsetMin = new Vector2(borderWidth, borderWidth);
            innerRect.offsetMax = new Vector2(-borderWidth, -borderWidth);

            return outer;
        }

        // ─── Text ────────────────────────────────────────────────
        public static Text CreateText(string name, Transform parent, string content, int fontSize, TextAnchor anchor, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<Text>();
            txt.text = content;
            txt.font = DefaultFont;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = anchor;
            txt.raycastTarget = false;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return txt;
        }

        // ─── Button ──────────────────────────────────────────────
        public static Button CreateButton(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, string label, Color bgColor,
            UnityEngine.Events.UnityAction onClick, int fontSize = 36)
        {
            var btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);

            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = bgColor;

            // Rounded feel via outline
            var outline = btnGo.AddComponent<Outline>();
            outline.effectColor = new Color(1, 1, 1, 0.15f);
            outline.effectDistance = new Vector2(2, -2);

            var btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = btnImg;

            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            btn.colors = colors;

            btn.onClick.AddListener(onClick);

            SetAnchors(btnGo, anchorMin, anchorMax);

            // Label
            var txtGo = new GameObject("Label");
            txtGo.transform.SetParent(btnGo.transform, false);
            var txt = txtGo.AddComponent<Text>();
            txt.text = label;
            txt.font = DefaultFont;
            txt.fontSize = fontSize;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = false;

            // Add shadow to text
            var shadow = txtGo.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(2, -2);

            var txtRect = txtGo.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return btn;
        }

        // ─── Anchors Helper ──────────────────────────────────────
        public static void SetAnchors(GameObject go, Vector2 min, Vector2 max)
        {
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        // ─── Separator Line ─────────────────────────────────────
        public static GameObject CreateSeparator(string name, Transform parent, Vector2 anchorY, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
            SetAnchors(go, new Vector2(0.05f, anchorY.x), new Vector2(0.95f, anchorY.y));
            return go;
        }
    }
}
