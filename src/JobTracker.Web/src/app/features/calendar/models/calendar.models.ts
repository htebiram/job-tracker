export type CalendarEventType = 'Interview' | 'Reminder' | 'Deadline' | 'Application';
export interface CalendarEvent {
  readonly id: string;
  readonly title: string;
  readonly date: string;
  readonly time: string;
  readonly type: CalendarEventType;
  readonly company: string;
  readonly description: string;
}
