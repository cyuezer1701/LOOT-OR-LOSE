using System.Collections.Generic;
using UnityEngine;

namespace LootOrLose.Managers
{
    /// <summary>
    /// Manages internationalization (i18n) and localization for all user-facing strings.
    /// Loads locale JSON files from Resources/Locales/ and provides a t() function
    /// for resolving localization keys to translated strings.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        // ─────────────────────────────────────────────
        //  Configuration
        // ─────────────────────────────────────────────

        /// <summary>
        /// Supported locale codes. MVP supports English and German.
        /// Future: "fr" (French), "es" (Spanish).
        /// </summary>
        public static readonly string[] SupportedLocales = { "en", "de" };

        private const string DefaultLocale = "en";
        private const string LocaleResourcePath = "Locales";

        // ─────────────────────────────────────────────
        //  Runtime State
        // ─────────────────────────────────────────────

        [Header("Localization Settings")]
        [SerializeField] private string currentLocale = "";

        /// <summary>
        /// The loaded locale data. Key = locale code, Value = dictionary of translation key-value pairs.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> localeData =
            new Dictionary<string, Dictionary<string, string>>();

        // ─────────────────────────────────────────────
        //  Public Properties
        // ─────────────────────────────────────────────

        /// <summary>
        /// The currently active locale code (e.g., "en", "de").
        /// </summary>
        public string CurrentLocale => currentLocale;

        // ─────────────────────────────────────────────
        //  Events
        // ─────────────────────────────────────────────

        /// <summary>
        /// Invoked when the active locale changes. UI elements should subscribe
        /// to this event to refresh their displayed text.
        /// </summary>
        public event System.Action<string> OnLocaleChanged;

        // ─────────────────────────────────────────────
        //  Lifecycle
        // ─────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Determine initial locale from system language or fall back to default
            if (string.IsNullOrEmpty(currentLocale))
            {
                currentLocale = DetectSystemLocale();
            }

            // Always load the fallback locale (English)
            LoadLocale(DefaultLocale);

            // Load the active locale if different from the fallback
            if (currentLocale != DefaultLocale)
            {
                LoadLocale(currentLocale);
            }

