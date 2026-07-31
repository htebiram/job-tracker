import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { CalendarDay, CalendarEvent, CalendarView } from '../../models/calendar.models';

@Component({
  selector: 'app-calendar-view',
  templateUrl: './calendar-view.html',
  styleUrl: './calendar-view.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CalendarViewComponent {
  readonly view = input.required<CalendarView>();
  readonly monthLabel = input.required<string>();
  readonly weekdays = input.required<readonly string[]>();
  readonly days = input.required<readonly CalendarDay[]>();
  readonly selectedDate = input.required<string>();
  readonly agenda = input.required<readonly CalendarEvent[]>();
  readonly monthChanged = output<number>();
  readonly dateSelected = output<string>();

  protected eventClass(event: CalendarEvent): string {
    return event.type.toLowerCase();
  }
}
