# Shared UI and Layouts

## Feature Overview

Business-agnostic UI primitives and the public/workspace presentation shells.

## Dependencies

- Angular Core for standalone components, signal inputs/outputs, and render-time focus.
- Angular Common `DOCUMENT` for browser-safe theme/reset access.
- Angular Router for outlets and navigation.

## Angular Imports

`Component`, `ChangeDetectionStrategy`, signal inputs/outputs, `afterNextRender`, `DOCUMENT`,
`RouterLink`, `RouterLinkActive`, and `RouterOutlet`.

## Third-party Services

None.

## Important Components

`PageHeader`, `Badge`, `EmptyState`, `DialogShell`, `PublicLayout`, `ApplicationShell`,
`TopNavigation`, `SideNavigation`, `DemoNotice`, and `AppFooter`.

## Services

`ThemeService` is the only Core singleton service.

## Models and Configuration

`BadgeTone`, `ColorTheme`, `NavigationItem`, and `PRIMARY_NAVIGATION`.

## External Integrations

None.

## Future Considerations

If accessibility requirements expand, evaluate Angular CDK focus trapping as a deliberate,
version-aligned dependency rather than implementing a growing custom overlay system.
