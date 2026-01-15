import {
  TodoListsService,
  TodoItemsService,
  type TodoListDto,
  type TodoItemDto,
  type TodoListForCreationDto,
  type TodoItemForCreationDto,
} from '@/services/payments_api';

// Re-export types for backwards compatibility
export type TodoItem = TodoItemDto;
export type TodoList = TodoListDto;
export type CreateTodoListDto = TodoListForCreationDto;
export type CreateTodoItemDto = TodoItemForCreationDto;

export const todoApi = {
  // Todo Lists
  getTodoLists: async () => {
    return await TodoListsService.getApiTodoLists();
  },

  getTodoListById: async (id: string) => {
    return await TodoListsService.getApiTodoLists1(id);
  },

  createTodoList: async (dto: CreateTodoListDto) => {
    return await TodoListsService.postApiTodoLists(dto);
  },

  deleteTodoList: async (id: string) => {
    await TodoListsService.deleteApiTodoLists(id);
  },

  // Todo Items
  getTodoItems: async (todoListId: string) => {
    return await TodoItemsService.getApiTodoListsItems(todoListId);
  },

  createTodoItem: async (dto: CreateTodoItemDto) => {
    const { todoListId, ...itemData } = dto;
    return await TodoItemsService.postApiTodoListsItems(
      todoListId,
      itemData as TodoItemForCreationDto
    );
  },

  completeTodoItem: async (id: string) => {
    await TodoItemsService.postApiTodoitemsComplete(id);
  },

  deleteTodoItem: async (id: string) => {
    await TodoItemsService.deleteApiTodoitems(id);
  },
};
