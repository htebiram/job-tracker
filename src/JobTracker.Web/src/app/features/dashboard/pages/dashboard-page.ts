import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';

import { DashboardRepository } from '../data/dashboard.repository';
import { MockDashboardRepository } from '../data/mock-dashboard.repository';

@Component({
  selector: 'app-dashboard-page',
  providers: [{ provide: DashboardRepository, useClass: MockDashboardRepository }],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPage {
  private readonly repository = inject(DashboardRepository);

  protected readonly dashboard = toSignal(this.repository.getDashboard(), { requireSync: true });
}
