import { Observable } from 'rxjs';
import { CalendarEvent } from '../models/calendar.models';
export abstract class CalendarRepository {
  abstract getAll(): Observable<readonly CalendarEvent[]>;
}
