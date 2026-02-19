import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import '../mocks/firebase-mock';
import { addDoc, deleteDoc, onSnapshot, doc } from 'firebase/firestore';
import { createItem, deleteItem, subscribeToItems } from '../../src/services/item-service';
import state from '../../src/state/app-state';
import { setupState, resetState } from '../helpers/item-factory';
import type { Mock } from 'vitest';

describe('Item Service', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setupState();
  });

  afterEach(() => {
    resetState();
  });

  describe('createItem', () => {
    it('should call addDoc with correct data', async () => {
      const itemData = { title: 'Test', description: 'Description' };
      await createItem('user-1', itemData);

      expect(addDoc).toHaveBeenCalledTimes(1);
      const callArgs = (addDoc as Mock).mock.calls[0][1] as Record<string, unknown>;
      expect(callArgs.title).toBe('Test');
      expect(callArgs.description).toBe('Description');
      expect(callArgs.authorUid).toBe('user-1');
      expect(callArgs.status).toBe('active');
    });

    it('should return the document ID', async () => {
      const result = await createItem('user-1', { title: 'Test', description: '' });
      expect(result).toBe('mock-doc-id');
    });
  });

  describe('deleteItem', () => {
    it('should call deleteDoc with correct reference', async () => {
      await deleteItem('item-123');

      expect(deleteDoc).toHaveBeenCalledTimes(1);
      expect(doc).toHaveBeenCalledWith({}, 'items', 'item-123');
    });
  });

  describe('subscribeToItems', () => {
    it('should call onSnapshot to subscribe', () => {
      const renderFn = vi.fn();
      subscribeToItems('user-1', renderFn);

      expect(onSnapshot).toHaveBeenCalledTimes(1);
    });

    it('should unsubscribe previous listener', () => {
      const mockUnsubscribe = vi.fn();
      state.unsubscribe = mockUnsubscribe;

      subscribeToItems('user-1', vi.fn());

      expect(mockUnsubscribe).toHaveBeenCalledTimes(1);
    });

    it('should update state and call render on snapshot', () => {
      const renderFn = vi.fn();
      subscribeToItems('user-1', renderFn);

      // Get the onSnapshot callback
      const snapshotCallback = (onSnapshot as Mock).mock.calls[0][1] as (snapshot: {
        docs: Array<{ id: string; data: () => Record<string, unknown> }>;
      }) => void;

      // Simulate a snapshot
      const mockSnapshot = {
        docs: [
          {
            id: 'item-1',
            data: () => ({ title: 'Item 1', description: 'Desc 1', authorUid: 'user-1' }),
          },
          {
            id: 'item-2',
            data: () => ({ title: 'Item 2', description: 'Desc 2', authorUid: 'user-1' }),
          },
        ],
      };

      snapshotCallback(mockSnapshot);

      expect(state.items).toHaveLength(2);
      expect(state.items[0].id).toBe('item-1');
      expect(state.items[0].title).toBe('Item 1');
      expect(renderFn).toHaveBeenCalledTimes(1);
    });
  });
});
