import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { take } from 'rxjs';
import { MockTaskRepository } from '../data/mock-task.repository';
import { TaskRepository } from '../data/task.repository';
import { Task, TaskCategory, TaskDraft, TaskPriority, TaskView } from '../models/task.models';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { TaskFilters } from '../components/task-filters/task-filters';
import { TaskFormDialog } from '../components/task-form-dialog/task-form-dialog';
import { TaskList } from '../components/task-list/task-list';

const PRIORITIES: readonly TaskPriority[] = ['High', 'Medium', 'Low'];
const CATEGORIES: readonly TaskCategory[] = ['Application', 'Interview', 'Follow-up', 'Personal'];

@Component({
  selector: 'app-tasks-page',
  imports: [PageHeader, TaskFilters, TaskFormDialog, TaskList],
  providers: [{ provide: TaskRepository, useExisting: MockTaskRepository }],
  templateUrl: './tasks-page.html',
  styleUrl: './tasks-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TasksPage {
  private readonly repository = inject(TaskRepository);
  private readonly tasks = toSignal(this.repository.getAll(), { initialValue: [] });
  protected readonly priorities = PRIORITIES;
  protected readonly categories = CATEGORIES;
  protected readonly priority = signal<TaskPriority | 'All'>('All');
  protected readonly category = signal<TaskCategory | 'All'>('All');
  protected readonly view = signal<TaskView>('Active');
  protected readonly isFormOpen = signal(false);
  protected readonly editing = signal<Task | null>(null);
  protected readonly visible = computed(() =>
    this.tasks().filter(
      (task) =>
        !task.archived &&
        (this.view() === 'Completed' ? task.completed : !task.completed) &&
        (this.priority() === 'All' || task.priority === this.priority()) &&
        (this.category() === 'All' || task.category === this.category()),
    ),
  );
  protected readonly activeCount = computed(
    () => this.tasks().filter((task) => !task.archived && !task.completed).length,
  );
  protected readonly completedCount = computed(
    () => this.tasks().filter((task) => !task.archived && task.completed).length,
  );
  protected openCreate(): void {
    this.editing.set(null);
    this.isFormOpen.set(true);
  }
  protected openEdit(task: Task): void {
    this.editing.set(task);
    this.isFormOpen.set(true);
  }
  protected save(draft: TaskDraft): void {
    const editing = this.editing();
    (editing ? this.repository.update(editing.id, draft) : this.repository.create(draft))
      .pipe(take(1))
      .subscribe(() => this.isFormOpen.set(false));
  }
  protected toggle(task: Task): void {
    this.repository.setCompleted(task.id, !task.completed).pipe(take(1)).subscribe();
  }
  protected archive(task: Task): void {
    this.repository.archive(task.id).pipe(take(1)).subscribe();
  }
}
