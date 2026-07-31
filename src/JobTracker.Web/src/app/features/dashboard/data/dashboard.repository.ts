import { Observable } from 'rxjs';

import { DashboardData } from '../models/dashboard.models';

export abstract class DashboardRepository {
  abstract getDashboard(): Observable<DashboardData>;
}
