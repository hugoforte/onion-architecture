import { describe, it, expect, beforeAll } from 'vitest';
import { todoApi } from '@/api/todoApi';
import { OpenAPI } from '@/services/payments_api';

describe('todoApi integration tests', () => {
  beforeAll(() => {
    // Ensure OpenAPI is configured
    console.log('OpenAPI.BASE:', OpenAPI.BASE);
  });

  it('should have OpenAPI BASE configured', () => {
    expect(OpenAPI.BASE).toBe('http://localhost:5273');
  });

  it('should fetch todo lists from real API', async () => {
    try {
      const lists = await todoApi.getTodoLists();
      console.log('Fetched lists:', lists);
      expect(Array.isArray(lists)).toBe(true);
    } catch (error: any) {
      console.error('Error fetching lists:', error);
      console.error('Error message:', error.message);
      console.error('Error response:', error.response);
      throw error;
    }
  }, 10000);

  it('should fetch todo items for a list from real API', async () => {
    try {
      // First get or create a list
      let lists = await todoApi.getTodoLists();

      if (lists.length === 0) {
        // Create a test list
        const newList = await todoApi.createTodoList({
          name: 'Test List',
          description: 'Integration test list',
        });
        lists = [newList];
      }

      const listId = lists[0].id!;
      console.log('Fetching items for list:', listId);

      const items = await todoApi.getTodoItems(listId);
      console.log('Fetched items:', items);
      expect(Array.isArray(items)).toBe(true);
    } catch (error: any) {
      console.error('Error fetching items:', error);
      console.error('Error message:', error.message);
      console.error('Error response:', error.response);
      console.error('Error config:', error.config);
      throw error;
    }
  }, 10000);
});
