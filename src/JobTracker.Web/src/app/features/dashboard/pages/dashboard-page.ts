import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';

import { DashboardRepository } from '../data/dashboard.repository';
import { MockDashboardRepository } from '../data/mock-dashboard.repository';
import { ActivityFeed } from '../components/activity-feed/activity-feed';
import { RecentApplications } from '../components/recent-applications/recent-applications';
import { StatusChart } from '../components/status-chart/status-chart';
import { SummaryStatistics } from '../components/summary-statistics/summary-statistics';
import { UpcomingInterviews } from '../components/upcoming-interviews/upcoming-interviews';
import { PageHeader } from '@shared/ui/page-header/page-header';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    ActivityFeed,
    PageHeader,
    RecentApplications,
    RouterLink,
    StatusChart,
    SummaryStatistics,
    UpcomingInterviews,
  ],
  providers: [{ provide: DashboardRepository, useClass: MockDashboardRepository }],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPage {
  private readonly repository = inject(DashboardRepository);

  protected readonly dashboard = toSignal(this.repository.getDashboard(), { requireSync: true });
}
