/**
 * Connection Monitor
 * Detects browser online/offline state and shows a banner to the user.
 */

let connectionBanner: HTMLElement | null = null;

export function initConnectionMonitor(): void {
  connectionBanner = document.getElementById("connection-banner");

  window.addEventListener("offline", () => showDisconnected());
  window.addEventListener("online", () => showReconnected());

  if (!navigator.onLine) {
    showDisconnected();
  }
}

function showDisconnected(): void {
  if (!connectionBanner) return;
  connectionBanner.textContent = "Connection lost...";
  connectionBanner.className = "connection-banner disconnected";
}

function showReconnected(): void {
  if (!connectionBanner) return;
  connectionBanner.textContent = "Connected!";
  connectionBanner.className = "connection-banner connected";
  setTimeout(() => {
    connectionBanner!.classList.add("hidden");
  }, 3000);
}
