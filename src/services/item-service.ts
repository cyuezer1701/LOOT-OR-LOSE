/**
 * Item Service
 * Manages Firestore operations for items: CRUD + real-time subscription.
 */
import { db } from "../config/firebase";
import {
  collection,
  doc,
  addDoc,
  deleteDoc,
  query,
  where,
  orderBy,
  onSnapshot,
  serverTimestamp,
} from "firebase/firestore";
import { COLLECTIONS } from "../constants/app-constants";
import state from "../state/app-state";
import type { Item, RenderCallback } from "../types/index";

/**
 * Creates a new item in Firestore
 * @param userId - Author's user ID
 * @param itemData - { title, description }
 * @returns Document ID of created item
 */
export async function createItem(
  userId: string,
  itemData: { title: string; description: string },
): Promise<string> {
  const docRef = await addDoc(collection(db, COLLECTIONS.ITEMS), {
    ...itemData,
    authorUid: userId,
    createdAt: serverTimestamp(),
    status: "active",
  });
  return docRef.id;
}

/**
 * Deletes an item from Firestore
 * @param itemId - Item document ID
 */
export async function deleteItem(itemId: string): Promise<void> {
  await deleteDoc(doc(db, COLLECTIONS.ITEMS, itemId));
}

/**
 * Subscribes to real-time item updates for a user
 * @param userId - User ID to filter items by
 * @param renderCallback - Called after state updates
 */
export function subscribeToItems(
  userId: string,
  renderCallback: RenderCallback,
): void {
  if (state.unsubscribe) state.unsubscribe();

  const q = query(
    collection(db, COLLECTIONS.ITEMS),
    where("authorUid", "==", userId),
    orderBy("createdAt", "desc"),
  );

  state.unsubscribe = onSnapshot(q, (snapshot) => {
    state.items = snapshot.docs.map((d) => ({
      id: d.id,
      ...d.data(),
    })) as Item[];
    renderCallback();
  });
}
