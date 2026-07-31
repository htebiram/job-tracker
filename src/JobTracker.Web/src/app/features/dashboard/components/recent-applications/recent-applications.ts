import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RecentApplication } from '../../models/dashboard.models';
import { Badge } from '@shared/ui/badge/badge';

@Component({
  selector: 'app-recent-applications',
  imports: [Badge],
  template: `
    <section class="panel" aria-labelledby="applications-title">
      <p>Latest additions</p>
      <h2 id="applications-title">Recent applications</h2>
      <div class="scroll">
        <table>
          <thead>
            <tr>
              <th>Company</th>
              <th>Role</th>
              <th>Status</th>
              <th>Applied</th>
            </tr>
          </thead>
          <tbody>
            @for (item of applications(); track item.id) {
              <tr>
                <td>
                  <strong>{{ item.company }}</strong>
                </td>
                <td>{{ item.role }}</td>
                <td>
                  <app-badge [tone]="item.tone">{{ item.status }}</app-badge>
                </td>
                <td>{{ item.appliedLabel }}</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </section>
  `,
  styles: `
    .panel {
      padding: var(--space-5);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      background: var(--color-surface);
      box-shadow: var(--shadow-sm);
    }
    p {
      margin: 0 0 var(--space-2);
      color: var(--color-primary);
      font-size: var(--font-size-xs);
      font-weight: 800;
      text-transform: uppercase;
    }
    h2 {
      margin: 0 0 var(--space-5);
      font-size: var(--font-size-lg);
    }
    .scroll {
      overflow: auto;
    }
    table {
      inline-size: 100%;
      border-collapse: collapse;
      font-size: var(--font-size-sm);
    }
    th,
    td {
      padding: var(--space-3);
      border-block-start: 1px solid var(--color-border);
      text-align: start;
    }
    th {
      padding-block-start: 0;
      border: 0;
      color: var(--color-text-muted);
      font-size: var(--font-size-xs);
    }
    td {
      color: var(--color-text-muted);
    }
    td strong {
      color: var(--color-text-strong);
    }
    .badge {
      display: inline-flex;
      padding: var(--space-1) var(--space-2);
      border-radius: 2rem;
      background: var(--color-primary-soft);
      color: var(--color-primary-strong);
      font-size: var(--font-size-xs);
      font-weight: 700;
    }
    .badge.info {
      background: var(--color-info-soft);
      color: var(--color-info);
    }
    .badge.success {
      background: var(--color-success-soft);
      color: var(--color-success);
    }
    .badge.danger {
      background: var(--color-danger-soft);
      color: var(--color-danger);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RecentApplications {
  readonly applications = input.required<readonly RecentApplication[]>();
}
