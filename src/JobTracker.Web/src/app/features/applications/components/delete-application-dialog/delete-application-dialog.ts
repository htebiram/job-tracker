import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { JobApplication } from '../../models/application.models';
import { DialogShell } from '@shared/ui/dialog-shell/dialog-shell';

@Component({
  selector: 'app-delete-application-dialog',
  imports: [DialogShell],
  template: `<app-dialog-shell
    role="alertdialog"
    labelledBy="delete-title"
    maxWidth="compact"
    (dismissed)="cancelled.emit()"
  >
    <div class="dialog-content">
      <h2 id="delete-title">Delete application?</h2>
      <p>This permanently removes {{ application().company }} from the mock workspace.</p>
      <div class="actions">
        <button type="button" (click)="cancelled.emit()">Cancel</button
        ><button class="danger" type="button" (click)="confirmed.emit()">Delete</button>
      </div>
    </div>
  </app-dialog-shell>`,
  styles: `
    .actions {
      display: flex;
      justify-content: flex-end;
      gap: var(--space-3);
      margin-top: var(--space-5);
    }
    button {
      padding: 0.65rem 0.9rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-surface);
    }
    .danger {
      background: var(--color-danger);
      color: white;
      border-color: var(--color-danger);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeleteApplicationDialog {
  readonly application = input.required<JobApplication>();
  readonly confirmed = output<void>();
  readonly cancelled = output<void>();
}
