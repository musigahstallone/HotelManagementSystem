import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Todo } from './todo';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgClass, NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-todos',
  templateUrl: './todos.component.html',
  imports: [NgForOf, NgClass, NgIf, FormsModule],
  styleUrls: ['./todos.component.css']
})
export class TodosComponent implements OnInit {
  public todos: Todo[] = [];
  public isModalOpen = false;
  public newTodoNote = '';
  public searchQuery = '';
  public isListView = false; // Default to grid view

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getTodos();
  }

  getTodos() {
    this.http.get<Todo[]>('api/todos').subscribe(
      (result: Todo[]) => {
        this.todos = result
          .map(todo => ({
            ...todo,
            createdTime: todo.createdTime ? todo.createdTime.split('.')[0] : '',
            completedTime: todo.completedTime ? todo.completedTime.split('.')[0] : ''
          }))
          .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
      },
      (error) => console.error('Error fetching todos:', error)
    );
  }


  deleteTodo(id: string): void {
    this.http.delete(`api/todos/${id}`).subscribe({
      next: (): Todo[] => this.todos = this.todos.filter(todo => todo.id !== id),
      error: (err) => console.error('Error deleting todo:', err)
    });
  }

  completeTodo(id: string): void {
    this.http.patch<Todo>(`api/todos/${id}/complete`, {}).subscribe({
      next: (updatedTodo) => {
        this.todos = this.todos.map(todo =>
          todo.id === id ? { ...todo, isCompleted: true, completedDate: updatedTodo.completedDate, completedTime: updatedTodo.completedTime ? updatedTodo.completedTime.split('.')[0] : null } : todo
        );
      },
      error: (err) => console.error('Error completing todo:', err)
    });
  }

  unCompleteTodo(id: string): void {
    this.http.patch<Todo>(`api/todos/${id}/uncomplete`, {}).subscribe({
      next: () => {
        this.todos = this.todos.map(todo =>
          todo.id === id ? { ...todo, isCompleted: false, completedDate: null, completedTime: null } : todo
        );
      },
      error: (err) => console.error('Error uncompleting todo:', err)
    });
  }

  openModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.newTodoNote = '';
  }

  addTodo(): void {
    if (!this.newTodoNote.trim()) {
      console.error('Note cannot be empty');
      return;
    }
    const newTodo: Partial<Todo> = { note: this.newTodoNote };
    this.http.post<Todo>('api/todos', newTodo).subscribe({
      next: (createdTodo) => {
        this.todos.unshift(createdTodo);
        this.closeModal();
      },
      error: (err) => console.error('Error adding todo:', err)
    });
  }

  toggleView() {
    this.isListView = !this.isListView;
  }

  get filteredTodos() {
    return this.todos.filter(todo => todo.note.toLowerCase().includes(this.searchQuery.toLowerCase()));
  }
}
