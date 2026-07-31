# Settings Feature

## Feature Overview

Demo profile, calendar, and notification preferences with session-scoped local state.

## Dependencies

- Angular Core, signal interop, and Reactive Forms.
- RxJS repository stream and `take(1)`.
- Shared page header.

## Angular Imports

Signals, `toSignal`, typed reactive forms, and validators.

## Third-party Services

None.

## Important Components

`SettingsPage`.

## Services

`SettingsRepository` and root-scoped `MockSettingsRepository`.

## Models

`WorkspaceSettings` and `WeekStart`.

## External Integrations

None. Preferences are not persisted outside the active demo session.

## Future Considerations

Profile and notification settings must be tied to an authenticated server-side identity before
production use.
