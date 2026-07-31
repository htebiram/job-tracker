# Dashboard Feature

## Feature Overview

Composes summary statistics, status visualization, interviews, applications, and activity widgets.

## Dependencies

- Angular Core and signal interop.
- Angular Router for the create-application deep link.
- RxJS for the repository contract and mock response.
- Shared UI badge and page-header primitives.

## Angular Imports

- `Component`, `ChangeDetectionStrategy`, `inject`
- `toSignal`
- `RouterLink`

## Third-party Services

None.

## Important Components

`DashboardPage`, `SummaryStatistics`, `StatusChart`, `UpcomingInterviews`,
`RecentApplications`, and `ActivityFeed`.

## Services

`DashboardRepository` and `MockDashboardRepository`.

## Models

Dashboard data, metrics, status summaries, interviews, recent applications, and activity items.

## External Integrations

None; all values are curated mock data.

## Future Considerations

An API implementation can replace the mock repository. If widgets become independently refreshed,
add feature-local loading/error state rather than a global state library by default.
