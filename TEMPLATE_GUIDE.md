# Template Customization Guide

This guide explains how to adapt this template for your own project.

## 1. Rename the Project

Update these files with your project name:

- `package.json` — Change `"name"` field
- `public/manifest.json` — Change `"name"` and `"short_name"`
- `index.html` — Change `<title>` and heading text
- `public/icon-*.svg` — Replace with your own app icons

## 2. Configure Firebase

1. Update `.firebaserc` with your Firebase project ID
2. Create `.env` from `.env.example` and fill in your credentials
3. Update `.github/workflows/deploy.yml`:
   - Change `projectId` to your Firebase project ID
   - Change `firebaseServiceAccount` secret name if needed
4. Update `firestore.rules` with your collection security rules

## 3. Replace the Example Skeleton

The template includes a generic Items skeleton as a starting point. Replace these files with your domain logic:

### Types (`src/types/index.ts`)

Define your domain interfaces:

```typescript
export interface Product {
  id: string;
  userId: string;
  name: string;
  price: number;
  createdAt: Date;
  updatedAt: Date;
}
```

### Constants (`src/constants/app-constants.ts`)

Replace `COLLECTIONS`, `UI`, and `ITEM_STATUS` with your own constants:

```typescript
export const COLLECTIONS = {
  PRODUCTS: 'products',
  ORDERS: 'orders',
} as const;
```

### Business Logic (`src/core/item-logic.ts`)

Replace with your domain logic. Keep functions pure (no DOM, no Firebase):

```typescript
// Example: src/core/product-logic.ts
export function validateProduct(product: Product): ValidationResult { ... }
export function calculateTotal(items: CartItem[]): number { ... }
```

### State (`src/state/app-state.ts`)

Update the state object with your app's properties:

```typescript
const state: AppState = {
  userId: null,
  products: [],
  cart: [],
  unsubscribe: null,
};
```

### Services (`src/services/item-service.ts`)

Replace with your Firestore operations. Follow the same pattern:

```typescript
// Example: src/services/product-service.ts
export async function createProduct(userId: string, data: ProductData): Promise<string> { ... }
export function subscribeToProducts(userId: string, renderCallback: RenderCallback): Unsubscribe { ... }
```

### UI Files (`src/ui/`)

- `renderer.ts` — Replace `renderItemCard()` / `renderItemList()` with your rendering functions
- `event-handlers.ts` — Wire up your buttons and events
- `ui-manager.ts` — Add/remove screen IDs in `showScreen()`

### HTML (`index.html`)

Replace the start screen and app screen content with your own UI.

### Styles (`src/styles/components.css`)

Replace item-specific styles with your component styles.

## 4. Customize i18n

The template includes English and German translations:

1. Edit locale files in `src/i18n/locales/`:
   - `en.json` — English translations
   - `de.json` — German translations
2. Add new locales by creating additional JSON files (e.g., `fr.json`)
3. Update `src/i18n/locale-switcher.ts` `AVAILABLE_LOCALES` with your supported languages
4. Use `data-i18n="key.path"` attributes in HTML for static text
5. Use `t('key.path', { param: value })` in TypeScript for dynamic text
6. Use `data-i18n-placeholder="key.path"` for input placeholders

## 5. Customize Web Components

The template includes a component library in `src/components/`:

### Using existing components

```html
<app-button variant="primary" size="md">Click me</app-button>
<app-card elevation="md">
  <span slot="header">Title</span>
  <p slot="body">Content</p>
</app-card>
<app-modal title="My Modal"><p>Modal content</p></app-modal>
<app-input label="Name" placeholder="Enter name" maxlength="100"></app-input>
<app-empty-state icon="📋" message="No items yet."></app-empty-state>
```

### Creating new components

1. Create a new file in `src/components/` extending `BaseComponent`:

```typescript
import { BaseComponent } from './base-component';

export class AppMyWidget extends BaseComponent {
  static get observedAttributes(): string[] {
    return ['title'];
  }

  render(): void {
    this.html(`
      ${this.baseStyles()}
      <style>/* component styles */</style>
      <div>${this.getAttribute('title')}</div>
    `);
  }
}
```

2. Register it in `src/components/index.ts`:

```typescript
import { AppMyWidget } from './app-my-widget';
// Add to registerComponents():
customElements.define('app-my-widget', AppMyWidget);
```

3. Add tests in `tests/unit/components.test.ts`

### Programmatic notifications

```typescript
import { AppNotification } from './components/app-notification';
AppNotification.show('Item saved!', 'success');
AppNotification.show('Something went wrong', 'error', 5000);
```

## 6. Add New Firestore Collections

1. Define the collection name in `src/constants/app-constants.ts`
2. Create a service file in `src/services/`
3. Add security rules in `firestore.rules`
4. Add Firestore indexes in `firestore.indexes.json` if needed

## 7. Add New Screens

The app uses a screen-toggling pattern instead of a router:

1. Add a new `<div id="my-screen" class="hidden">` in `index.html`
2. Register the screen ID in `ui-manager.ts` `showScreen()` array
3. Call `showScreen('my-screen')` to navigate

## 8. Add New Tests

Follow the existing test structure:

- **Unit tests** in `tests/unit/` — Test pure logic functions
- **Integration tests** in `tests/integration/` — Test Firebase-dependent modules with mocks
- **Test factories** in `tests/helpers/` — Create test data
- Import `tests/mocks/firebase-mock.ts` in integration tests

```typescript
// Example unit test
import { describe, it, expect } from 'vitest';
import { validateProduct } from '../../src/core/product-logic';

describe('validateProduct', () => {
  it('should reject empty name', () => {
    const result = validateProduct({ name: '' });
    expect(result.isValid).toBe(false);
  });
});
```

## 9. Customize Tailwind Design Tokens

Edit the `@theme` block in `src/styles/main.css`:

```css
@theme {
  --color-primary: #1a237e;
  --color-primary-light: #1565c0;
  --color-accent: #10b981;
  --color-danger: #ef4444;
  --font-family-sans: 'Your Font', sans-serif;
}
```

Use tokens in your styles: `bg-primary`, `text-accent`, `font-sans`, etc.

## 10. Customize PWA

- Update `public/manifest.json` with your app info
- Replace `public/icon-*.svg` with your icons
- Update `public/sw.js` cache name when deploying updates
- Update `public/offline.html` with your branding

## 11. Use Claude Code Agents

The template includes four Claude Code slash commands:

| Command | When to use |
|---------|-------------|
| `/designer` | UI/UX work: layouts, styling, accessibility, responsive design |
| `/pm` | Planning: feature specs, task breakdown, dependency analysis |
| `/dev` | Implementation: TypeScript code, Firebase integration, architecture |
| `/tester` | Testing: write tests, coverage analysis, edge cases |

Project context is automatically provided via `CLAUDE.md`.

## 12. Deploy

The CI/CD pipeline automatically deploys to Firebase Hosting on push to `main`:

1. Add GitHub Secrets (see README)
2. Push to `main`
3. CI runs: format check, lint, typecheck, test, build
4. On CI success: CD deploys to Firebase Hosting
5. PRs get preview deployments on Firebase Preview Channels

To deploy Firestore rules manually:

```bash
firebase deploy --only firestore:rules
```
