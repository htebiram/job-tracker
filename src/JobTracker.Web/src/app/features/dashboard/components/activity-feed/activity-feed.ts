import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ActivityItem } from '../../models/dashboard.models';

@Component({
  selector: 'app-activity-feed',
  template: `<section class="panel" aria-labelledby="activity-title">
    <p>Updates</p>
    <h2 id="activity-title">Recent activity</h2>
    <ol>
      @for (item of activities(); track item.id) {
        <li>
          <span class="marker" aria-hidden="true">{{ item.marker }}</span
          ><span
            ><strong>{{ item.description }}</strong
            ><small>{{ item.timeLabel }}</small></span
          >
        </li>
      }
    </ol>
  </section>`,
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
    ol {
      display: grid;
      margin: 0;
      padding: 0;
      gap: var(--space-4);
      list-style: none;
    }
    li {
      display: flex;
      gap: var(--space-3);
    }
    li > span:last-child {
      display: flex;
      flex-direction: column;
    }
    strong {
      font-size: var(--font-size-sm);
    }
    small {
      color: var(--color-text-muted);
    }
    .marker {
      display: grid;
      place-items: center;
      flex: 0 0 auto;
      inline-size: 2rem;
      block-size: 2rem;
      border-radius: 50%;
      background: var(--color-surface-subtle);
      color: var(--color-primary);
      font-size: var(--font-size-xs);
      font-weight: 800;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActivityFeed {
  readonly activities = input.required<readonly ActivityItem[]>();
}
