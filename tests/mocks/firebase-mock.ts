/**
 * Shared Firebase mocks for integration tests.
 * Import this at the top of any test file that tests Firebase-dependent modules.
 */
import { vi } from 'vitest';

// Mock firebase/firestore
vi.mock('firebase/firestore', () => ({
  collection: vi.fn(),
  doc: vi.fn((_db: unknown, _collection: string, id: string) => ({ collection: _collection, id })),
  addDoc: vi.fn(() => Promise.resolve({ id: 'mock-doc-id' })),
  deleteDoc: vi.fn(() => Promise.resolve()),
  query: vi.fn(),
  where: vi.fn(),
  orderBy: vi.fn(),
  onSnapshot: vi.fn(),
  serverTimestamp: vi.fn(() => ({ _type: 'serverTimestamp' })),
}));

// Mock firebase config
vi.mock('../../src/config/firebase', () => ({ db: {} }));

// Mock utils (keep pure functions, mock DOM-dependent ones)
vi.mock('../../src/core/utils', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../../src/core/utils')>();
  return {
    ...actual,
    notify: vi.fn(),
  };
});

// Mock UI manager
vi.mock('../../src/ui/ui-manager', () => ({
  showScreen: vi.fn(),
  showModal: vi.fn(),
  hideModal: vi.fn(),
  setStatus: vi.fn(),
}));
