import { Component, effect, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { DatePipe } from '@angular/common';
import { TaskService } from '../../../../core/services/task.service';
import { TaskItem } from '../../../../models/task.models';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { TaskFormDialogComponent } from '../task-form-dialog/task-form-dialog.component';

@Component({
  selector: 'app-task-list',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatCardModule,
    DatePipe,
    EmptyStateComponent,
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss',
})
export class TaskListComponent {
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly tasks = signal<TaskItem[]>([]);
  readonly loading = signal(false);

  readonly displayedColumns = ['title', 'status', 'dueDate', 'actions'];

  constructor() {
    effect(() => {
      if (this.taskService.loading()) {
        this.loading.set(true);
      }
    });
  }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading.set(true);
    this.taskService.getAll().subscribe({
      next: (data) => {
        this.tasks.set(data);
        this.loading.set(false);
        this.taskService.setLoading(false);
      },
      error: () => {
        this.loading.set(false);
        this.taskService.setLoading(false);
      },
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TaskFormDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: null,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadTasks();
        this.snackBar.open('Task created successfully', 'Close', { duration: 3000 });
      }
    });
  }

  openEditDialog(task: TaskItem): void {
    const dialogRef = this.dialog.open(TaskFormDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: task,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadTasks();
        this.snackBar.open('Task updated successfully', 'Close', { duration: 3000 });
      }
    });
  }

  confirmDelete(task: TaskItem): void {
    const dialogRef = this.dialog.open<ConfirmDialogComponent, ConfirmDialogData>(
      ConfirmDialogComponent,
      {
        width: '400px',
        data: {
          title: 'Delete Task',
          message: `Are you sure you want to delete "${task.title}"? This action cannot be undone.`,
          confirmText: 'Delete',
          cancelText: 'Cancel',
        },
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deleteTask(task.id);
      }
    });
  }

  private deleteTask(id: number): void {
    this.taskService.delete(id).subscribe({
      next: () => {
        this.loadTasks();
        this.snackBar.open('Task deleted successfully', 'Close', { duration: 3000 });
      },
      error: (err: { message: string }) => {
        this.snackBar.open(err.message || 'Failed to delete task', 'Close', { duration: 5000 });
      },
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'pending':
        return 'status-pending';
      case 'in_progress':
        return 'status-in-progress';
      case 'completed':
        return 'status-completed';
      default:
        return '';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'pending':
        return 'Pending';
      case 'in_progress':
        return 'In Progress';
      case 'completed':
        return 'Completed';
      default:
        return status;
    }
  }
}
