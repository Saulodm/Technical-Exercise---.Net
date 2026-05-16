import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';

export const authRoutes: Routes = [
  { path: 'login', component: LoginComponent, title: 'Sign In' },
  { path: 'register', component: RegisterComponent, title: 'Create Account' },
];
