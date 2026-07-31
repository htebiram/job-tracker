# Applications Feature

## Feature Overview

Searchable and sortable application pipeline with details, create/edit, delete, and pagination.

## Dependencies

- Angular Core, Router, signal interop, and Reactive Forms.
- RxJS repository streams and `take(1)` mutation boundaries.
- Shared badge, dialog, empty-state, and page-header UI.

## Angular Imports

`Component`, signals, `ActivatedRoute`, `ReactiveFormsModule`, `FormGroup`, `FormControl`, and
`Validators`.

## Third-party Services

None.

## Important Components

`ApplicationsPage`, `ApplicationFilters`, `ApplicationTable`, `ApplicationDetails`,
`ApplicationFormDialog`, and `DeleteApplicationDialog`.

## Services

`ApplicationRepository` and root-scoped `MockApplicationRepository`.

## Models

`JobApplication`, `ApplicationDraft`, `ApplicationStatus`, and `ApplicationSort`.

## External Integrations

None. `crypto.randomUUID()` creates demo-only identifiers.

## Future Considerations

Add API authorization, server validation, loading/error feedback, and feature-state extraction when
remote pagination and filtering are introduced.
