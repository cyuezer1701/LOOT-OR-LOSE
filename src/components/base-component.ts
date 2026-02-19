/**
 * Base Component
 * Abstract base class for all Web Components in this project.
 * Provides Shadow DOM setup, templating helper, and event emission.
 */

export abstract class BaseComponent extends HTMLElement {
  protected shadow: ShadowRoot;

  constructor() {
    super();
    this.shadow = this.attachShadow({ mode: "open" });
  }

  connectedCallback(): void {
    this.render();
  }

  abstract render(): void;

  /**
   * Sets the shadow DOM innerHTML
   */
  protected html(template: string): void {
    this.shadow.innerHTML = template;
  }

  /**
   * Dispatches a typed custom event that bubbles through Shadow DOM
   */
  protected emit<T>(name: string, detail?: T): void {
    this.dispatchEvent(
      new CustomEvent(name, {
        detail,
        bubbles: true,
        composed: true,
      }),
    );
  }

  /**
   * Returns shared base styles for components
   */
  protected baseStyles(): string {
    return `
      <style>
        :host {
          display: block;
          box-sizing: border-box;
        }
        *, *::before, *::after {
          box-sizing: border-box;
        }
      </style>
    `;
  }
}
