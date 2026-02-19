/**
 * Component Registry
 * Registers all custom elements used in the application.
 */

import { AppButton } from "./app-button";
import { AppCard } from "./app-card";
import { AppModal } from "./app-modal";
import { AppNotification } from "./app-notification";
import { AppInput } from "./app-input";
import { AppEmptyState } from "./app-empty-state";

export {
  AppButton,
  AppCard,
  AppModal,
  AppNotification,
  AppInput,
  AppEmptyState,
};

/**
 * Registers all Web Components as custom elements.
 * Call this once during app initialization, before other init functions.
 */
export function registerComponents(): void {
  const components: [string, CustomElementConstructor][] = [
    ["app-button", AppButton],
    ["app-card", AppCard],
    ["app-modal", AppModal],
    ["app-notification", AppNotification],
    ["app-input", AppInput],
    ["app-empty-state", AppEmptyState],
  ];

  for (const [name, constructor] of components) {
    if (!customElements.get(name)) {
      customElements.define(name, constructor);
    }
  }
}
