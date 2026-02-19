/**
 * App Modal Component
 * <app-modal title="My Modal">
 *   Body content
 *   <div slot="footer">Footer actions</div>
 * </app-modal>
 *
 * Use .show() and .hide() methods to control visibility.
 */

import { BaseComponent } from "./base-component";

export class AppModal extends BaseComponent {
  static get observedAttributes(): string[] {
    return ["open", "title"];
  }

  attributeChangedCallback(): void {
    this.render();
  }

  show(): void {
    this.setAttribute("open", "");
  }

  hide(): void {
    this.removeAttribute("open");
    this.emit("modal-close");
  }

  render(): void {
    const isOpen = this.hasAttribute("open");
    const title = this.getAttribute("title") || "";

    this.html(`
      ${this.baseStyles()}
      <style>
        .overlay {
          display: ${isOpen ? "flex" : "none"};
          position: fixed;
          inset: 0;
          z-index: 10000;
          background: rgba(0,0,0,0.5);
          backdrop-filter: blur(4px);
          align-items: center;
          justify-content: center;
          padding: 16px;
          animation: fadeIn 0.2s ease;
        }
        .modal {
          background: #fff;
          border-radius: 20px;
          width: 100%;
          max-width: 480px;
          max-height: 90vh;
          overflow-y: auto;
          box-shadow: 0 24px 64px rgba(0,0,0,0.3);
          animation: slideUp 0.3s ease;
        }
        .modal-header {
          padding: 24px 24px 0;
          text-align: center;
        }
        .modal-title {
          font-size: 24px;
          font-weight: 700;
          color: #1565c0;
          margin: 0;
        }
        .modal-body {
          padding: 24px;
        }
        .modal-footer {
          padding: 0 24px 24px;
        }
        @keyframes fadeIn {
          from { opacity: 0; }
          to { opacity: 1; }
        }
        @keyframes slideUp {
          from { transform: translateY(20px); opacity: 0; }
          to { transform: translateY(0); opacity: 1; }
        }
      </style>
      <div class="overlay" part="overlay">
        <div class="modal" part="modal">
          ${title ? `<div class="modal-header"><h2 class="modal-title">${title}</h2></div>` : ""}
          <div class="modal-body"><slot></slot></div>
          <div class="modal-footer"><slot name="footer"></slot></div>
        </div>
      </div>
    `);

    // Close on overlay click
    if (isOpen) {
      const overlay = this.shadow.querySelector(".overlay");
      overlay?.addEventListener("click", (e) => {
        if (e.target === overlay) this.hide();
      });
    }
  }
}
