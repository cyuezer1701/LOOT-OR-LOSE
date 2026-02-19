/**
 * Test data factory helpers for integration tests.
 * Creates realistic item data and manages test state.
 */
import state from '../../src/state/app-state';
import type { Item, AppState } from '../../src/types/index';

let itemIdCounter = 1;

export function resetItemIds(): void {
  itemIdCounter = 1;
}

export function createItem(overrides: Partial<Item> = {}): Item {
  const id = itemIdCounter++;
  return {
    id: `item-${id}`,
    title: `Test Item ${id}`,
    description: 'This is test description for the item.',
    authorUid: 'test-user-1',
    createdAt: Date.now() - id * 1000,
    status: 'active',
    ...overrides,
  };
}

export function createItemList(count: number = 3): Item[] {
  return Array.from({ length: count }, () => createItem());
}

export function setupState(overrides: Partial<AppState> = {}): void {
  state.userId = 'test-user-1';
  state.items = [];
  state.unsubscribe = null;
  state.currentScreen = 'start-screen';
  Object.assign(state, overrides);
}

export function resetState(): void {
  state.userId = null;
  state.items = [];
  state.unsubscribe = null;
  state.currentScreen = 'start-screen';
}
