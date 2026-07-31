export interface NavigationItem {
  readonly label: string;
  readonly route: string;
  readonly shortLabel: string;
}

export const PRIMARY_NAVIGATION: readonly NavigationItem[] = [
  { label: 'Dashboard', route: '/workspace', shortLabel: 'D' },
  { label: 'Applications', route: '/workspace/applications', shortLabel: 'A' },
  { label: 'Tasks', route: '/workspace/tasks', shortLabel: 'T' },
  { label: 'Calendar', route: '/workspace/calendar', shortLabel: 'C' },
  { label: 'Settings', route: '/workspace/settings', shortLabel: 'S' },
] as const;
