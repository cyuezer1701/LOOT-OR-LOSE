/**
 * App Button Component
 * <app-button variant="primary|success|danger" size="sm|md|lg">Label</app-button>
 */

import { BaseComponent } from "./base-component";

export class AppButton extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["variant", "size", "disabled"];
  }

  attributeChangedCallback(): void {
    this.render();
  }

  render(): void {
    const variant = this.getAttribute("variant") || "primary";
    const size = this.getAttribute("size") || "md";
    const disabled = this.hasAttribute("disabled");

    const colors: Record<string, string> = {
      primary:
        "background: linear-gradient(135deg, #1565c0, #0d47a1); color: #fff;",
      success:
        "background: linear-gradient(135deg, #10b981, #059669); color: #fff;",
      danger:
        "background: linear-gradient(135deg, #ef4444, #dc2626); color: #fff;",
      ghost:
        "background: rgba(255,255,255,0.2); color: #fff; box-shadow: none;",
    };

    const sizes: Record<string, string> = {
      sm: "padding: 8px 14px; font-size: 12px;",
      md: "padding: 12px 20px; font-size: 14px;",
      lg: "padding: 18px 28px; font-size: 16px;",
    };

    this.html(`
      ${this.baseStyles()}
      <style>
        button {
          ${colors[variant] || colors.primary}
          ${sizes[size] || sizes.md}
          border: none;
          border-radius: 12px;
          font-weight: 700;
          font-family: inherit;
          cursor: ${disabled ? "not-allowed" : "pointer"};
          opacity: ${disabled ? "0.5" : "1"};
          transition: all 0.2s ease;
          width: 100%;
        }
        button:hover:not(:disabled) {
          transform: translateY(-1px);
          filter: brightness(1.1);
        }
        button:active:not(:disabled) {
          transform: translateY(0);
        }
      </style>
      <button ${disabled ? "disabled" : ""}>
        <slot></slot>
      </button>
    `);
  }
}
