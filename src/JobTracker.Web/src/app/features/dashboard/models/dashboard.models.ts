export type DashboardMetricTone = 'primary' | 'info' | 'success' | 'danger';
export type ApplicationStatus = 'Applied' | 'Interview' | 'Offer' | 'Rejected';

export interface DashboardMetric {
  readonly label: string;
  readonly marker: string;
  readonly value: number;
  readonly change: string;
  readonly tone: DashboardMetricTone;
}

export interface StatusSummary {
  readonly label: ApplicationStatus;
  readonly count: number;
  readonly percentage: number;
  readonly tone: DashboardMetricTone;
}

export interface UpcomingInterview {
  readonly id: string;
  readonly company: string;
  readonly role: string;
  readonly dateLabel: string;
  readonly timeLabel: string;
  readonly format: string;
}

export interface RecentApplication {
  readonly id: string;
  readonly company: string;
  readonly role: string;
  readonly status: ApplicationStatus;
  readonly tone: DashboardMetricTone;
  readonly appliedLabel: string;
}

export interface ActivityItem {
  readonly id: string;
  readonly description: string;
  readonly timeLabel: string;
  readonly marker: string;
}

export interface DashboardData {
  readonly userName: string;
  readonly metrics: readonly DashboardMetric[];
  readonly statuses: readonly StatusSummary[];
  readonly upcomingInterviews: readonly UpcomingInterview[];
  readonly recentApplications: readonly RecentApplication[];
  readonly recentActivity: readonly ActivityItem[];
}
