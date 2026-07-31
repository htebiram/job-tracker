import { Routes } from '@angular/router';

export const SETTINGS_ROUTES: Routes = [
  {
    path: '',
    title: 'Settings | JobTracker',
    loadComponent: () =>
      import('./pages/settings-page').then((component) => component.SettingsPage),
  },
];
