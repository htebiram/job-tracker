import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { ApplicationDraft, JobApplication } from '../models/application.models';
import { ApplicationRepository } from './application.repository';

const APPLICATIONS: readonly JobApplication[] = [
  ['Arcwell', 'Senior Angular Engineer', 'Remote', 'Applied', '2026-07-30', 'LinkedIn'],
  ['Kinetic Cloud', 'Frontend Platform Lead', 'Manila', 'Interview', '2026-07-28', 'Referral'],
  ['Cedar Finance', 'Staff UI Engineer', 'Hybrid', 'Offer', '2026-07-24', 'Company site'],
  ['Orbit Health', 'Product Engineer', 'Remote', 'Rejected', '2026-07-20', 'Job board'],
  ['Northstar Labs', 'Senior Frontend Engineer', 'Makati', 'Interview', '2026-07-18', 'Referral'],
  ['Lumen Works', 'Frontend Architect', 'Remote', 'Applied', '2026-07-15', 'Company site'],
].map(([company, role, location, status, appliedDate, source], index) => ({
  id: `application-${index + 1}`,
  company,
  role,
  location,
  status: status as JobApplication['status'],
  appliedDate,
  source,
  notes: `Track the next steps for ${company}.`,
  timeline: [{ label: 'Application submitted', date: appliedDate }],
}));

@Injectable()
export class MockApplicationRepository implements ApplicationRepository {
  private readonly applications = new BehaviorSubject<readonly JobApplication[]>(APPLICATIONS);

  getAll(): Observable<readonly JobApplication[]> {
    return this.applications.asObservable();
  }

  create(draft: ApplicationDraft): Observable<JobApplication> {
    const application: JobApplication = {
      ...draft,
      id: crypto.randomUUID(),
      timeline: [{ label: 'Application submitted', date: draft.appliedDate }],
    };
    this.applications.next([application, ...this.applications.value]);
    return of(application);
  }

  update(id: string, draft: ApplicationDraft): Observable<JobApplication> {
    const current = this.applications.value.find((item) => item.id === id);
    if (!current) throw new Error('Application not found');
    const updated = { ...current, ...draft };
    this.applications.next(
      this.applications.value.map((item) => (item.id === id ? updated : item)),
    );
    return of(updated);
  }

  delete(id: string): Observable<void> {
    this.applications.next(this.applications.value.filter((item) => item.id !== id));
    return of(undefined);
  }
}
