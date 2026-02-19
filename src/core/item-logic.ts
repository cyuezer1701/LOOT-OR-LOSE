/**
 * Item Logic - Pure business logic functions
 * All functions are pure (no side effects, no DOM, no Firebase).
 * This makes them easy to unit test.
 */

import { UI } from "../constants/app-constants";
import type { ValidationResult } from "../types/index";

/**
 * Validates an item before saving
 * @param item - Item object { title, description }
 * @returns { valid: boolean, message: string }
 */
export function validateItem(item: unknown): ValidationResult {
  if (!item || typeof item !== "object") {
    return { valid: false, message: "Invalid item object" };
  }

  const { title, description } = item as {
    title?: string;
    description?: string;
  };

  if (!title || title.trim().length === 0) {
    return { valid: false, message: "Title is required" };
  }
  if (title.length > UI.MAX_TITLE_LENGTH) {
    return {
      valid: false,
      message: `Title must be under ${UI.MAX_TITLE_LENGTH} characters`,
    };
  }
  if (description && description.length > UI.MAX_DESCRIPTION_LENGTH) {
    return {
      valid: false,
      message: `Description must be under ${UI.MAX_DESCRIPTION_LENGTH} characters`,
    };
  }
  return { valid: true, message: "" };
}

/**
 * Sorts items by creation date (newest first)
 * @param items - Array of item objects
 * @returns Sorted copy of items array
 */
export function sortItems<T extends { createdAt?: number }>(
  items: T[] | unknown,
): T[] {
  if (!items || !Array.isArray(items)) return [];
  return [...items].sort(
    (a: T, b: T) =>
      ((b.createdAt as number) || 0) - ((a.createdAt as number) || 0),
  );
}

/**
 * Filters items by search query (matches title and description)
 * @param items - Array of item objects
 * @param query - Search query string
 * @returns Filtered items
 */
export function filterItems<T extends { title?: string; description?: string }>(
  items: T[],
  query: string | null | undefined,
): T[] {
  if (!query || query.trim().length === 0) return items;
  const lowerQuery = query.toLowerCase().trim();
  return items.filter(
    (item) =>
      (item.title && item.title.toLowerCase().includes(lowerQuery)) ||
      (item.description && item.description.toLowerCase().includes(lowerQuery)),
  );
}

/**
 * Truncates text to a maximum length with ellipsis
 * @param text - Full text
 * @param maxLength - Maximum length (default: 100)
 * @returns Truncated text with ellipsis if needed
 */
export function truncateText(
  text: string | null | undefined,
  maxLength: number = 100,
): string {
  if (!text) return "";
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength).trim() + "...";
}
