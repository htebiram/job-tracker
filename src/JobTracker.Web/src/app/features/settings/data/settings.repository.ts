import { Observable } from 'rxjs';
import { WorkspaceSettings } from '../models/settings.models';

export abstract class SettingsRepository {
  abstract get(): Observable<WorkspaceSettings>;
  abstract update(settings: WorkspaceSettings): Observable<WorkspaceSettings>;
}
