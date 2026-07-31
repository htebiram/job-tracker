import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CalendarEvent } from '../../models/calendar.models';

@Component({
  selector: 'app-schedule-sidebar',
  templateUrl: './schedule-sidebar.html',
  styleUrl: './schedule-sidebar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ScheduleSidebar {
  readonly selectedDate = input.required<string>();
  readonly selectedEvents = input.required<readonly CalendarEvent[]>();
  readonly interviews = input.required<readonly CalendarEvent[]>();
  readonly reminders = input.required<readonly CalendarEvent[]>();

  protected eventClass(event: CalendarEvent): string {
    return event.type.toLowerCase();
  }
}
