/**
 * Renderer
 * Pure DOM rendering functions for the items list.
 */

import { formatDate } from "../core/utils";
import { truncateText } from "../core/item-logic";
import type { Item } from "../types/index";

/**
 * Renders a single item card element
 * @param item - Item data object
 * @param onDelete - Delete handler
 * @returns Item card DOM element
 */
export function renderItemCard(
  item: Item,
  onDelete: (id: string) => void,
): HTMLElement {
  const card = document.createElement("div");
  card.className = "item-card glass-panel card-enter";
  card.dataset.itemId = item.id;

  const title = document.createElement("h3");
  title.className = "item-title";
  title.textContent = item.title;

  const description = document.createElement("p");
  description.className = "item-description";
  description.textContent = truncateText(item.description);

  const footer = document.createElement("div");
  footer.className = "item-footer";

  const meta = document.createElement("span");
  meta.className = "item-meta";
  const createdAt = item.createdAt as number | { toDate: () => Date };
  const timestamp =
    typeof createdAt === "object" && createdAt !== null && "toDate" in createdAt
      ? createdAt.toDate()
      : new Date(createdAt as number);
  meta.textContent = formatDate(timestamp.getTime());

  const deleteBtn = document.createElement("button");
  deleteBtn.className = "btn-delete";
  deleteBtn.textContent = "Delete";
  deleteBtn.onclick = (e: MouseEvent) => {
    e.stopPropagation();
    onDelete(item.id);
  };

  footer.append(meta, deleteBtn);
  card.append(title, description, footer);
  return card;
}

/**
 * Renders the full item list into the container
 * @param items - Array of item objects
 */
export function renderItemList(items: Item[]): void {
  const container = document.getElementById("item-list");
  if (!container) return;

  container.innerHTML = "";

  if (!items || items.length === 0) {
    const empty = document.createElement("div");
    empty.className = "empty-state";
    empty.innerHTML =
      '<div class="empty-icon">📋</div><p>No items yet. Create your first item!</p>';
    container.appendChild(empty);
    return;
  }

  items.forEach((item) => {
    const card = renderItemCard(item, handleDeleteItem);
    container.appendChild(card);
  });
}

function handleDeleteItem(itemId: string): void {
  document.dispatchEvent(
    new CustomEvent("delete-item", { detail: { itemId } }),
  );
}
