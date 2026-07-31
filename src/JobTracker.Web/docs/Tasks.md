# Tasks Feature

## Feature Overview

Local task planning with filters, completion, archive, and create/edit workflows.

## Dependencies

- Angular Core, signal interop, and Reactive Forms.
- RxJS repository streams and bounded mutation subscriptions.
- Shared badge, dialog, empty-state, and page-header UI.

## Angular Imports

Signals, `toSignal`, `ReactiveFormsModule`, typed controls, and validators.

## Third-party Services

None.

## Important Components

`TasksPage`, `TaskFilters`, `TaskList`, and `TaskFormDialog`.

## Services

`TaskRepository` and root-scoped `MockTaskRepository`.

## Models

`Task`, `TaskDraft`, `TaskPriority`, `TaskCategory`, and `TaskView`.

## External Integrations

None.

## Future Considerations

Move filter selectors to a feature facade if server synchronization, assignment, or large task
collections are introduced.
