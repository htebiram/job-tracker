import { ChangeDetectionStrategy, Component, OnInit, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogShell } from '@shared/ui/dialog-shell/dialog-shell';
import { Task, TaskCategory, TaskDraft, TaskPriority } from '../../models/task.models';

@Component({
  selector: 'app-task-form-dialog',
  imports: [DialogShell, ReactiveFormsModule],
  templateUrl: './task-form-dialog.html',
  styleUrl: './task-form-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskFormDialog implements OnInit {
  readonly task = input<Task | null>(null);
  readonly priorities = input.required<readonly TaskPriority[]>();
  readonly categories = input.required<readonly TaskCategory[]>();
  readonly saved = output<TaskDraft>();
  readonly cancelled = output<void>();
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

  ngOnInit(): void {
    this.form.reset(
      this.task() ?? {
        priority: 'Medium',
        category: 'Application',
        dueDate: new Date().toISOString().slice(0, 10),
      },
    );
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saved.emit(this.form.getRawValue() as TaskDraft);
  }
}
