export type TaskStatus = 'pending' | 'in_progress' | 'completed';

export interface TaskItem {
  id: number;
  title: string;
  description: string | null;
  status: string;
  dueDate: string | null;
  userId: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
  status: string;
  dueDate?: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string | null;
  status: string;
  dueDate?: string | null;
}
