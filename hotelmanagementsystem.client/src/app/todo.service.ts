import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo } from './todos/todo';
import { ApiResponse, PaginatedResponse } from '../models/api-response';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private apiUrl = 'api/todos';

  constructor(private http: HttpClient) {}

  getTodos(page: number, pageSize: number,searchQuery: string): Observable<PaginatedResponse<Todo>> {
    return this.http.get<PaginatedResponse<Todo>>(`${this.apiUrl}/all-todos?page=${page}&pageSize=${pageSize}&searchQuery=${searchQuery}`);
  }

  /*
  curl -X 'GET' \
  'https://localhost:7031/api/todos/all-todos?page=0&pageSize=0&searchQuery=cloud' \
  -H 'accept: text/plain'
  */


  getPaginatedTodos(page: number, pageSize: number): Observable<PaginatedResponse<Todo>> {
    return this.http.get<PaginatedResponse<Todo>>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }

  getTodoById(id: string): Observable<ApiResponse<Todo>> {
    return this.http.get<ApiResponse<Todo>>(`${this.apiUrl}/${id}`);
  }

  deleteTodo(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }

  completeTodo(id: string): Observable<ApiResponse<Todo>> {
    return this.http.patch<ApiResponse<Todo>>(`${this.apiUrl}/${id}/complete`, {});
  }

  unCompleteTodo(id: string): Observable<ApiResponse<Todo>> {
    return this.http.patch<ApiResponse<Todo>>(`${this.apiUrl}/${id}/uncomplete`, {});
  }

  addTodo(note: string): Observable<ApiResponse<Todo>> {
    return this.http.post<ApiResponse<Todo>>(this.apiUrl, { note });
  }
/*
  updateTodo(id: string, note: string): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.apiUrl}/${id}`, note, {
      headers: { 'Content-Type': 'text/plain' }
    });
  }*/
  updateTodo(id: string, note: string): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.apiUrl}/${id}`, note, {
      headers: { 'Content-Type': 'text/plain' }
    });
  }

  bulkDelete(ids: string[]): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/bulk-delete`, { body: ids });
  }

  bulkAdd(notes: string[]): Observable<ApiResponse<Todo[]>> {
    return this.http.post<ApiResponse<Todo[]>>(`${this.apiUrl}/bulk-add`, notes);
  }
}
