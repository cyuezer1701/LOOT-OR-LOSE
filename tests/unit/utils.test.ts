import { describe, it, expect } from 'vitest';
import { generateId, formatTime, formatDate } from '../../src/core/utils';

describe('generateId', () => {
  it('should generate an ID of default length 8', () => {
    const id = generateId();
    expect(id).toHaveLength(8);
  });

  it('should generate an ID of specified length', () => {
    const id = generateId(12);
    expect(id).toHaveLength(12);
  });

  it('should only contain valid characters', () => {
    const id = generateId(100);
    const validChars = 'ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789';
    for (const char of id) {
      expect(validChars).toContain(char);
    }
  });

  it('should generate unique IDs', () => {
    const ids = new Set(Array.from({ length: 100 }, () => generateId()));
    expect(ids.size).toBe(100);
  });

  it('should handle length of 0', () => {
    const id = generateId(0);
    expect(id).toBe('');
  });
});

describe('formatTime', () => {
  it('should format a timestamp as time string', () => {
    // Use a known timestamp: Jan 1, 2024 12:30:00 UTC
    const timestamp = new Date('2024-01-01T12:30:00Z').getTime();
    const result = formatTime(timestamp);
    expect(result).toMatch(/\d{2}:\d{2}/);
  });

  it('should return a non-empty string', () => {
    const result = formatTime(Date.now());
    expect(result.length).toBeGreaterThan(0);
  });
});

describe('formatDate', () => {
  it('should format a timestamp as date string', () => {
    const timestamp = new Date('2024-06-15T00:00:00Z').getTime();
    const result = formatDate(timestamp);
    expect(result).toContain('2024');
  });

  it('should return a non-empty string', () => {
    const result = formatDate(Date.now());
    expect(result.length).toBeGreaterThan(0);
  });
});
