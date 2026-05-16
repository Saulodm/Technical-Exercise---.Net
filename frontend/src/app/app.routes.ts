import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/tasks',
    pathMatch: 'full',
  },
  {
    path: '',
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.authRoutes),
  },
  {
    path: '',
    loadChildren: () =>
      import('./features/tasks/tasks.routes').then((m) => m.taskRoutes),
  },
  {
    path: '**',
    redirectTo: '/tasks',
  },
];
