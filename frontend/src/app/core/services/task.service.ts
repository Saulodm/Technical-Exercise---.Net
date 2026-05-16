import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TaskItem, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly apiUrl = `${environment.apiUrl}/api/Tasks`;

  private readonly loadingSignal = signal(false);

  readonly loading = this.loadingSignal.asReadonly();

  constructor(private http: HttpClient) {}

  getAll(): Observable<TaskItem[]> {
    this.loadingSignal.set(true);
    return this.http.get<TaskItem[]>(this.apiUrl);
  }

  getById(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.apiUrl, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  setLoading(value: boolean): void {
    this.loadingSignal.set(value);
  }
}
