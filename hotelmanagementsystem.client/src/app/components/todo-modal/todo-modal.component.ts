// src/app/components/todo-modal/todo-modal.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-todo-modal',
  templateUrl: './todo-modal.component.html',
  imports: [FormsModule, NgClass, NgIf],
  styleUrls: ['./todo-modal.component.css']
})
export class TodoModalComponent {
  @Input() isOpen = false;
  @Input() isEditing = false;
  @Input() todo: { note: string } = { note: '' };
  @Input() position: 'right' | 'top' = 'right'; // Modal position: 'right' or 'top'

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ note: string }>();

  closeModal() {
    this.isOpen = false;
    setTimeout(() => this.close.emit(), 300); // Wait for animation to finish
  }

  saveTodo() {
    if (this.todo.note.trim()) {
      this.save.emit({ ...this.todo });
      this.closeModal();
    }
  }
}
