/**
 * App Input Component
 * <app-input label="Title" placeholder="Enter..." type="text|textarea" maxlength="100"></app-input>
 */

import { BaseComponent } from "./base-component";

export class AppInput extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["label", "placeholder", "type", "maxlength", "value", "error"];
  }

  attributeChangedCallback(): void {
    this.render();
  }

  get value(): string {
    const input = this.shadow.querySelector("input, textarea") as
      | HTMLInputElement
      | HTMLTextAreaElement
      | null;
    return input?.value || "";
  }

  set value(val: string) {
    const input = this.shadow.querySelector("input, textarea") as
      | HTMLInputElement
      | HTMLTextAreaElement
      | null;
    if (input) input.value = val;
  }

  render(): void {
    const label = this.getAttribute("label") || "";
    const placeholder = this.getAttribute("placeholder") || "";
    const type = this.getAttribute("type") || "text";
    const maxlength = this.getAttribute("maxlength") || "";
    const error = this.getAttribute("error") || "";
    const isTextarea = type === "textarea";

    this.html(`
      ${this.baseStyles()}
      <style>
        .form-group { margin-bottom: 0; }
        label {
          display: block;
          font-size: 14px;
          font-weight: 600;
          color: #374151;
          margin-bottom: 6px;
        }
        input, textarea {
          width: 100%;
          background: rgba(255,255,255,0.8);
          border: 2px solid ${error ? "#ef4444" : "#e5e7eb"};
          border-radius: 12px;
          padding: 12px 16px;
          font-size: ${isTextarea ? "14px" : "16px"};
          font-family: inherit;
          color: #1a1a1a;
          transition: all 0.2s;
        }
        textarea {
          min-height: 120px;
          resize: vertical;
        }
        input:focus, textarea:focus {
          outline: none;
          border-color: #3b82f6;
          background: #fff;
        }
        .error-text {
          color: #ef4444;
          font-size: 12px;
          margin-top: 4px;
        }
      </style>
      <div class="form-group">
        ${label ? `<label>${label}</label>` : ""}
        ${
          isTextarea
            ? `<textarea placeholder="${placeholder}" ${maxlength ? `maxlength="${maxlength}"` : ""}></textarea>`
            : `<input type="text" placeholder="${placeholder}" ${maxlength ? `maxlength="${maxlength}"` : ""} />`
        }
        ${error ? `<div class="error-text">${error}</div>` : ""}
      </div>
    `);

    // Emit input-change event
    const inputEl = this.shadow.querySelector("input, textarea");
    inputEl?.addEventListener("input", () => {
      this.emit("input-change", { value: (inputEl as HTMLInputElement).value });
    });
  }
}
