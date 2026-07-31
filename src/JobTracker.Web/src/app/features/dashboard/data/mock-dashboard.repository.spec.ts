import { firstValueFrom } from 'rxjs';

import { MockDashboardRepository } from './mock-dashboard.repository';

describe('MockDashboardRepository', () => {
  it('returns a complete local dashboard snapshot', async () => {
    const repository = new MockDashboardRepository();

    const dashboard = await firstValueFrom(repository.getDashboard());

    expect(dashboard.metrics).toHaveLength(4);
    expect(dashboard.statuses.reduce((total, status) => total + status.count, 0)).toBe(48);
    expect(dashboard.upcomingInterviews.length).toBeGreaterThan(0);
    expect(dashboard.recentApplications.length).toBeGreaterThan(0);
    expect(dashboard.recentActivity.length).toBeGreaterThan(0);
  });
});
