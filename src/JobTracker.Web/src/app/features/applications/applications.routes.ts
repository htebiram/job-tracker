import { Routes } from '@angular/router';

export const APPLICATIONS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/applications-page').then((component) => component.ApplicationsPage),
    title: 'Applications | JobTracker',
  },
];
