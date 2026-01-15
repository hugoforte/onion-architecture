/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { TodoItemDto } from '../models/TodoItemDto';
import type { TodoItemForCreationDto } from '../models/TodoItemForCreationDto';
import type { TodoItemForUpdateDto } from '../models/TodoItemForUpdateDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class TodoItemsService {
  /**
   * @param listId
   * @returns TodoItemDto OK
   * @throws ApiError
   */
  public static getApiTodoListsItems(listId: string): CancelablePromise<Array<TodoItemDto>> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/api/todo-lists/{listId}/items',
      path: {
        listId: listId,
      },
    });
  }
  /**
   * @param listId
   * @param requestBody
   * @returns TodoItemDto Created
   * @throws ApiError
   */
  public static postApiTodoListsItems(
    listId: string,
    requestBody?: TodoItemForCreationDto
  ): CancelablePromise<TodoItemDto> {
    return __request(OpenAPI, {
      method: 'POST',
      url: '/api/todo-lists/{listId}/items',
      path: {
        listId: listId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        400: `Bad Request`,
      },
    });
  }
  /**
   * @param id
   * @returns TodoItemDto OK
   * @throws ApiError
   */
  public static getApiTodoitems(id: string): CancelablePromise<TodoItemDto> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/api/todoitems/{id}',
      path: {
        id: id,
      },
      errors: {
        404: `Not Found`,
      },
    });
  }
  /**
   * @param id
   * @param requestBody
   * @returns void
   * @throws ApiError
   */
  public static putApiTodoitems(
    id: string,
    requestBody?: TodoItemForUpdateDto
  ): CancelablePromise<void> {
    return __request(OpenAPI, {
      method: 'PUT',
      url: '/api/todoitems/{id}',
      path: {
        id: id,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        404: `Not Found`,
      },
    });
  }
  /**
   * @param id
   * @returns void
   * @throws ApiError
   */
  public static deleteApiTodoitems(id: string): CancelablePromise<void> {
    return __request(OpenAPI, {
      method: 'DELETE',
      url: '/api/todoitems/{id}',
      path: {
        id: id,
      },
      errors: {
        404: `Not Found`,
      },
    });
  }
  /**
   * @param id
   * @returns void
   * @throws ApiError
   */
  public static postApiTodoitemsComplete(id: string): CancelablePromise<void> {
    return __request(OpenAPI, {
      method: 'POST',
      url: '/api/todoitems/{id}/complete',
      path: {
        id: id,
      },
      errors: {
        404: `Not Found`,
      },
    });
  }
}
