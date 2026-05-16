import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { provideNativeDateAdapter } from '@angular/material/core';
import { TaskService } from '../../../../core/services/task.service';
import { TaskItem, TaskStatus } from '../../../../models/task.models';

@Component({
  selector: 'app-task-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatProgressBarModule,
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './task-form-dialog.component.html',
  styleUrl: './task-form-dialog.component.scss',
})
export class TaskFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly dialogRef = inject(MatDialogRef<TaskFormDialogComponent>);
  readonly task: TaskItem | null = inject(MAT_DIALOG_DATA);

  readonly loading = signal(false);
  readonly isEditing = signal(this.task !== null);

  readonly statuses: { value: TaskStatus; label: string }[] = [
    { value: 'pending', label: 'Pending' },
    { value: 'in_progress', label: 'In Progress' },
    { value: 'completed', label: 'Completed' },
  ];

  readonly form = this.fb.nonNullable.group({
    title: [
      this.task?.title || '',
      [Validators.required, Validators.maxLength(300)],
    ],
    description: [this.task?.description || ''],
    status: [this.task?.status || 'pending', Validators.required],
    dueDate: [this.task?.dueDate ? new Date(this.task.dueDate) : null as Date | null],
  });

  get title() {
    return this.form.controls.title;
  }
  get status() {
    return this.form.controls.status;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);
    const raw = this.form.getRawValue();

    const request = {
      title: raw.title,
      description: raw.description || null,
      status: raw.status,
      dueDate: raw.dueDate ? raw.dueDate.toISOString().split('T')[0] : null,
    };

    const action = this.isEditing()
      ? this.taskService.update(this.task!.id, request)
      : this.taskService.create(request);

    action.subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: (err: { message: string }) => {
        console.error('Failed to save task:', err.message);
        this.loading.set(false);
      },
      complete: () => {
        this.loading.set(false);
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
