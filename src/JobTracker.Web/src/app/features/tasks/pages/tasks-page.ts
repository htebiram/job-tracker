import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MockTaskRepository } from '../data/mock-task.repository';
import { TaskRepository } from '../data/task.repository';
import { Task, TaskCategory, TaskDraft, TaskPriority } from '../models/task.models';

const PRIORITIES: readonly TaskPriority[] = ['High', 'Medium', 'Low'];
const CATEGORIES: readonly TaskCategory[] = ['Application', 'Interview', 'Follow-up', 'Personal'];

@Component({
  selector: 'app-tasks-page',
  imports: [ReactiveFormsModule],
  providers: [{ provide: TaskRepository, useClass: MockTaskRepository }],
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
  protected readonly view = signal<'Active' | 'Completed'>('Active');
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
  protected readonly form = new FormGroup({
    title: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(400)],
    }),
    priority: new FormControl<TaskPriority>('Medium', { nonNullable: true }),
    category: new FormControl<TaskCategory>('Application', { nonNullable: true }),
    dueDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });
  protected updatePriority(event: Event): void {
    this.priority.set((event.target as HTMLSelectElement).value as TaskPriority | 'All');
  }
  protected updateCategory(event: Event): void {
    this.category.set((event.target as HTMLSelectElement).value as TaskCategory | 'All');
  }
  protected openCreate(): void {
    this.editing.set(null);
    this.form.reset({
      priority: 'Medium',
      category: 'Application',
      dueDate: new Date().toISOString().slice(0, 10),
    });
    this.isFormOpen.set(true);
  }
  protected openEdit(task: Task): void {
    this.editing.set(task);
    this.form.reset(task);
    this.isFormOpen.set(true);
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const draft = this.form.getRawValue() as TaskDraft;
    const editing = this.editing();
    (editing ? this.repository.update(editing.id, draft) : this.repository.create(draft)).subscribe(
      () => this.isFormOpen.set(false),
    );
  }
  protected toggle(task: Task): void {
    this.repository.setCompleted(task.id, !task.completed).subscribe();
  }
  protected archive(task: Task): void {
    this.repository.archive(task.id).subscribe();
  }
}
