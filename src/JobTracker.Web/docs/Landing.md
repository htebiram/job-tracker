# Landing Feature

## Feature Overview

Public SaaS-style entry page that directs visitors into the local demo.

## Dependencies

- Angular Core: standalone component and `OnPush` rendering.
- Angular Router: demo navigation links.

## Angular Imports

- `ChangeDetectionStrategy`, `Component`
- `RouterLink`

## Third-party Services

None.

## Important Components

- `LandingPage`

## Services and Models

No service. Feature-highlight presentation data is private to the page.

## External Integrations

None. The dashboard preview is CSS-only.

## Future Considerations

Keep authentication links separate from demo entry. Avoid adding an analytics SDK without consent
and privacy requirements.
