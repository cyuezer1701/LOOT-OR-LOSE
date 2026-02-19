/**
 * UI Event Handlers
 * Wires up button clicks, modal interactions, and custom events.
 */
import state from "../state/app-state";
import { showScreen, showModal, hideModal } from "./ui-manager";
import { notify } from "../core/utils";
import { validateItem } from "../core/item-logic";
import {
  createItem,
  deleteItem,
  subscribeToItems,
} from "../services/item-service";
import type { RenderCallback } from "../types/index";

let _renderApp: RenderCallback | null = null;

export function initAppHandlers(renderAppFn: RenderCallback): void {
  _renderApp = renderAppFn;

  // "Get Started" button on start screen
  const getStartedBtn = document.getElementById(
    "btn-get-started",
  ) as HTMLButtonElement;
  getStartedBtn.onclick = () => {
    showScreen("app-screen");
    if (state.userId && _renderApp) {
      subscribeToItems(state.userId, _renderApp);
    }
  };

  // "Add Item" button
  const addItemBtn = document.getElementById(
    "btn-add-item",
  ) as HTMLButtonElement;
  addItemBtn.onclick = () => {
    (document.getElementById("item-title-input") as HTMLInputElement).value =
      "";
    (
      document.getElementById("item-description-input") as HTMLTextAreaElement
    ).value = "";
    showModal("create-item-modal");
    (document.getElementById("item-title-input") as HTMLInputElement).focus();
  };

  // Save item
  const saveItemBtn = document.getElementById(
    "btn-save-item",
  ) as HTMLButtonElement;
  saveItemBtn.onclick = async () => {
    const title = (
      document.getElementById("item-title-input") as HTMLInputElement
    ).value;
    const description = (
      document.getElementById("item-description-input") as HTMLTextAreaElement
    ).value;

    const validation = validateItem({ title, description });
    if (!validation.valid) {
      notify(validation.message, "error");
      return;
    }

    try {
      await createItem(state.userId!, { title, description });
      hideModal("create-item-modal");
      notify("Item created!", "success");
    } catch (error) {
      console.error("Error creating item:", error);
      notify("Failed to create item: " + (error as Error).message, "error");
    }
  };

  // Cancel create item
  const cancelItemBtn = document.getElementById(
    "btn-cancel-item",
  ) as HTMLButtonElement;
  cancelItemBtn.onclick = () => {
    hideModal("create-item-modal");
  };

  // Delete item via custom event
  document.addEventListener("delete-item", async (e: Event) => {
    const { itemId } = (e as CustomEvent<{ itemId: string }>).detail;
    if (!confirm("Delete this item?")) return;

    try {
      await deleteItem(itemId);
      notify("Item deleted", "success");
    } catch (error) {
      console.error("Error deleting item:", error);
      notify("Failed to delete item: " + (error as Error).message, "error");
    }
  });

  // Back to start
  const backBtn = document.getElementById("btn-back-to-start");
  if (backBtn) {
    backBtn.onclick = () => {
      showScreen("start-screen");
    };
  }
}
