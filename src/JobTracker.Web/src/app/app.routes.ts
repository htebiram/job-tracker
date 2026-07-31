import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout').then((component) => component.PublicLayout),
    loadChildren: () =>
      import('./features/landing/landing.routes').then((routes) => routes.LANDING_ROUTES),
  },
  {
    path: 'workspace',
    loadComponent: () =>
      import('./layouts/application-shell/application-shell').then(
        (component) => component.ApplicationShell,
      ),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((routes) => routes.DASHBOARD_ROUTES),
      },
      {
        path: 'applications',
        loadChildren: () =>
          import('./features/applications/applications.routes').then(
            (routes) => routes.APPLICATIONS_ROUTES,
          ),
      },
      {
        path: 'tasks',
        loadChildren: () =>
          import('./features/tasks/tasks.routes').then((routes) => routes.TASKS_ROUTES),
      },
      {
        path: 'calendar',
        loadChildren: () =>
          import('./features/calendar/calendar.routes').then((routes) => routes.CALENDAR_ROUTES),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