            Debug.Log($"[LocalizationManager] Initialized with locale: {currentLocale}");
        }

        // ─────────────────────────────────────────────
        //  Locale Detection
        // ─────────────────────────────────────────────

        /// <summary>
        /// Detect the system language and map it to a supported locale code.
        /// Falls back to "en" if the system language is not supported.
        /// </summary>
        /// <returns>The detected locale code.</returns>
        private string DetectSystemLocale()
        {
            SystemLanguage lang = Application.systemLanguage;

            switch (lang)
            {
                case SystemLanguage.German:
                    return "de";
                case SystemLanguage.French:
                    return "fr";
                case SystemLanguage.Spanish:
                    return "es";
                case SystemLanguage.English:
                default:
                    return DefaultLocale;
            }
        }

        // ─────────────────────────────────────────────
        //  Locale Loading
        // ─────────────────────────────────────────────

        /// <summary>
        /// Load a locale JSON file from Resources/Locales/{locale}.json.
        /// The JSON file should be a flat object with string key-value pairs.
        /// </summary>
        /// <param name="locale">The locale code to load (e.g., "en", "de").</param>
        private void LoadLocale(string locale)
        {
            if (localeData.ContainsKey(locale))
            {
                return; // Already loaded
            }

            string path = $"{LocaleResourcePath}/{locale}";
            TextAsset json = Resources.Load<TextAsset>(path);

            if (json == null)
            {
                Debug.LogWarning($"[LocalizationManager] Locale file not found: Resources/{path}");
                return;
            }

            var translations = ParseLocaleJson(json.text);
            if (translations != null)
            {
                localeData[locale] = translations;
                Debug.Log($"[LocalizationManager] Loaded locale '{locale}' with {translations.Count} keys.");
            }
            else
            {
                Debug.LogWarning($"[LocalizationManager] Failed to parse locale file: {locale}");
            }
        }

        /// <summary>
        /// Parse a flat JSON object into a string dictionary.
        /// Unity's JsonUtility does not support Dictionary deserialization,
        /// so we use a simple manual parser for flat key-value JSON.
        /// </summary>
        /// <param name="jsonText">The raw JSON text to parse.</param>
        /// <returns>A dictionary of key-value pairs, or null on failure.</returns>
        private Dictionary<string, string> ParseLocaleJson(string jsonText)
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Use a simple approach: wrap in a helper class
                // Since JsonUtility cannot handle Dictionary<string,string>,
                // we parse the JSON manually for flat key-value structures.
                var wrapper = JsonUtility.FromJson<LocaleWrapper>(jsonText);
                if (wrapper != null && wrapper.entries != null)
                {
                    foreach (var entry in wrapper.entries)
                    {
                        if (!string.IsNullOrEmpty(entry.key))
                        {
                            result[entry.key] = entry.value ?? "";
                        }
                    }
                    return result;
                }
            }
            catch
            {
                // JsonUtility wrapper approach failed, try manual parsing
            }

            // Fallback: minimal manual JSON parsing for flat {"key": "value"} objects
            return ParseFlatJson(jsonText);
        }

        /// <summary>
        /// Minimal parser for flat JSON objects with string-only key-value pairs.
        /// Handles simple {"key1": "value1", "key2": "value2"} structures.
        /// </summary>
        /// <param name="jsonText">The raw JSON text.</param>
        /// <returns>A dictionary of parsed key-value pairs.</returns>
        private Dictionary<string, string> ParseFlatJson(string jsonText)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(jsonText)) return result;

            // Remove outer braces
            string trimmed = jsonText.Trim();
            if (trimmed.StartsWith("{")) trimmed = trimmed.Substring(1);
            if (trimmed.EndsWith("}")) trimmed = trimmed.Substring(0, trimmed.Length - 1);

            // State machine for parsing key-value pairs
            bool inString = false;
            bool escaped = false;
            bool parsingKey = true;
            var currentToken = new System.Text.StringBuilder();
            string currentKey = null;

            for (int i = 0; i < trimmed.Length; i++)
            {
                char c = trimmed[i];

                if (escaped)
                {
                    currentToken.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inString)
                {
                    escaped = true;
                    currentToken.Append(c);
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                    continue;
                }

                if (!inString)
                {
                    if (c == ':')
                    {
                        currentKey = currentToken.ToString().Trim();
                        currentToken.Clear();
                        parsingKey = false;
                        continue;
                    }

                    if (c == ',')
                    {
                        if (currentKey != null)
                        {
                            result[currentKey] = currentToken.ToString().Trim();
                            currentKey = null;
                        }
                        currentToken.Clear();
                        parsingKey = true;
                        continue;
                    }

                    if (char.IsWhiteSpace(c) && !inString)
                        continue;
                }

                currentToken.Append(c);
            }

            // Handle the last key-value pair
            if (currentKey != null)
            {
                result[currentKey] = currentToken.ToString().Trim();
            }

            return result;
        }

        // ─────────────────────────────────────────────
        //  Translation
        // ─────────────────────────────────────────────

        /// <summary>
        /// Translate a localization key to the current locale's string.
        /// Falls back to English if the key is not found in the current locale.
        /// Returns the raw key if no translation exists in any locale.
        /// </summary>
        /// <param name="key">The localization key to look up.</param>
        /// <returns>The translated string, or the key itself if not found.</returns>
        public string t(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";

            // Try current locale
            if (localeData.TryGetValue(currentLocale, out var currentTranslations))
            {
                if (currentTranslations.TryGetValue(key, out string value))
                    return value;
            }

            // Fallback to English
            if (currentLocale != DefaultLocale && localeData.TryGetValue(DefaultLocale, out var fallbackTranslations))
            {
                if (fallbackTranslations.TryGetValue(key, out string value))
                    return value;
            }

            // No translation found - return the key itself
            Debug.LogWarning($"[LocalizationManager] Missing translation for key: {key} (locale: {currentLocale})");
            return key;
        }

        /// <summary>
        /// Translate a localization key with string.Format interpolation.
        /// The translated string may contain {0}, {1}, etc. placeholders.
        /// Falls back to English, then to the raw key.
        /// </summary>
        /// <param name="key">The localization key to look up.</param>
        /// <param name="args">Format arguments to interpolate into the translated string.</param>
        /// <returns>The formatted translated string, or the key itself if not found.</returns>
        public string t(string key, params object[] args)
        {
            string template = t(key);

            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (System.FormatException)
            {
                Debug.LogWarning($"[LocalizationManager] Format error for key: {key} with {args.Length} args.");
                return template;
            }
        }

        // ─────────────────────────────────────────────
        //  Locale Switching
        // ─────────────────────────────────────────────

        /// <summary>
        /// Switch the active locale to a new language.
        /// Loads the locale file if not already cached, then fires the OnLocaleChanged event.
        /// </summary>
        /// <param name="locale">The locale code to switch to (e.g., "en", "de").</param>
        public void SetLocale(string locale)
        {
            if (string.IsNullOrEmpty(locale)) return;
            if (locale == currentLocale) return;

            // Validate that this is a known/supported locale
            bool supported = false;
            foreach (string supported_locale in SupportedLocales)
            {
                if (supported_locale == locale)
                {
                    supported = true;
                    break;
                }
            }

            if (!supported)
            {
                Debug.LogWarning($"[LocalizationManager] Locale '{locale}' is not in the supported list. " +
                                 "Attempting to load anyway.");
            }

            // Load the locale data if not already loaded
            LoadLocale(locale);

            string previousLocale = currentLocale;
            currentLocale = locale;

            Debug.Log($"[LocalizationManager] Locale changed: {previousLocale} -> {currentLocale}");
            OnLocaleChanged?.Invoke(currentLocale);
        }

        // ─────────────────────────────────────────────
        //  Utility
        // ─────────────────────────────────────────────

        /// <summary>
        /// Check if a specific translation key exists in the current locale or the fallback.
        /// </summary>
        /// <param name="key">The localization key to check.</param>
        /// <returns>True if the key has a translation available.</returns>
        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            if (localeData.TryGetValue(currentLocale, out var currentTranslations))
            {
                if (currentTranslations.ContainsKey(key))
                    return true;
            }

            if (currentLocale != DefaultLocale && localeData.TryGetValue(DefaultLocale, out var fallbackTranslations))
            {
                if (fallbackTranslations.ContainsKey(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the total number of translation keys loaded for the current locale.
        /// </summary>
        /// <returns>The number of loaded translation keys.</returns>
        public int GetKeyCount()
        {
            if (localeData.TryGetValue(currentLocale, out var translations))
            {
                return translations.Count;
            }
            return 0;
        }
    }

    // ─────────────────────────────────────────────
    //  JSON Helper Classes
    //  Unity's JsonUtility requires concrete classes
    //  for deserialization (no Dictionary support).
    // ─────────────────────────────────────────────

    /// <summary>
    /// Wrapper class for deserializing locale JSON files via JsonUtility.
    /// Expects JSON in the format: { "entries": [ { "key": "...", "value": "..." }, ... ] }
    /// </summary>
    [System.Serializable]
    public class LocaleWrapper
    {
        /// <summary>Array of key-value translation entries.</summary>
        public LocaleEntry[] entries;
    }

    /// <summary>
    /// A single key-value translation entry within a locale file.
    /// </summary>
    [System.Serializable]
    public class LocaleEntry
    {
        /// <summary>The localization key (e.g., "menu_start_game").</summary>
        public string key;

        /// <summary>The translated string value for this key.</summary>
        public string value;
    }
}
