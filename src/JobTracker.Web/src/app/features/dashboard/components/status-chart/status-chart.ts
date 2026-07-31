import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { StatusSummary } from '../../models/dashboard.models';

@Component({
  selector: 'app-status-chart',
  template: `
    <section class="panel" aria-labelledby="status-title">
      <div class="heading">
        <div>
          <p>Pipeline</p>
          <h2 id="status-title">Application status</h2>
        </div>
        <span>{{ total() }} total</span>
      </div>
      <div class="chart" role="img" aria-label="Bar chart showing application totals by status">
        @for (status of statuses(); track status.label) {
          <div>
            <div class="label">
              <span>{{ status.label }}</span
              ><strong>{{ status.count }}</strong>
            </div>
            <div class="track">
              <span class="bar {{ status.tone }}" [style.inline-size.%]="status.percentage"></span>
            </div>
          </div>
        }
      </div>
    </section>
  `,
  styleUrl: './status-chart.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusChart {
  readonly statuses = input.required<readonly StatusSummary[]>();
  readonly total = input.required<number>();
}
