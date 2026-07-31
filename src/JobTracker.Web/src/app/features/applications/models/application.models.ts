export type ApplicationStatus = 'Applied' | 'Interview' | 'Offer' | 'Rejected';
export type ApplicationSort = 'newest' | 'oldest' | 'company';

export interface ApplicationTimelineItem {
  readonly label: string;
  readonly date: string;
}

export interface JobApplication {
  readonly id: string;
  readonly company: string;
  readonly role: string;
  readonly location: string;
  readonly status: ApplicationStatus;
  readonly appliedDate: string;
  readonly source: string;
  readonly notes: string;
  readonly timeline: readonly ApplicationTimelineItem[];
}

export type ApplicationDraft = Omit<JobApplication, 'id' | 'timeline'>;
