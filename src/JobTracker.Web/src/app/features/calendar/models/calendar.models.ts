export type CalendarEventType = 'Interview' | 'Reminder' | 'Deadline' | 'Application';
export type CalendarView = 'Month' | 'Agenda';

export interface CalendarEvent {
  readonly id: string;
  readonly title: string;
  readonly date: string;
  readonly time: string;
  readonly type: CalendarEventType;
  readonly company: string;
  readonly description: string;
}

export interface CalendarDay {
  readonly date: string;
  readonly day: number;
  readonly currentMonth: boolean;
  readonly events: readonly CalendarEvent[];
}
