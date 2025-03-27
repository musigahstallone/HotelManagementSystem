// src/app/models/todo.ts
export interface Todo {
  id: string;
  note: string;
  isCompleted: boolean;
  createdDate: string;  // Format: YYYY-MM-DD
  createdTime: string;  // Format: HH:mm:ss.SSSSSSS
  completedDate?: string | null;
  completedTime?: string | null;
}
