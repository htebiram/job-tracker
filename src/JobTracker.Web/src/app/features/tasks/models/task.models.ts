export type TaskPriority = 'High' | 'Medium' | 'Low';
export type TaskCategory = 'Application' | 'Interview' | 'Follow-up' | 'Personal';

export interface Task {
  readonly id: string;
  readonly title: string;
  readonly description: string;
  readonly priority: TaskPriority;
  readonly category: TaskCategory;
  readonly dueDate: string;
  readonly completed: boolean;
  readonly archived: boolean;
}

export type TaskDraft = Omit<Task, 'id' | 'completed' | 'archived'>;
