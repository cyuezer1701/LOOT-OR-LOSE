import { describe, it, expect, vi, beforeEach } from 'vitest';

// Mock dynamic import for locale files
vi.mock('../../src/i18n/locales/en.json', () => ({
  default: {
    app: { name: 'My App', tagline: 'Test' },
    validation: { titleTooLong: 'Title must be under {max} characters' },
    actions: { save: 'Save' },
  },
}));

vi.mock('../../src/i18n/locales/de.json', () => ({
  default: {
    app: { name: 'Meine App', tagline: 'Test DE' },
    validation: { titleTooLong: 'Titel darf maximal {max} Zeichen haben' },
    actions: { save: 'Speichern' },
  },
}));

// We need to test the t() function logic directly
describe('i18n', () => {
  let i18n: typeof import('../../src/i18n/i18n');

  beforeEach(async () => {
    vi.resetModules();
    i18n = await import('../../src/i18n/i18n');
  });

  describe('initI18n', () => {
    it('should initialize with English by default', async () => {
      await i18n.initI18n('en');
      expect(i18n.getLocale()).toBe('en');
    });

    it('should load German locale', async () => {
      await i18n.initI18n('de');
      expect(i18n.getLocale()).toBe('de');
    });
  });

  describe('t()', () => {
    it('should translate a nested key', async () => {
      await i18n.initI18n('en');
      expect(i18n.t('app.name')).toBe('My App');
    });

    it('should return key for missing translations', async () => {
      await i18n.initI18n('en');
      expect(i18n.t('nonexistent.key')).toBe('nonexistent.key');
    });

    it('should interpolate parameters', async () => {
      await i18n.initI18n('en');
      const result = i18n.t('validation.titleTooLong', { max: 100 });
      expect(result).toBe('Title must be under 100 characters');
    });

    it('should use German translations when locale is de', async () => {
      await i18n.initI18n('de');
      expect(i18n.t('app.name')).toBe('Meine App');
      expect(i18n.t('actions.save')).toBe('Speichern');
    });
  });
});
