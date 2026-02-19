import { describe, it, expect, beforeAll } from 'vitest';
import { registerComponents } from '../../src/components';

describe('Web Components', () => {
  beforeAll(() => {
    registerComponents();
  });

  describe('registerComponents', () => {
    it('should register app-button', () => {
      expect(customElements.get('app-button')).toBeDefined();
    });

    it('should register app-card', () => {
      expect(customElements.get('app-card')).toBeDefined();
    });

    it('should register app-modal', () => {
      expect(customElements.get('app-modal')).toBeDefined();
    });

    it('should register app-notification', () => {
      expect(customElements.get('app-notification')).toBeDefined();
    });

    it('should register app-input', () => {
      expect(customElements.get('app-input')).toBeDefined();
    });

    it('should register app-empty-state', () => {
      expect(customElements.get('app-empty-state')).toBeDefined();
    });
  });

  describe('AppButton', () => {
    it('should create an element', () => {
      const el = document.createElement('app-button');
      expect(el).toBeDefined();
      expect(el.shadowRoot).toBeDefined();
    });
  });

  describe('AppCard', () => {
    it('should create an element', () => {
      const el = document.createElement('app-card');
      expect(el).toBeDefined();
      expect(el.shadowRoot).toBeDefined();
    });
  });

  describe('AppEmptyState', () => {
    it('should create an element with attributes', () => {
      const el = document.createElement('app-empty-state');
      el.setAttribute('icon', '🔍');
      el.setAttribute('message', 'Nothing found');
      document.body.appendChild(el);
      expect(el.shadowRoot).toBeDefined();
      document.body.removeChild(el);
    });
  });
});
