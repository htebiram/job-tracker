# Dependency Inventory

Reviewed against the npm registry on 2026-07-31 using Node 24.18.0 and npm 11.16.0.

## Runtime packages

| Package                     | Installed | Latest | Purpose                                    |      Source usage | Documentation                                                | Status |
| --------------------------- | --------: | -----: | ------------------------------------------ | ----------------: | ------------------------------------------------------------ | ------ |
| `@angular/common`           |    22.1.0 | 22.1.0 | Common browser tokens and APIs             |           3 files | [Angular Common](https://angular.dev/api/common)             | Active |
| `@angular/compiler`         |    22.1.0 | 22.1.0 | Angular template compilation runtime       | Framework-managed | [Angular compiler](https://angular.dev/api/compiler)         | Active |
| `@angular/core`             |    22.1.0 | 22.1.0 | Components, DI, signals, lifecycle         |          52 files | [Angular Core](https://angular.dev/api/core)                 | Active |
| `@angular/forms`            |    22.1.0 | 22.1.0 | Reactive forms and validation              |           3 files | [Angular Forms](https://angular.dev/guide/forms)             | Active |
| `@angular/platform-browser` |    22.1.0 | 22.1.0 | Browser bootstrap and rendering            |            1 file | [Platform Browser](https://angular.dev/api/platform-browser) | Active |
| `@angular/router`           |    22.1.0 | 22.1.0 | Lazy routes, outlets, links, titles        |          24 files | [Angular Router](https://angular.dev/guide/routing)          | Active |
| `rxjs`                      |     7.8.2 |  7.8.2 | Repository streams and mutation boundaries |          20 files | [RxJS](https://rxjs.dev/)                                    | Active |
| `tslib`                     |     2.8.1 |  2.8.1 | TypeScript emitted runtime helpers         |  Compiler-emitted | [tslib](https://www.npmjs.com/package/tslib)                 | Active |

## Development packages

| Package                 | Installed | Latest | Purpose                             | Project usage                            | Documentation                                                                | Status                     |
| ----------------------- | --------: | -----: | ----------------------------------- | ---------------------------------------- | ---------------------------------------------------------------------------- | -------------------------- |
| `@angular/build`        |    22.1.0 | 22.1.2 | Build and unit-test builders        | `angular.json`                           | [Angular build system](https://angular.dev/tools/cli/build-system-migration) | Active                     |
| `@angular/cli`          |    22.1.0 | 22.1.2 | Angular workspace commands          | npm scripts                              | [Angular CLI](https://angular.dev/tools/cli)                                 | Active; advisory monitored |
| `@angular/compiler-cli` |    22.1.0 | 22.1.0 | AOT and strict template compilation | Build pipeline                           | [Compiler CLI](https://angular.dev/tools/cli/aot-compiler)                   | Active                     |
| `@eslint/js`            |    10.0.1 | 10.0.1 | ESLint recommended JavaScript rules | `eslint.config.js`                       | [ESLint](https://eslint.org/docs/latest/)                                    | Active                     |
| `angular-eslint`        |    22.1.0 | 22.1.0 | Angular TypeScript/template linting | `eslint.config.js`, Angular lint builder | [angular-eslint](https://github.com/angular-eslint/angular-eslint)           | Active                     |
| `eslint`                |    10.8.0 | 10.8.0 | Static analysis engine              | `npm run lint`                           | [ESLint](https://eslint.org/)                                                | Active                     |
| `jsdom`                 |    28.1.0 | 30.0.1 | DOM environment for unit tests      | Angular unit-test builder                | [jsdom](https://github.com/jsdom/jsdom)                                      | Active; major deferred     |
| `prettier`              |     3.9.6 |  3.9.6 | Source formatting                   | format scripts                           | [Prettier](https://prettier.io/docs/)                                        | Active                     |
| `typescript`            |     6.0.3 |  7.0.2 | Language compiler and static typing | Entire workspace                         | [TypeScript](https://www.typescriptlang.org/docs/)                           | Active; major deferred     |
| `typescript-eslint`     |    8.62.1 | 8.65.0 | Type-aware ESLint integration       | `eslint.config.js`                       | [typescript-eslint](https://typescript-eslint.io/)                           | Active                     |
| `vitest`                |    4.1.10 | 4.1.10 | Unit-test runner                    | 17 spec files                            | [Vitest](https://vitest.dev/)                                                | Active                     |

## Absent by design

The project does not install Angular Material, CDK, Bootstrap, PrimeNG, Chart.js, Font Awesome,
Zone.js, an authentication SDK, an HTTP wrapper, a storage library, or a state-management package.
Native Angular, CSS, signals, and RxJS cover the current demo requirements.

## Security notes

`npm audit` reports three moderate development-tool findings in the Angular CLI transitive chain:
`@angular/cli` → `@modelcontextprotocol/sdk` → `@hono/node-server`. There are no high or critical
findings and no vulnerable package is bundled into the browser application. The registry-proposed
fix is an Angular CLI downgrade and is intentionally not applied.
