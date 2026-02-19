/**
 * App Card Component
 * <app-card elevation="sm|md|lg">
 *   <div slot="header">Header</div>
 *   Body content
 *   <div slot="footer">Footer</div>
 * </app-card>
 */

import { BaseComponent } from "./base-component";

export class AppCard extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["elevation"];
  }

  attributeChangedCallback(): void {
    this.render();
  }

  render(): void {
    const elevation = this.getAttribute("elevation") || "md";

    const shadows: Record<string, string> = {
      sm: "0 2px 8px rgba(0,0,0,0.1)",
      md: "0 8px 32px rgba(31,38,135,0.15)",
      lg: "0 16px 48px rgba(31,38,135,0.25)",
    };

    this.html(`
      ${this.baseStyles()}
      <style>
        .card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: 16px;
          border: 1px solid rgba(255,255,255,0.2);
          box-shadow: ${shadows[elevation] || shadows.md};
          overflow: hidden;
          transition: all 0.2s ease;
        }
        .card:hover {
          transform: translateY(-2px);
          box-shadow: ${shadows.lg};
        }
        .header {
          padding: 16px 20px;
          border-bottom: 1px solid rgba(0,0,0,0.06);
        }
        .body {
          padding: 20px;
        }
        .footer {
          padding: 12px 20px;
          border-top: 1px solid rgba(0,0,0,0.06);
        }
        ::slotted([slot="header"]) {
          font-weight: 700;
          color: #1a1a1a;
        }
      </style>
      <div class="card">
        <div class="header"><slot name="header"></slot></div>
        <div class="body"><slot></slot></div>
        <div class="footer"><slot name="footer"></slot></div>
      </div>
    `);
  }
}
