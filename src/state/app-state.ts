/**
 * Shared application state singleton.
 * All modules import this object and read/write its properties directly.
 */
import type { AppState } from "../types/index";

const state: AppState = {
  userId: null,
  items: [],
  unsubscribe: null,
  currentScreen: "start-screen",
};

export default state;
