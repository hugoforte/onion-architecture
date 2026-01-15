/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { TodoPriority } from './TodoPriority';
export type TodoItemDto = {
  id?: string;
  title?: string | null;
  description?: string | null;
  isCompleted?: boolean;
  dueDate?: string | null;
  priority?: TodoPriority;
  todoListId?: string;
  createdAt?: string;
  updatedAt?: string;
};
