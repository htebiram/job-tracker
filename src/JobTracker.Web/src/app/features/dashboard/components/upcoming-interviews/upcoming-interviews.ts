import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { UpcomingInterview } from '../../models/dashboard.models';

@Component({
  selector: 'app-upcoming-interviews',
  template: `
    <section class="panel" aria-labelledby="interviews-title">
      <p class="eyebrow">Next up</p>
      <h2 id="interviews-title">Upcoming interviews</h2>
      <ul>
        @for (item of interviews(); track item.id) {
          <li>
            <span class="date"
              ><strong>{{ item.dateLabel }}</strong
              ><span>{{ item.timeLabel }}</span></span
            ><span class="details"
              ><strong>{{ item.company }}</strong
              ><span>{{ item.role }}</span></span
            ><span class="format">{{ item.format }}</span>
          </li>
        }
      </ul>
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
    .eyebrow {
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
    ul {
      margin: 0;
      padding: 0;
      list-style: none;
    }
    li {
      display: grid;
      grid-template-columns: 4.5rem 1fr auto;
      align-items: center;
      padding-block: var(--space-3);
      gap: var(--space-3);
      border-block-start: 1px solid var(--color-border);
    }
    li:first-child {
      padding-block-start: 0;
      border: 0;
    }
    .date,
    .details {
      display: flex;
      min-inline-size: 0;
      flex-direction: column;
    }
    .date {
      padding: var(--space-2);
      border-radius: var(--radius-md);
      background: var(--color-primary-soft);
      color: var(--color-primary-strong);
      text-align: center;
    }
    .date strong,
    .date span,
    .details span,
    .format {
      font-size: var(--font-size-xs);
    }
    .details strong {
      font-size: var(--font-size-sm);
    }
    .details span,
    .format {
      color: var(--color-text-muted);
    }
    @media (max-width: 38rem) {
      li {
        grid-template-columns: 4.5rem 1fr;
      }
      .format {
        display: none;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpcomingInterviews {
  readonly interviews = input.required<readonly UpcomingInterview[]>();
}
