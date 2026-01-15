import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import TodoItemsView from '@/features/todoItems/TodoItemsView';
import { renderWithProviders } from '@/test/test-utils';
import { todoApi } from '@/api/todoApi';

vi.mock('@/api/todoApi', () => ({
  todoApi: {
    getTodoLists: vi.fn(),
    getTodoListById: vi.fn(),
    createTodoList: vi.fn(),
    deleteTodoList: vi.fn(),
    getTodoItems: vi.fn(),
    createTodoItem: vi.fn(),
    completeTodoItem: vi.fn(),
    deleteTodoItem: vi.fn(),
  },
}));

describe('TodoItemsView', () => {
  const mockListId = '1';

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should display loading state initially', () => {
    vi.mocked(todoApi.getTodoItems).mockImplementationOnce(
      () =>
        new Promise(() => {
          /* never resolves */
        })
    );

    renderWithProviders(<TodoItemsView todoListId={mockListId} />);

    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should display error when fetch fails', async () => {
    vi.mocked(todoApi.getTodoItems).mockRejectedValueOnce(new Error('API Error'));

    renderWithProviders(<TodoItemsView todoListId={mockListId} />);

    await waitFor(() => {
      expect(screen.getByText(/failed to load/i)).toBeInTheDocument();
    });
  });

  it('should display add item button', async () => {
    vi.mocked(todoApi.getTodoItems).mockResolvedValueOnce([]);

    renderWithProviders(<TodoItemsView todoListId={mockListId} />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /add item/i })).toBeInTheDocument();
    });
  });

  it('should display items when data loads', async () => {
    const mockItems = [
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

    vi.mocked(todoApi.getTodoItems).mockResolvedValueOnce(mockItems);

    renderWithProviders(<TodoItemsView todoListId={mockListId} />);

    await waitFor(() => {
      expect(screen.getByText('Buy milk')).toBeInTheDocument();
      expect(screen.getByText('Buy eggs')).toBeInTheDocument();
    });
  });

  it('should display items with correct details', async () => {
    const mockItems = [
      {
        id: '1',
        title: 'Buy milk',
        description: '2% milk',
        isCompleted: false,
        createdAt: '2025-01-01T00:00:00Z',
        updatedAt: '2025-01-01T00:00:00Z',
      },
    ];

    vi.mocked(todoApi.getTodoItems).mockResolvedValueOnce(mockItems);

    renderWithProviders(<TodoItemsView todoListId={mockListId} />);

    await waitFor(() => {
      expect(screen.getByText('Buy milk')).toBeInTheDocument();
      expect(screen.getByText('2% milk')).toBeInTheDocument();
    });
  });
});
