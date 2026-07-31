import { Routes } from '@angular/router';

export const LANDING_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/landing-page').then((component) => component.LandingPage),
    title: 'JobTracker | Organize your job search',
  },
];
