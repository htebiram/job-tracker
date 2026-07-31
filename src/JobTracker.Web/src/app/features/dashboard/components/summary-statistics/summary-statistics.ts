import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { DashboardMetric } from '../../models/dashboard.models';

@Component({
  selector: 'app-summary-statistics',
  template: `
    <div class="grid" aria-label="Application summary">
      @for (metric of metrics(); track metric.label) {
        <article>
          <div class="heading">
            <p>{{ metric.label }}</p>
            <span class="icon {{ metric.tone }}" aria-hidden="true">{{ metric.marker }}</span>
          </div>
          <strong>{{ metric.value }}</strong>
          <span class="change">{{ metric.change }}</span>
        </article>
      }
    </div>
  `,
  styles: `
    .grid {
      display: grid;
      grid-template-columns: repeat(4, minmax(0, 1fr));
      gap: var(--space-4);
    }
    article {
      padding: var(--space-5);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      background: var(--color-surface);
      box-shadow: var(--shadow-sm);
    }
    .heading {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .heading p {
      margin: 0;
      color: var(--color-text-muted);
      font-size: var(--font-size-sm);
      font-weight: 650;
    }
    .icon {
      display: grid;
      place-items: center;
      inline-size: 2rem;
      block-size: 2rem;
      border-radius: var(--radius-md);
      background: var(--color-primary-soft);
      color: var(--color-primary-strong);
      font-size: var(--font-size-xs);
      font-weight: 800;
    }
    .icon.info {
      background: var(--color-info-soft);
      color: var(--color-info);
    }
    .icon.success {
      background: var(--color-success-soft);
      color: var(--color-success);
    }
    .icon.danger {
      background: var(--color-danger-soft);
      color: var(--color-danger);
    }
    article > strong {
      display: block;
      margin-block-start: var(--space-4);
      color: var(--color-text-strong);
      font-size: 2rem;
      line-height: 1;
    }
    .change {
      display: block;
      margin-block-start: var(--space-2);
      color: var(--color-text-muted);
      font-size: var(--font-size-xs);
    }
    @media (max-width: 72rem) {
      .grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
    @media (max-width: 38rem) {
      .grid {
        grid-template-columns: 1fr;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SummaryStatistics {
  readonly metrics = input.required<readonly DashboardMetric[]>();
}
