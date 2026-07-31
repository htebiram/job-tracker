import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { CalendarEvent } from '../models/calendar.models';
import { CalendarRepository } from './calendar.repository';

const EVENTS: readonly CalendarEvent[] = [
  {
    id: 'e1',
    title: 'Technical interview',
    date: '2026-08-03',
    time: '10:30 AM',
    type: 'Interview',
    company: 'Northstar Labs',
    description: 'Frontend architecture panel',
  },
  {
    id: 'e2',
    title: 'Send portfolio',
    date: '2026-08-05',
    time: '9:00 AM',
    type: 'Deadline',
    company: 'Arcwell',
    description: 'Submit updated case study',
  },
  {
    id: 'e3',
    title: 'Follow up',
    date: '2026-08-07',
    time: '2:00 PM',
    type: 'Reminder',
    company: 'Kinetic Cloud',
    description: 'Confirm final interview schedule',
  },
  {
    id: 'e4',
    title: 'Application submitted',
    date: '2026-08-10',
    time: '11:00 AM',
    type: 'Application',
    company: 'Lumen Works',
    description: 'Frontend Architect application',
  },
  {
    id: 'e5',
    title: 'Culture interview',
    date: '2026-08-14',
    time: '3:30 PM',
    type: 'Interview',
    company: 'Cedar Finance',
    description: 'Meet the product organization',
  },
  {
    id: 'e6',
    title: 'Offer response',
    date: '2026-08-18',
    time: '5:00 PM',
    type: 'Deadline',
    company: 'Cedar Finance',
    description: 'Offer decision deadline',
  },
];
@Injectable()
export class MockCalendarRepository implements CalendarRepository {
  getAll(): Observable<readonly CalendarEvent[]> {
    return of(EVENTS);
  }
}
