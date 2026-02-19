/**
 * Lightweight i18n engine
 * Supports JSON locale files, nested keys, parameter interpolation,
 * browser language detection, and declarative DOM translation.
 */

type TranslationDict = Record<string, unknown>;

let currentLocale = "en";
let translations: TranslationDict = {};
let fallbackTranslations: TranslationDict = {};

/**
 * Initializes the i18n system
 * @param locale - Target locale (defaults to browser language)
 */
export async function initI18n(locale?: string): Promise<void> {
  const targetLocale = locale || navigator.language.split("-")[0] || "en";

  fallbackTranslations = await loadLocale("en");

  if (targetLocale !== "en") {
    translations = await loadLocale(targetLocale);
  } else {
    translations = fallbackTranslations;
  }

  currentLocale = targetLocale;
  translateDOM();
}

async function loadLocale(locale: string): Promise<TranslationDict> {
  try {
    const module = await import(`./locales/${locale}.json`);
    return module.default as TranslationDict;
  } catch {
    console.warn(`Locale "${locale}" not found, falling back to "en"`);
    return fallbackTranslations;
  }
}

/**
 * Translates a key to the current locale
 * @param key - Dot-notation key (e.g., 'app.name')
 * @param params - Optional parameters for interpolation (e.g., { max: 100 })
 * @returns Translated string or the key itself if not found
 */
export function t(
  key: string,
  params?: Record<string, string | number>,
): string {
  const value =
    getNestedValue(translations, key) ||
    getNestedValue(fallbackTranslations, key) ||
    key;

  if (typeof value !== "string") return key;

  if (params) {
    return value.replace(/\{(\w+)\}/g, (_, k: string) =>
      String(params[k] ?? `{${k}}`),
    );
  }
  return value;
}

function getNestedValue(obj: TranslationDict, path: string): unknown {
  return path.split(".").reduce((acc: unknown, part: string) => {
    if (acc && typeof acc === "object")
      return (acc as Record<string, unknown>)[part];
    return undefined;
  }, obj);
}

/**
 * Translates all DOM elements with data-i18n and data-i18n-placeholder attributes
 * @param root - Root element to search (defaults to document.documentElement)
 */
export function translateDOM(root: Element = document.documentElement): void {
  root.querySelectorAll("[data-i18n]").forEach((el) => {
    const key = el.getAttribute("data-i18n");
    if (key) el.textContent = t(key);
  });
  root.querySelectorAll("[data-i18n-placeholder]").forEach((el) => {
    const key = el.getAttribute("data-i18n-placeholder");
    if (key) (el as HTMLInputElement).placeholder = t(key);
  });
}

/**
 * Returns the current locale
 */
export function getLocale(): string {
  return currentLocale;
}

/**
 * Switches to a different locale and re-translates the DOM
 * @param locale - Target locale code
 */
export async function setLocale(locale: string): Promise<void> {
  await initI18n(locale);
}
