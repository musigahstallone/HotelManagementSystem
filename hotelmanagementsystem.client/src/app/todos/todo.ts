export interface Todo {
  id: string;
  note: string;
  isCompleted: boolean;
  createdDate: string;  // Format: YYYY-MM-DD
  createdTime: string;  // Format: HH:mm:ss.SSSSSSS
  completedDate?: string | null; // Nullable, only set if completed
  completedTime?: string | null; // Nullable, only set if completed
}
