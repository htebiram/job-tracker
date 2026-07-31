# JobTracker Demo

JobTracker is an Angular demo for exploring a job-search workspace. It uses curated local mock data
and does not connect to an API, identity provider, or real user account.

## Demo behavior

- `/` renders the public landing page.
- `/workspace` opens the demo dashboard.
- Applications, tasks, calendar, and settings are lazy-loaded workspace features.
- Application, task, and settings changes persist while the SPA session remains open.
- Reloading or choosing **Reset demo** restores the curated starting data.
- Login and profile access are intentionally unavailable until authentication is implemented.

The workspace routes are not protected. Do not treat this demo as an authenticated application or
store sensitive information in it.

## Architecture

The application uses standalone Angular components and feature-first organization:

```text
src/app/
├── core/       # Application-wide constants and singleton services
├── features/   # Lazy-loaded business features with pages, components, models, and repositories
├── layouts/    # Public and demo workspace shells
└── shared/ui/  # Business-agnostic presentation primitives
```

Features depend on Core and Shared UI. Core and Shared UI do not depend on feature implementations.
Repository abstractions isolate pages from the local mock stores so API-backed implementations can
be introduced later.

## Dependency documentation

The [`docs`](docs) directory contains the reviewed package inventory and feature-level dependency
guides:

- [Dependency inventory](docs/Dependency-Inventory.md)
- [Landing](docs/Landing.md)
- [Dashboard](docs/Dashboard.md)
- [Applications](docs/Applications.md)
- [Tasks](docs/Tasks.md)
- [Calendar](docs/Calendar.md)
- [Settings](docs/Settings.md)
- [Shared UI and layouts](docs/Shared.md)

## Requirements

- Node.js compatible with Angular 22
- npm 11

Install dependencies:

```bash
npm ci
```

## Development

Start the development server:

```bash
npm start
```

Open `http://localhost:4200/`.

## Quality checks

```bash
npm run format:check
npm run lint
npm run test:ci
npm run build
```

Coverage reporting is not currently configured. Add a Vitest coverage provider and explicit
thresholds before using coverage as a release gate.

## Production-readiness boundary

Before adapting this demo for production, add:

- authentication and authorization guards;
- secure server-side session handling;
- API validation and authorization for every mutation;
- centralized loading, error, and retry behavior;
- deployment security headers and HTTPS enforcement;
- automated accessibility and end-to-end tests;
- measurable unit-test coverage thresholds;
- a dependency remediation policy and continuous vulnerability scanning.
