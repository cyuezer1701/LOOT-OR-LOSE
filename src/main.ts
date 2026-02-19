/**
 * App Template - Main Entry Point
 *
 * This module initializes Firebase auth, sets up the render bridge,
 * and wires up all application modules.
 */

// Styles
import "./styles/main.css";
import "./styles/components.css";
import "./styles/ui.css";
import "./styles/animations.css";

// Firebase
import { auth } from "./config/firebase";
import { signInAnonymously } from "firebase/auth";
import { initConnectionMonitor } from "./services/connection-monitor";

// State
import state from "./state/app-state";

// UI
import { showScreen } from "./ui/ui-manager";
import { notify } from "./core/utils";

// i18n
import { initI18n } from "./i18n/i18n";
import { getSavedLocale } from "./i18n/locale-switcher";

// Web Components
import { registerComponents } from "./components";

// App modules
import { initAppHandlers } from "./ui/event-handlers";
import { renderItemList } from "./ui/renderer";

/* ==================== FIREBASE AUTH ==================== */

console.log("Connecting to Firebase...");
try {
  await signInAnonymously(auth);
  state.userId = auth.currentUser!.uid;
  console.log("Firebase connected! User ID:", state.userId);
} catch (error) {
  console.error("Firebase connection error:", error);
  notify("Firebase connection error: " + (error as Error).message, "error");
}

initConnectionMonitor();

/* ==================== I18N & COMPONENTS ==================== */

registerComponents();
await initI18n(getSavedLocale());

/* ==================== RENDER BRIDGE ==================== */

function renderApp(): void {
  renderItemList(state.items);
}

/* ==================== INITIALIZE ALL MODULES ==================== */

initAppHandlers(renderApp);

// Remove skeleton loader
const skeleton = document.getElementById("skeleton-loader");
if (skeleton) {
  skeleton.style.transition = "opacity 0.3s";
  skeleton.style.opacity = "0";
  setTimeout(() => skeleton.remove(), 300);
}

showScreen("start-screen");

console.log("App initialized");
