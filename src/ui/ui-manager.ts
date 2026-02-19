/**
 * UI Manager
 * Handles screen navigation and modal show/hide.
 */

/**
 * Shows a specific screen and hides all others
 * @param screenId - ID of screen to show
 */
export function showScreen(screenId: string): void {
  ["start-screen", "app-screen"].forEach((id) => {
    const el = document.getElementById(id);
    if (el) el.classList.add("hidden");
  });

  const target = document.getElementById(screenId);
  if (target) target.classList.remove("hidden");
}

/**
 * Shows a modal by ID
 * @param modalId - Modal element ID
 */
export function showModal(modalId: string): void {
  const modal = document.getElementById(modalId);
  if (modal) modal.classList.remove("hidden");
}

/**
 * Hides a modal by ID
 * @param modalId - Modal element ID
 */
export function hideModal(modalId: string): void {
  const modal = document.getElementById(modalId);
  if (modal) modal.classList.add("hidden");
}

/**
 * Sets the status text
 * @param message - Status message
 * @param isError - Whether this is an error message
 */
export function setStatus(message: string, isError: boolean = false): void {
  const statusEl = document.getElementById("status-message");
  if (statusEl) {
    statusEl.textContent = message;
    statusEl.style.color = isError ? "#ef4444" : "#94a3b8";
  }
}
