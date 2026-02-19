# Vite + Firebase + PWA Template

A production-ready TypeScript project template for web apps with Firebase, PWA support, i18n, Web Components, and a complete CI/CD pipeline.

## Features

- **TypeScript** — Strict mode, full type coverage
- **Vite 7.3** — Lightning-fast dev server and optimized builds
- **Firebase 12.9** — Firestore real-time database + Anonymous Auth
- **PWA** — Service worker, offline fallback, installable app
- **Tailwind CSS v4** — Local Vite plugin (no CDN), `@theme` design tokens
- **i18n** — Lightweight translation engine with EN/DE support
- **Web Components** — Custom Elements library with Shadow DOM
- **CI/CD** — GitHub Actions: lint, typecheck, test, build, deploy
- **Testing** — Vitest with jsdom, unit + integration tests, Firebase mocks
- **Code Quality** — ESLint 9 (flat config) + Prettier + TypeScript strict
- **Claude Code Agents** — 4 specialized agents (Designer, PM, Dev, Tester)

## Quick Start

1. Click **"Use this template"** on GitHub
2. Clone your new repository
3. Install dependencies:

```bash
nvm use         # Uses Node 22 from .nvmrc
npm install
```

4. Set up Firebase (see below)
5. Start developing:

```bash
npm run dev     # Dev server at http://localhost:3000
```

## Firebase Setup

