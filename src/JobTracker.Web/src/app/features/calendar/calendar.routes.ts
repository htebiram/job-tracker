import { Routes } from '@angular/router';

export const CALENDAR_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/calendar-page').then((component) => component.CalendarPage),
    title: 'Calendar | JobTracker',
  },
];
