/**
 * App Empty State Component
 * <app-empty-state icon="📋" message="No items yet."></app-empty-state>
 */

import { BaseComponent } from "./base-component";

export class AppEmptyState extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["icon", "message"];
  }

  attributeChangedCallback(): void {
    this.render();
  }

  render(): void {
    const icon = this.getAttribute("icon") || "📋";
    const message = this.getAttribute("message") || "Nothing here yet.";

    this.html(`
      ${this.baseStyles()}
      <style>
        .empty-state {
          text-align: center;
          padding: 48px 24px;
          color: rgba(255,255,255,0.6);
        }
        .icon {
          font-size: 48px;
          margin-bottom: 16px;
        }
        .message {
          font-size: 16px;
          font-weight: 500;
        }
      </style>
      <div class="empty-state">
        <div class="icon">${icon}</div>
        <p class="message">${message}</p>
      </div>
    `);
  }
}
