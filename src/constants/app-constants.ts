/* ==================== APP CONSTANTS ==================== */

export const APP_NAME: string = "My App";
export const APP_VERSION: string = "1.0.0";

/**
 * Firestore collection names
 */
export const COLLECTIONS = {
  ITEMS: "items",
} as const;

/**
 * UI constants
 */
export const UI = {
  MAX_TITLE_LENGTH: 100,
  MAX_DESCRIPTION_LENGTH: 2000,
  ITEMS_PER_PAGE: 20,
  NOTIFICATION_DURATION_MS: 3000,
} as const;

/**
 * Item status
 */
export const ITEM_STATUS = {
  ACTIVE: "active",
  ARCHIVED: "archived",
} as const;
