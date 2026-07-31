import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { WorkspaceSettings } from '../models/settings.models';
import { SettingsRepository } from './settings.repository';

const INITIAL_SETTINGS: WorkspaceSettings = {
  displayName: 'Jordan Davis',
  email: 'jordan.davis@example.com',
  weekStart: 'Monday',
  interviewReminders: true,
  taskReminders: true,
  weeklySummary: false,
};

@Injectable({ providedIn: 'root' })
export class MockSettingsRepository implements SettingsRepository {
  private readonly settings = new BehaviorSubject<WorkspaceSettings>(INITIAL_SETTINGS);

  get(): Observable<WorkspaceSettings> {
    return this.settings.asObservable();
  }

  update(settings: WorkspaceSettings): Observable<WorkspaceSettings> {
    this.settings.next(settings);
    return of(settings);
  }
}
