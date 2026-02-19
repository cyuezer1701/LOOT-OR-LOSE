/**
 * Core application type definitions.
 */

/** Represents an item in the application */
export interface Item {
  id: string;
  title: string;
  description: string;
  authorUid: string;
  createdAt: number | { toDate: () => Date } | { _type: string };
  status: string;
}

/** Result of validating an item */
export interface ValidationResult {
  valid: boolean;
  message: string;
}

/** Shared application state */
export interface AppState {
  userId: string | null;
  items: Item[];
  unsubscribe: (() => void) | null;
  currentScreen: string;
}

/** Notification severity type */
export type NotificationType = "info" | "success" | "error" | "warning";

/** Callback invoked after state changes to re-render the UI */
export type RenderCallback = () => void;