1. Create a project at [Firebase Console](https://console.firebase.google.com)
2. Enable **Anonymous Authentication** (Authentication > Sign-in method)
3. Create a **Cloud Firestore** database
4. Copy your Firebase config values
5. Create a `.env` file from the template:

```bash
cp .env.example .env
```

6. Fill in your Firebase credentials in `.env`
7. Update `.firebaserc` with your project ID
8. Update `deploy.yml` with your project ID and service account secret name

## Environment Variables

| Variable | Description |
|----------|-------------|
| `VITE_FIREBASE_API_KEY` | Firebase API key |
| `VITE_FIREBASE_AUTH_DOMAIN` | Firebase Auth domain |
| `VITE_FIREBASE_PROJECT_ID` | Firebase project ID |
| `VITE_FIREBASE_STORAGE_BUCKET` | Firebase storage bucket |
| `VITE_FIREBASE_MESSAGING_SENDER_ID` | Firebase messaging sender ID |
| `VITE_FIREBASE_APP_ID` | Firebase app ID |
| `VITE_FIREBASE_MEASUREMENT_ID` | Firebase measurement ID |

## Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start dev server (port 3000) |
| `npm run build` | Typecheck + production build to `dist/` |
| `npm run preview` | Preview production build |
| `npm run typecheck` | Run TypeScript type checking |
| `npm test` | Run tests in watch mode |
| `npm run test:ui` | Run tests with visual UI |
| `npm run test:coverage` | Run tests with coverage report |
| `npm run lint` | Check code quality |
| `npm run lint:fix` | Auto-fix lint issues |
| `npm run format` | Format code |
| `npm run format:check` | Check formatting |

## Project Structure

```
src/
├── main.ts                      # Entry point & orchestrator
├── vite-env.d.ts                # Vite environment type declarations
├── types/
│   └── index.ts                 # Shared TypeScript interfaces
├── config/
│   └── firebase.ts              # Firebase initialization
├── constants/
│   └── app-constants.ts         # Application constants
├── core/
│   ├── item-logic.ts            # Pure business logic (no side effects)
│   └── utils.ts                 # Utility functions
├── services/
│   ├── connection-monitor.ts    # Online/offline detection
│   └── item-service.ts          # Firestore CRUD + real-time
├── state/
│   └── app-state.ts             # Singleton state object
├── ui/
│   ├── event-handlers.ts        # DOM event wiring
│   ├── renderer.ts              # DOM rendering functions
│   └── ui-manager.ts            # Screen/modal management
├── components/
│   ├── base-component.ts        # Abstract Web Component base class
│   ├── app-button.ts            # <app-button> component
│   ├── app-card.ts              # <app-card> component
│   ├── app-modal.ts             # <app-modal> component
│   ├── app-notification.ts      # <app-notification> component
│   ├── app-input.ts             # <app-input> component
│   ├── app-empty-state.ts       # <app-empty-state> component
│   └── index.ts                 # Component registration
├── i18n/
│   ├── i18n.ts                  # Translation engine (t(), initI18n)
│   ├── locale-switcher.ts       # Locale switching utilities
│   └── locales/
│       ├── en.json              # English translations
│       └── de.json              # German translations
└── styles/
    ├── main.css                 # Tailwind import + design tokens
    ├── components.css           # Component-specific styles
    ├── ui.css                   # Shared UI components
    └── animations.css           # Keyframe animations
```

## Architecture

### Folder Responsibilities

| Folder | Purpose | Example |
|--------|---------|---------|
| `config/` | External service configuration | Firebase init |
| `constants/` | Application-wide constants | Collection names, limits |
| `core/` | Pure business logic (no side effects) | Validation, sorting |
| `services/` | External service integrations | Firestore CRUD, connection monitor |
| `state/` | Application state management | Singleton state object |
| `ui/` | DOM manipulation and rendering | Screen management, event handlers |
| `components/` | Reusable Web Components | Buttons, cards, modals, inputs |
| `i18n/` | Internationalization | Translation engine, locale files |
| `styles/` | CSS files | Tailwind tokens, components, animations |
| `types/` | TypeScript type definitions | Item, AppState, ValidationResult |

### Design Patterns

- **Singleton State** — Single shared state object imported by all modules
- **Render Bridge** — `main.ts` provides a `renderApp()` function to modules
- **Module Init Pattern** — Each module exports an `init*()` function
- **Pure Logic Separation** — Business logic in `core/` has no dependencies on DOM or Firebase
- **Web Components** — Custom Elements with Shadow DOM for encapsulated, reusable UI

### Web Components

The template includes a component library built on native Custom Elements:

| Component | Tag | Description |
|-----------|-----|-------------|
| `AppButton` | `<app-button>` | Button with variant (primary/success/danger/ghost) and size (sm/md/lg) |
| `AppCard` | `<app-card>` | Card with header/body/footer slots and elevation levels |
| `AppModal` | `<app-modal>` | Modal dialog with show()/hide() API |
| `AppNotification` | `<app-notification>` | Toast notifications via static `AppNotification.show()` |
| `AppInput` | `<app-input>` | Form input with label, validation, and change events |
| `AppEmptyState` | `<app-empty-state>` | Empty state placeholder with icon and message |

### i18n

The template includes a lightweight translation engine:

- `t('key.path')` — Translate a key with dot notation
- `t('key', { param: value })` — Interpolate parameters
- `translateDOM()` — Translate all elements with `data-i18n` attributes
- Locale files in `src/i18n/locales/` (EN + DE included)
- Browser language auto-detection with localStorage persistence

## Claude Code Agents

This template includes four specialized Claude Code agents as slash commands:

| Command | Role | Focus |
|---------|------|-------|
| `/designer` | UI/UX Designer | Tailwind v4, accessibility (WCAG 2.1 AA), responsive design, design tokens |
| `/pm` | Project Manager | Feature planning, task decomposition, dependency analysis |
| `/dev` | Developer | TypeScript implementation, architecture patterns, Firebase, testing |
| `/tester` | Tester/QA | Vitest tests, coverage analysis, edge cases, Firebase mocking |

Project context is provided via `CLAUDE.md` at the project root.

## GitHub Secrets

For CI/CD to work, add these secrets in your repository settings:

- All `VITE_FIREBASE_*` variables (same as `.env`)
- `FIREBASE_SERVICE_ACCOUNT` — Firebase service account JSON (for deployment)

## CI/CD Pipeline

```
Push/PR to main
    └─> CI: Install → Format Check → Lint → Typecheck → Test → Coverage → Build
              └─> CD (on main): Build → Deploy to Firebase Hosting
              └─> PR: Build → Deploy to Firebase Preview Channel
```

## License

MIT
