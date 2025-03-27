// src/app/todos/todos.component.ts
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { TodoService } from '../todo.service';
import { TodoModalComponent } from '../components/todo-modal/todo-modal.component';
import {Todo} from './todo';
import {ApiResponse, PaginatedResponse} from '../../models/api-response';

@Component({
  selector: 'app-todos',
  templateUrl: './todos.component.html',
  imports: [NgForOf, NgClass, NgIf, FormsModule, TodoModalComponent],
  styleUrls: ['./todos.component.css']
})
export class TodosComponent implements OnInit {
  public todos: Todo[] = [];
  public isModalOpen = false;
  public searchQuery = '';
  public isListView = false; // Default view

  // Editing-related properties
  public isEditing = false;

  // Use selectedTodo to bind data to the modal
  public selectedTodo: Todo = { id: '', note: '', isCompleted: false, createdDate: '', createdTime: '' };

  // Pagination
  page = 1;
  pageSize = 10;
  totalItems = 0;

  constructor(private todoService: TodoService) {}

  ngOnInit() {
    this.loadTodos();
  }

  toggleView() {
    this.isListView = !this.isListView;
  }

// Add a new search method
  searchTodos(): void {
    this.page = 1;
    this.loadTodos(true); // true indicates to use the search endpoint
  }

// Modify your getter to simply return the todos fetched from the server
  get filteredTodos(): Todo[] {
    return this.todos;
  }


  loadTodos(fetchAll: boolean = false) {
    this.todoService.getTodos(this.page, this.pageSize, this.searchQuery).subscribe({
      next: (response: PaginatedResponse<Todo>) => {
        this.todos = response.data || [];
        this.totalItems = response.totalCount;
      },
      error: (err) => {
        console.error('Error loading todos', err);
        alert('Failed to load todos. Please try again.');
      }
    });
  }


  deleteTodo(id: string) {
    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        this.todos = this.todos.filter(todo => todo.id !== id);
      },
      error: (err) => console.error('Error deleting todo', err)
    });
  }

  completeTodo(id: string) {
    this.todoService.completeTodo(id).subscribe({
      next: (response: ApiResponse<Todo>) => {
        const updatedTodo = response.data;
        if (updatedTodo) {
          this.todos = this.todos.map(todo =>
            todo.id === id ? { ...todo, isCompleted: true, completedTime: updatedTodo.completedTime } : todo
          );
        }
      },
      error: (err) => console.error('Error completing todo', err)
    });
  }

  unCompleteTodo(id: string) {
    this.todoService.unCompleteTodo(id).subscribe({
      next: () => {
        this.todos = this.todos.map(todo =>
          todo.id === id ? { ...todo, isCompleted: false, completedTime: null } : todo
        );
      },
      error: (err) => console.error('Error uncompleting todo', err)
    });
  }

  // New method to toggle complete status
  toggleComplete(id: string) {
    const target = this.todos.find(todo => todo.id === id);
    if (target) {
      target.isCompleted ? this.unCompleteTodo(id) : this.completeTodo(id);
    }
  }

  // Open modal for add or edit
  openModal(todo?: Todo) {
    if (todo) {
      this.isEditing = true;
      // Clone the todo so changes in the modal don't affect the list until saved
      this.selectedTodo = { ...todo };
    } else {
      this.isEditing = false;
      this.selectedTodo = { id: '', note: '', isCompleted: false, createdDate: '', createdTime: '' };
    }
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveTodo(updated: { note: string }) {
    if (!updated.note.trim()) {
      console.error('Note cannot be empty');
      return;
    }
    if (this.isEditing && this.selectedTodo.id) {
      this.todoService.updateTodo(this.selectedTodo.id, updated.note).subscribe({
        next: () => {
          this.todos = this.todos.map(todo =>
            todo.id === this.selectedTodo.id ? { ...todo, note: updated.note } : todo
          );
          this.closeModal();
        },
        error: (error) => console.error('Error updating todo', error)
      });
    } else {
      this.todoService.addTodo(updated.note).subscribe({
        next: (response: ApiResponse<Todo>) => {
          if (response.data) {
            this.todos.unshift(response.data);
          }
          this.closeModal();
        },
        error: (err) => console.error('Error adding todo', err)
      });
    }
  }

  // Pagination methods
  nextPage() {
    if (this.page * this.pageSize < this.totalItems) {
      this.page++;
      this.loadTodos();
    }
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.loadTodos();
    }
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  goToPage() {
    if (this.page < 1) {
      this.page = 1;
    } else if (this.page > this.totalPages) {
      this.page = this.totalPages;
    }
    this.loadTodos();
  }

  trackById(index: number, todo: Todo): string {
    return todo.id;
  }

  // (Optional) Filter todos based on searchQuery
  // get filteredTodos(): Todo[] {
  //   return this.todos.filter(todo =>
  //     todo.note.toLowerCase().includes(this.searchQuery.toLowerCase())
  //   );
  // }

  // get filteredTodos(): Todo[] {
  //   if (!this.searchQuery.trim()) {
  //     return this.todos.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
  //   }
  //
  //   // Fetch all todos if searching
  //   if (this.page === 1) {
  //     this.loadTodos(true);
  //   }
  //
  //   const filtered = this.todos.filter(todo =>
  //     todo.note.toLowerCase().includes(this.searchQuery.toLowerCase())
  //   );
  //
  //   // Update total items and pages
  //   this.totalItems = filtered.length;
  //
  //   return filtered.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
  // }

}
