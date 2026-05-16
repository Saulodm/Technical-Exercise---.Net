import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TaskService } from './task.service';
import { environment } from '../../../environments/environment';
import { TaskItem, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.models';

describe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController;

  const mockTasks: TaskItem[] = [
    {
      id: 1,
      title: 'Task 1',
      description: 'Description 1',
      status: 'pending',
      dueDate: '2026-06-01',
      userId: 1,
      createdAt: '2026-05-01T00:00:00Z',
      updatedAt: '2026-05-01T00:00:00Z',
    },
    {
      id: 2,
      title: 'Task 2',
      description: null,
      status: 'in_progress',
      dueDate: null,
      userId: 1,
      createdAt: '2026-05-02T00:00:00Z',
      updatedAt: '2026-05-02T00:00:00Z',
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        TaskService,
      ],
    });

    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('should return all tasks', () => {
      service.getAll().subscribe((tasks) => {
        expect(tasks).toEqual(mockTasks);
        expect(tasks.length).toBe(2);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Tasks`);
      expect(req.request.method).toBe('GET');
      req.flush(mockTasks);
    });
  });

  describe('getById', () => {
    it('should return a single task', () => {
      service.getById(1).subscribe((task) => {
        expect(task).toEqual(mockTasks[0]);
        expect(task.id).toBe(1);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Tasks/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockTasks[0]);
    });
  });

  describe('create', () => {
    it('should create a task and return it', () => {
      const createRequest: CreateTaskRequest = {
        title: 'New Task',
        description: 'Description',
        status: 'pending',
        dueDate: '2026-06-15',
      };

      const createdTask: TaskItem = {
        id: 3,
        ...createRequest,
        description: 'Description',
        dueDate: '2026-06-15',
        userId: 1,
        createdAt: '2026-05-16T00:00:00Z',
        updatedAt: '2026-05-16T00:00:00Z',
      };

      service.create(createRequest).subscribe((task) => {
        expect(task).toEqual(createdTask);
        expect(task.title).toBe('New Task');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Tasks`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(createRequest);
      req.flush(createdTask);
    });
  });

  describe('update', () => {
    it('should update a task and return it', () => {
      const updateRequest: UpdateTaskRequest = {
        title: 'Updated Task',
        description: 'Updated description',
        status: 'completed',
      };

      const updatedTask: TaskItem = {
        id: 1,
        ...updateRequest,
        description: 'Updated description',
        dueDate: null,
        userId: 1,
        createdAt: '2026-05-01T00:00:00Z',
        updatedAt: '2026-05-16T00:00:00Z',
      };

      service.update(1, updateRequest).subscribe((task) => {
        expect(task).toEqual(updatedTask);
        expect(task.title).toBe('Updated Task');
        expect(task.status).toBe('completed');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Tasks/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush(updatedTask);
    });
  });

  describe('delete', () => {
    it('should delete a task', () => {
      service.delete(1).subscribe(() => {
        expect(true).toBeTrue();
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Tasks/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null, { status: 204, statusText: 'No Content' });
    });
  });
});
