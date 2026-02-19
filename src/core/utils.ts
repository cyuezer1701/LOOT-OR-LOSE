/**
 * Utility functions
 * General-purpose helpers used across the application.
 */

import { UI } from "../constants/app-constants";
import type { NotificationType } from "../types/index";

/**
 * Generates a random alphanumeric ID
 * @param length - Length of the ID (default: 8)
 * @returns Random ID
 */
export function generateId(length: number = 8): string {
  const chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
  let result = "";
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
}

/**
 * Formats a timestamp for display
 * @param timestamp - Unix timestamp in milliseconds
 * @returns Formatted time string
 */
export function formatTime(timestamp: number): string {
  return new Date(timestamp).toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

/**
 * Formats a date for display
 * @param timestamp - Unix timestamp in milliseconds
 * @returns Formatted date string
 */
export function formatDate(timestamp: number): string {
  return new Date(timestamp).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

/**
 * Shows a notification to the user
 * @param message - Notification message
 * @param type - Notification type: 'info', 'success', 'error', 'warning'
 */
export function notify(message: string, type: NotificationType = "info"): void {
  const notif = document.createElement("div");
  notif.className = `notification ${type}`;
  notif.textContent = message;
  document.body.appendChild(notif);

  setTimeout(() => {
    notif.style.opacity = "0";
    notif.style.transform = "translateX(-50%) translateY(-100px)";
    setTimeout(() => notif.remove(), 300);
  }, UI.NOTIFICATION_DURATION_MS || 3000);
}
