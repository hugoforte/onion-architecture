import { describe, it, expect, vi, beforeEach } from 'vitest';
import userEvent from '@testing-library/user-event';
import { screen, waitFor } from '@testing-library/react';
import TodoListsView from '@/features/todoLists/TodoListsView';
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

describe('TodoListsView', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should display loading state initially', () => {
    vi.mocked(todoApi.getTodoLists).mockImplementationOnce(
      () =>
        new Promise(() => {
          /* never resolves */
        })
    );

    renderWithProviders(<TodoListsView />);

    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should display error when fetch fails', async () => {
    vi.mocked(todoApi.getTodoLists).mockRejectedValueOnce(new Error('API Error'));

    renderWithProviders(<TodoListsView />);

    await waitFor(() => {
      expect(screen.getByText(/failed to load/i)).toBeInTheDocument();
    });
  });

  it('should display my todo lists heading', async () => {
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<TodoListsView />);

    await waitFor(() => {
      expect(screen.getByText('My Todo Lists')).toBeInTheDocument();
    });
  });

  it('should display create list button', async () => {
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<TodoListsView />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /create list/i })).toBeInTheDocument();
    });
  });

  it('should display todo lists when data loads', async () => {
    const mockLists = [
      {
        id: '1',
        name: 'Shopping',
        description: 'Grocery list',
        createdAt: '2025-01-01T00:00:00Z',
        updatedAt: '2025-01-01T00:00:00Z',
      },
      {
        id: '2',
        name: 'Work',
        description: 'Work tasks',
        createdAt: '2025-01-01T00:00:00Z',
        updatedAt: '2025-01-01T00:00:00Z',
      },
    ];

    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce(mockLists);

    renderWithProviders(<TodoListsView />);

    await waitFor(() => {
      expect(screen.getByText('Shopping')).toBeInTheDocument();
      expect(screen.getByText('Work')).toBeInTheDocument();
    });
  });

  it('should open create dialog when clicking create button', async () => {
    const user = userEvent.setup();
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<TodoListsView />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /create list/i })).toBeInTheDocument();
    });

    await user.click(screen.getByRole('button', { name: /create list/i }));

    expect(screen.getByRole('dialog')).toBeInTheDocument();
  });
});
