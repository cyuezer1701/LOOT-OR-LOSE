/**
 * Locale Switcher
 * Utility for switching the app language at runtime.
 * Can be wired to a UI dropdown or button.
 */

import { setLocale, getLocale } from "./i18n";

const STORAGE_KEY = "app-locale";

/**
 * Available locales in this application
 */
export const AVAILABLE_LOCALES: { code: string; label: string }[] = [
  { code: "en", label: "English" },
  { code: "de", label: "Deutsch" },
];

/**
 * Gets the saved locale from localStorage or returns undefined
 */
export function getSavedLocale(): string | undefined {
  return localStorage.getItem(STORAGE_KEY) || undefined;
}

/**
 * Switches the application locale and saves the preference
 * @param locale - Locale code (e.g., 'en', 'de')
 */
export async function switchLocale(locale: string): Promise<void> {
  localStorage.setItem(STORAGE_KEY, locale);
  await setLocale(locale);
}

/**
 * Returns the current locale code
 */
export function getCurrentLocale(): string {
  return getLocale();
}
