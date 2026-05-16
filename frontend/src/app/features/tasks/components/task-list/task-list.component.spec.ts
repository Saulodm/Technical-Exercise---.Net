import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { TaskListComponent } from './task-list.component';

describe('TaskListComponent', () => {
  let component: TaskListComponent;
  let fixture: ComponentFixture<TaskListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        provideNoopAnimations(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show loading state initially', () => {
    expect(component.loading()).toBeTrue();
  });

  it('should display empty state when no tasks', () => {
    component.tasks.set([]);
    component.loading.set(false);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const emptyTitle = compiled.querySelector('.empty-state__title');
    expect(emptyTitle?.textContent).toContain('No tasks yet');
  });

  it('should have displayed columns defined', () => {
    expect(component.displayedColumns).toContain('title');
    expect(component.displayedColumns).toContain('status');
    expect(component.displayedColumns).toContain('dueDate');
    expect(component.displayedColumns).toContain('actions');
  });

  it('should format status labels correctly', () => {
    expect(component.getStatusLabel('pending')).toBe('Pending');
    expect(component.getStatusLabel('in_progress')).toBe('In Progress');
    expect(component.getStatusLabel('completed')).toBe('Completed');
  });

  it('should return correct CSS classes for status', () => {
    expect(component.getStatusClass('pending')).toBe('status-pending');
    expect(component.getStatusClass('in_progress')).toBe('status-in-progress');
    expect(component.getStatusClass('completed')).toBe('status-completed');
    expect(component.getStatusClass('unknown')).toBe('');
  });
});
