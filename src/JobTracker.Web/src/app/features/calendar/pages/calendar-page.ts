import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { CalendarRepository } from '../data/calendar.repository';
import { MockCalendarRepository } from '../data/mock-calendar.repository';
import { CalendarDay, CalendarView } from '../models/calendar.models';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { CalendarViewComponent } from '../components/calendar-view/calendar-view';
import { ScheduleSidebar } from '../components/schedule-sidebar/schedule-sidebar';
const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'] as const;

@Component({
  selector: 'app-calendar-page',
  imports: [CalendarViewComponent, PageHeader, ScheduleSidebar],
  providers: [{ provide: CalendarRepository, useClass: MockCalendarRepository }],
  templateUrl: './calendar-page.html',
  styleUrl: './calendar-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CalendarPage {
  private readonly repository = inject(CalendarRepository);
  private readonly events = toSignal(this.repository.getAll(), { initialValue: [] });
  protected readonly weekdays = WEEKDAYS;
  protected readonly view = signal<CalendarView>('Month');
  protected readonly month = signal(new Date(2026, 7, 1));
  protected readonly selectedDate = signal('2026-08-03');
  protected readonly monthLabel = computed(() =>
    this.month().toLocaleDateString('en-US', { month: 'long', year: 'numeric' }),
  );
  protected readonly days = computed(() => this.buildDays(this.month()));
  protected readonly selectedEvents = computed(() =>
    this.events().filter((event) => event.date === this.selectedDate()),
  );
  protected readonly agenda = computed(() =>
    [...this.events()].sort((a, b) => a.date.localeCompare(b.date)),
  );
  protected readonly interviews = computed(() =>
    this.events().filter((event) => event.type === 'Interview'),
  );
  protected readonly reminders = computed(() =>
    this.events().filter((event) => event.type === 'Reminder' || event.type === 'Deadline'),
  );
  protected changeMonth(delta: number): void {
    const value = this.month();
    this.month.set(new Date(value.getFullYear(), value.getMonth() + delta, 1));
  }
  private buildDays(month: Date): readonly CalendarDay[] {
    const first = new Date(month.getFullYear(), month.getMonth(), 1);
    const start = new Date(month.getFullYear(), month.getMonth(), 1 - first.getDay());
    return Array.from({ length: 42 }, (_, index) => {
      const date = new Date(start);
      date.setDate(start.getDate() + index);
      const iso = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
      return {
        date: iso,
        day: date.getDate(),
        currentMonth: date.getMonth() === month.getMonth(),
        events: this.events().filter((event) => event.date === iso),
      };
    });
  }
}
