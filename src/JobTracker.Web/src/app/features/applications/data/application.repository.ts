import { Observable } from 'rxjs';
import { ApplicationDraft, JobApplication } from '../models/application.models';

export abstract class ApplicationRepository {
  abstract getAll(): Observable<readonly JobApplication[]>;
  abstract create(draft: ApplicationDraft): Observable<JobApplication>;
  abstract update(id: string, draft: ApplicationDraft): Observable<JobApplication>;
  abstract delete(id: string): Observable<void>;
}
