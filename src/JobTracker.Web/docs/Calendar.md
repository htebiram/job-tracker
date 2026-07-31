# Calendar Feature

## Feature Overview

Month and agenda views for interviews, reminders, deadlines, and application events.

## Dependencies

- Angular Core and signal interop.
- RxJS repository contract.
- Shared page header.

## Angular Imports

Signals, computed values, `inject`, `toSignal`, and modern control-flow templates.

## Third-party Services

None. No calendar or date library is installed.

## Important Components

`CalendarPage`, `CalendarViewComponent`, and `ScheduleSidebar`.

## Services

`CalendarRepository` and `MockCalendarRepository`.

## Models

`CalendarEvent`, `CalendarEventType`, `CalendarDay`, and `CalendarView`.

## External Integrations

None.

## Future Considerations

Adopt Angular date formatting or a dedicated date library only when timezone, locale, recurrence, or
external calendar synchronization requirements justify it.
