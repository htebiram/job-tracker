import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Badge, BadgeTone } from '@shared/ui/badge/badge';
import { EmptyState } from '@shared/ui/empty-state/empty-state';
import { Task, TaskPriority } from '../../models/task.models';

@Component({
  selector: 'app-task-list',
  imports: [Badge, EmptyState],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskList {
  readonly tasks = input.required<readonly Task[]>();
  readonly toggled = output<Task>();
  readonly edited = output<Task>();
  readonly archived = output<Task>();

  protected priorityTone(priority: TaskPriority): BadgeTone {
    return priority === 'High' ? 'danger' : priority === 'Low' ? 'success' : 'info';
  }
}
