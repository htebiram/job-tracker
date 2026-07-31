export type WeekStart = 'Sunday' | 'Monday';

export interface WorkspaceSettings {
  readonly displayName: string;
  readonly email: string;
  readonly weekStart: WeekStart;
  readonly interviewReminders: boolean;
  readonly taskReminders: boolean;
  readonly weeklySummary: boolean;
}
