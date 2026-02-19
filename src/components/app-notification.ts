/**
 * App Notification Component
 * Provides a static method for showing toast notifications.
 *
 * Usage: AppNotification.show('Message', 'success');
 */

import { BaseComponent } from "./base-component";

export class AppNotification extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["type", "duration"];
  }

  /**
   * Static helper to show a notification programmatically
   */
  static show(
    message: string,
    type: "info" | "success" | "error" | "warning" = "info",
    duration = 3000,
  ): void {
    const notif = document.createElement("app-notification") as AppNotification;
    notif.setAttribute("type", type);
    notif.setAttribute("duration", String(duration));
    notif.textContent = message;
    document.body.appendChild(notif);
  }

  render(): void {
    const type = this.getAttribute("type") || "info";
    const duration = parseInt(this.getAttribute("duration") || "3000", 10);

    const colors: Record<string, string> = {
      info: "#3b82f6",
      success: "#10b981",
      error: "#ef4444",
      warning: "#f59e0b",
    };

    this.html(`
      ${this.baseStyles()}
      <style>
        :host {
          position: fixed;
          top: 20px;
          left: 50%;
          transform: translateX(-50%);
          z-index: 10001;
          animation: slideDown 0.3s ease;
        }
        .notification {
          background: ${colors[type] || colors.info};
          color: #fff;
          padding: 12px 24px;
          border-radius: 12px;
          font-size: 14px;
          font-weight: 600;
          font-family: inherit;
          box-shadow: 0 8px 24px rgba(0,0,0,0.2);
          white-space: nowrap;
        }
        @keyframes slideDown {
          from { transform: translateX(-50%) translateY(-100%); opacity: 0; }
          to { transform: translateX(-50%) translateY(0); opacity: 1; }
        }
      </style>
      <div class="notification"><slot></slot></div>
    `);

    setTimeout(() => {
      this.style.transition = "opacity 0.3s, transform 0.3s";
      this.style.opacity = "0";
      this.style.transform = "translateX(-50%) translateY(-100%)";
      setTimeout(() => this.remove(), 300);
    }, duration);
  }
}
