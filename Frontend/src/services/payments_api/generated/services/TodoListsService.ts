/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { TodoListDto } from '../models/TodoListDto';
import type { TodoListForCreationDto } from '../models/TodoListForCreationDto';
import type { TodoListForUpdateDto } from '../models/TodoListForUpdateDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class TodoListsService {
  /**
   * @returns TodoListDto OK
   * @throws ApiError
   */
  public static getApiTodoLists(): CancelablePromise<Array<TodoListDto>> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/api/todo-lists',
    });
  }
  /**
   * @param requestBody
   * @returns TodoListDto Created
   * @throws ApiError
   */
  public static postApiTodoLists(
    requestBody?: TodoListForCreationDto
  ): CancelablePromise<TodoListDto> {
    return __request(OpenAPI, {
      method: 'POST',
      url: '/api/todo-lists',
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        400: `Bad Request`,
      },
    });
  }
  /**
   * @param id
   * @returns TodoListDto OK
   * @throws ApiError
   */
  public static getApiTodoLists1(id: string): CancelablePromise<TodoListDto> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/api/todo-lists/{id}',
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
  public static putApiTodoLists(
    id: string,
    requestBody?: TodoListForUpdateDto
  ): CancelablePromise<void> {
    return __request(OpenAPI, {
      method: 'PUT',
      url: '/api/todo-lists/{id}',
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
  public static deleteApiTodoLists(id: string): CancelablePromise<void> {
    return __request(OpenAPI, {
      method: 'DELETE',
      url: '/api/todo-lists/{id}',
      path: {
        id: id,
      },
      errors: {
        404: `Not Found`,
      },
    });
  }
}
