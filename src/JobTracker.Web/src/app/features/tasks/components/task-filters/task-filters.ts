import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { TaskCategory, TaskPriority, TaskView } from '../../models/task.models';

@Component({
  selector: 'app-task-filters',
  template: `
    <div class="toolbar">
      <div class="tabs" aria-label="Task views">
        <button
          type="button"
          [class.active]="view() === 'Active'"
          (click)="viewChange.emit('Active')"
        >
          Active <span>{{ activeCount() }}</span>
        </button>
        <button
          type="button"
          [class.active]="view() === 'Completed'"
          (click)="viewChange.emit('Completed')"
        >
          Completed <span>{{ completedCount() }}</span>
        </button>
      </div>
      <label>
        Priority
        <select [value]="priority()" (change)="priorityChange.emit(priorityValue($event))">
          <option>All</option>
          @for (item of priorities(); track item) {
            <option>{{ item }}</option>
          }
        </select>
      </label>
      <label>
        Category
        <select [value]="category()" (change)="categoryChange.emit(categoryValue($event))">
          <option>All</option>
          @for (item of categories(); track item) {
            <option>{{ item }}</option>
          }
        </select>
      </label>
    </div>
  `,
  styleUrl: './task-filters.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskFilters {
  readonly priorities = input.required<readonly TaskPriority[]>();
  readonly categories = input.required<readonly TaskCategory[]>();
  readonly priority = input.required<TaskPriority | 'All'>();
  readonly category = input.required<TaskCategory | 'All'>();
  readonly view = input.required<TaskView>();
  readonly activeCount = input.required<number>();
  readonly completedCount = input.required<number>();
  readonly priorityChange = output<TaskPriority | 'All'>();
  readonly categoryChange = output<TaskCategory | 'All'>();
  readonly viewChange = output<TaskView>();

  protected priorityValue(event: Event): TaskPriority | 'All' {
    return (event.target as HTMLSelectElement).value as TaskPriority | 'All';
  }

  protected categoryValue(event: Event): TaskCategory | 'All' {
    return (event.target as HTMLSelectElement).value as TaskCategory | 'All';
  }
}
