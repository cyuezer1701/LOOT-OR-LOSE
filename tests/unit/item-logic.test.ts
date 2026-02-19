import { describe, it, expect } from 'vitest';
import {
  validateItem,
  sortItems,
  filterItems,
  truncateText,
} from '../../src/core/item-logic';

describe('validateItem', () => {
  it('should accept a valid item', () => {
    const result = validateItem({ title: 'Test', description: 'Description' });
    expect(result.valid).toBe(true);
    expect(result.message).toBe('');
  });

  it('should reject null input', () => {
    const result = validateItem(null);
    expect(result.valid).toBe(false);
    expect(result.message).toBe('Invalid item object');
  });

  it('should reject non-object input', () => {
    const result = validateItem('string');
    expect(result.valid).toBe(false);
  });

  it('should reject empty title', () => {
    const result = validateItem({ title: '', description: 'Description' });
    expect(result.valid).toBe(false);
    expect(result.message).toBe('Title is required');
  });

  it('should reject whitespace-only title', () => {
    const result = validateItem({ title: '   ', description: 'Description' });
    expect(result.valid).toBe(false);
    expect(result.message).toBe('Title is required');
  });

  it('should reject title exceeding max length', () => {
    const result = validateItem({ title: 'A'.repeat(101), description: '' });
    expect(result.valid).toBe(false);
    expect(result.message).toContain('under 100');
  });

  it('should accept title at max length', () => {
    const result = validateItem({ title: 'A'.repeat(100), description: '' });
    expect(result.valid).toBe(true);
  });

  it('should reject description exceeding max length', () => {
    const result = validateItem({ title: 'Test', description: 'A'.repeat(2001) });
    expect(result.valid).toBe(false);
    expect(result.message).toContain('under 2000');
  });

  it('should accept item without description', () => {
    const result = validateItem({ title: 'Test' });
    expect(result.valid).toBe(true);
  });
});

describe('sortItems', () => {
  it('should sort items by createdAt descending', () => {
    const items = [
      { title: 'Old', createdAt: 1000 },
      { title: 'New', createdAt: 3000 },
      { title: 'Mid', createdAt: 2000 },
    ];
    const sorted = sortItems(items);
    expect(sorted[0].title).toBe('New');
    expect(sorted[1].title).toBe('Mid');
    expect(sorted[2].title).toBe('Old');
  });

  it('should return empty array for null input', () => {
    expect(sortItems(null)).toEqual([]);
  });

  it('should return empty array for non-array input', () => {
    expect(sortItems('not an array')).toEqual([]);
  });

  it('should not mutate the original array', () => {
    const items = [{ title: 'A', createdAt: 2 }, { title: 'B', createdAt: 1 }];
    const sorted = sortItems(items);
    expect(items[0].title).toBe('A');
    expect(sorted[0].title).toBe('A');
  });

  it('should handle items without createdAt', () => {
    const items = [{ title: 'A' }, { title: 'B', createdAt: 1000 }];
    const sorted = sortItems(items);
    expect(sorted).toHaveLength(2);
  });
});

describe('filterItems', () => {
  const items = [
    { title: 'Shopping List', description: 'Buy milk and eggs' },
    { title: 'Meeting Notes', description: 'Discuss project timeline' },
    { title: 'Ideas', description: 'New app concept' },
  ];

  it('should filter by title match', () => {
    const result = filterItems(items, 'shopping');
    expect(result).toHaveLength(1);
    expect(result[0].title).toBe('Shopping List');
  });

  it('should filter by description match', () => {
    const result = filterItems(items, 'milk');
    expect(result).toHaveLength(1);
    expect(result[0].title).toBe('Shopping List');
  });

  it('should be case insensitive', () => {
    const result = filterItems(items, 'MEETING');
    expect(result).toHaveLength(1);
  });

  it('should return all items for empty query', () => {
    expect(filterItems(items, '')).toHaveLength(3);
    expect(filterItems(items, null)).toHaveLength(3);
    expect(filterItems(items, '   ')).toHaveLength(3);
  });

  it('should return empty array for no matches', () => {
    const result = filterItems(items, 'xyz123');
    expect(result).toHaveLength(0);
  });
});

describe('truncateText', () => {
  it('should return full text if under max length', () => {
    expect(truncateText('Short text')).toBe('Short text');
  });

  it('should truncate long text with ellipsis', () => {
    const long = 'A'.repeat(150);
    const result = truncateText(long, 100);
    expect(result.length).toBeLessThanOrEqual(103); // 100 + '...'
    expect(result.endsWith('...')).toBe(true);
  });

  it('should return empty string for null text', () => {
    expect(truncateText(null)).toBe('');
  });

  it('should return empty string for undefined text', () => {
    expect(truncateText(undefined)).toBe('');
  });

  it('should respect custom max length', () => {
    const result = truncateText('Hello World', 5);
    expect(result).toBe('Hello...');
  });
});
