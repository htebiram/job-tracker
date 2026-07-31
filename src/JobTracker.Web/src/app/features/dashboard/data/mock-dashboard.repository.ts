import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { DashboardData } from '../models/dashboard.models';
import { DashboardRepository } from './dashboard.repository';

const MOCK_DASHBOARD: DashboardData = {
  userName: 'Jordan',
  metrics: [
    {
      label: 'Total Applications',
      marker: 'T',
      value: 48,
      change: '+6 this month',
      tone: 'primary',
    },
    { label: 'Interviews', marker: 'I', value: 12, change: '3 upcoming', tone: 'info' },
    { label: 'Offers', marker: 'O', value: 3, change: '+1 this month', tone: 'success' },
    { label: 'Rejections', marker: 'R', value: 18, change: '38% of total', tone: 'danger' },
  ],
  statuses: [
    { label: 'Applied', count: 15, percentage: 31, tone: 'primary' },
    { label: 'Interview', count: 12, percentage: 25, tone: 'info' },
    { label: 'Offer', count: 3, percentage: 6, tone: 'success' },
    { label: 'Rejected', count: 18, percentage: 38, tone: 'danger' },
  ],
  upcomingInterviews: [
    {
      id: 'interview-1',
      company: 'Northstar Labs',
      role: 'Senior Frontend Engineer',
      dateLabel: 'Tomorrow',
      timeLabel: '10:30 AM',
      format: 'Video call',
    },
    {
      id: 'interview-2',
      company: 'Horizon Systems',
      role: 'UI Platform Engineer',
      dateLabel: 'Aug 4',
      timeLabel: '2:00 PM',
      format: 'Technical panel',
    },
    {
      id: 'interview-3',
      company: 'Lumen Works',
      role: 'Frontend Architect',
      dateLabel: 'Aug 7',
      timeLabel: '9:00 AM',
      format: 'On-site',
    },
  ],
  recentApplications: [
    {
      id: 'application-1',
      company: 'Arcwell',
      role: 'Senior Angular Engineer',
      status: 'Applied',
      tone: 'primary',
      appliedLabel: 'Today',
    },
    {
      id: 'application-2',
      company: 'Kinetic Cloud',
      role: 'Frontend Platform Lead',
      status: 'Interview',
      tone: 'info',
      appliedLabel: 'Yesterday',
    },
    {
      id: 'application-3',
      company: 'Cedar Finance',
      role: 'Staff UI Engineer',
      status: 'Offer',
      tone: 'success',
      appliedLabel: 'Jul 27',
    },
    {
      id: 'application-4',
      company: 'Orbit Health',
      role: 'Product Engineer',
      status: 'Rejected',
      tone: 'danger',
      appliedLabel: 'Jul 25',
    },
  ],
  recentActivity: [
    {
      id: 'activity-1',
      description: 'Moved Kinetic Cloud to Interview',
      timeLabel: '35 minutes ago',
      marker: 'K',
    },
    {
      id: 'activity-2',
      description: 'Added an application for Arcwell',
      timeLabel: '2 hours ago',
      marker: 'A',
    },
    {
      id: 'activity-3',
      description: 'Received an offer from Cedar Finance',
      timeLabel: 'Yesterday',
      marker: 'C',
    },
  ],
};

@Injectable()
export class MockDashboardRepository implements DashboardRepository {
  getDashboard(): Observable<DashboardData> {
    return of(MOCK_DASHBOARD);
  }
}
