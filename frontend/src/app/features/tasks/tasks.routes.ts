import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { TaskListComponent } from './components/task-list/task-list.component';

export const taskRoutes: Routes = [
  {
    path: 'tasks',
    component: TaskListComponent,
    canActivate: [authGuard],
    title: 'My Tasks',
  },
];
