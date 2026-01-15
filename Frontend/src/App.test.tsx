import { describe, it, expect, vi } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import App from '@/App';
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

describe('App', () => {
  it('should render the application', async () => {
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<App />);

    await waitFor(() => {
      expect(screen.getByText(/todo app/i)).toBeInTheDocument();
    });
  });

  it('should display my todo lists section', async () => {
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<App />);

    await waitFor(() => {
      expect(screen.getByText('My Todo Lists')).toBeInTheDocument();
    });
  });

  it('should display create list button', async () => {
    vi.mocked(todoApi.getTodoLists).mockResolvedValueOnce([]);

    renderWithProviders(<App />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /create list/i })).toBeInTheDocument();
    });
  });
});
