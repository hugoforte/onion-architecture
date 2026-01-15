import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { todoApi, TodoList, TodoItem, CreateTodoListDto, CreateTodoItemDto } from '@/api/todoApi';
import client from '@/api/client';

vi.mock('@/api/client');

describe('todoApi', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('getTodoLists', () => {
    it('should fetch all todo lists', async () => {
      const mockLists: TodoList[] = [
        {
          id: '1',
          name: 'Shopping',
          description: 'Grocery list',
        },
        {
          id: '2',
          name: 'Work',
          description: 'Work tasks',
        },
      ];

      vi.mocked(client.get).mockResolvedValue({ data: mockLists });

      const result = await todoApi.getTodoLists();

      expect(client.get).toHaveBeenCalledWith('/todo-lists');
      expect(result).toEqual(mockLists);
    });

    it('should handle empty list', async () => {
      vi.mocked(client.get).mockResolvedValue({ data: [] });

      const result = await todoApi.getTodoLists();

      expect(result).toEqual([]);
    });
  });

  describe('getTodoListById', () => {
    it('should fetch a single todo list by id', async () => {
      const mockList: TodoList = {
        id: '1',
        name: 'Shopping',
        description: 'Grocery list',
      };

      vi.mocked(client.get).mockResolvedValue({ data: mockList });

      const result = await todoApi.getTodoListById('1');

      expect(client.get).toHaveBeenCalledWith('/todo-lists/1');
      expect(result).toEqual(mockList);
    });
  });

  describe('createTodoList', () => {
    it('should create a new todo list', async () => {
      const dto: CreateTodoListDto = {
        name: 'New List',
        description: 'A new todo list',
      };

      const mockCreatedList: TodoList = {
        id: '3',
        ...dto,
      };

      vi.mocked(client.post).mockResolvedValue({ data: mockCreatedList });

      const result = await todoApi.createTodoList(dto);

      expect(client.post).toHaveBeenCalledWith('/todo-lists', dto);
      expect(result).toEqual(mockCreatedList);
    });
  });

  describe('deleteTodoList', () => {
    it('should delete a todo list', async () => {
      vi.mocked(client.delete).mockResolvedValue({});

      await todoApi.deleteTodoList('1');

      expect(client.delete).toHaveBeenCalledWith('/todo-lists/1');
    });
  });

  describe('getTodoItems', () => {
    it('should fetch items for a todo list', async () => {
      const mockItems: TodoItem[] = [
        {
          id: '1',
          title: 'Buy milk',
          description: '2% milk',
          isCompleted: false,
          createdAt: '2025-01-01T00:00:00Z',
          updatedAt: '2025-01-01T00:00:00Z',
        },
        {
          id: '2',
          title: 'Buy eggs',
          description: 'Brown eggs',
          isCompleted: true,
          createdAt: '2025-01-01T00:00:00Z',
          updatedAt: '2025-01-01T00:00:00Z',
        },
      ];

      vi.mocked(client.get).mockResolvedValue({ data: mockItems });

      const result = await todoApi.getTodoItems('1');

      expect(client.get).toHaveBeenCalledWith('/todo-lists/1/items');
      expect(result).toEqual(mockItems);
    });
  });

  describe('createTodoItem', () => {
    it('should create a new todo item', async () => {
      const dto: CreateTodoItemDto = {
        title: 'Buy bread',
        description: 'Whole wheat',
        todoListId: '1',
      };

      const mockCreatedItem: TodoItem = {
        id: '3',
        title: 'Buy bread',
        description: 'Whole wheat',
        isCompleted: false,
        createdAt: '2025-01-14T00:00:00Z',
        updatedAt: '2025-01-14T00:00:00Z',
      };

      vi.mocked(client.post).mockResolvedValue({ data: mockCreatedItem });

      const result = await todoApi.createTodoItem(dto);

      expect(client.post).toHaveBeenCalledWith('/todo-items', dto);
      expect(result).toEqual(mockCreatedItem);
    });
  });
});
