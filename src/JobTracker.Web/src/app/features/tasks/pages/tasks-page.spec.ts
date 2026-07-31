import { TestBed } from '@angular/core/testing';
import { TasksPage } from './tasks-page';
import { MockTaskRepository } from '../data/mock-task.repository';
import { TaskRepository } from '../data/task.repository';

describe('TasksPage', () => {
  it('renders active tasks, filters, and the create dialog', async () => {
    await TestBed.configureTestingModule({ imports: [TasksPage] }).compileComponents();
    const fixture = TestBed.createComponent(TasksPage);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    expect(fixture.debugElement.injector.get(TaskRepository)).toBe(
      TestBed.inject(MockTaskRepository),
    );
    expect(element.querySelectorAll('.task-list article')).toHaveLength(3);
    expect(element.querySelectorAll('.toolbar select')).toHaveLength(2);
    (element.querySelector('.primary') as HTMLButtonElement).click();
    fixture.detectChanges();
    expect(element.querySelector('[role=dialog]')).not.toBeNull();
  });
});
