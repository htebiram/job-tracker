import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Task, TaskDraft } from '../models/task.models';
import { TaskRepository } from './task.repository';

const TASKS: readonly Task[] = [
  {
    id: 't1',
    title: 'Prepare Northstar interview',
    description: 'Review system design notes.',
    priority: 'High',
    category: 'Interview',
    dueDate: '2026-08-01',
    completed: false,
    archived: false,
  },
  {
    id: 't2',
    title: 'Follow up with Kinetic Cloud',
    description: 'Send availability for the panel.',
    priority: 'High',
    category: 'Follow-up',
    dueDate: '2026-08-02',
    completed: false,
    archived: false,
  },
  {
    id: 't3',
    title: 'Tailor Arcwell cover letter',
    description: 'Highlight Angular platform work.',
    priority: 'Medium',
    category: 'Application',
    dueDate: '2026-08-03',
    completed: false,
    archived: false,
  },
  {
    id: 't4',
    title: 'Update portfolio case study',
    description: 'Add accessibility metrics.',
    priority: 'Low',
    category: 'Personal',
    dueDate: '2026-08-06',
    completed: true,
    archived: false,
  },
];

@Injectable({ providedIn: 'root' })
export class MockTaskRepository implements TaskRepository {
  private readonly tasks = new BehaviorSubject<readonly Task[]>(TASKS);
  getAll(): Observable<readonly Task[]> {
    return this.tasks.asObservable();
  }
  create(draft: TaskDraft): Observable<Task> {
    const task = { ...draft, id: crypto.randomUUID(), completed: false, archived: false };
    this.tasks.next([task, ...this.tasks.value]);
    return of(task);
  }
  update(id: string, draft: TaskDraft): Observable<Task> {
    const current = this.tasks.value.find((task) => task.id === id);
    if (!current) throw new Error('Task not found');
    const task = { ...current, ...draft };
    this.tasks.next(this.tasks.value.map((item) => (item.id === id ? task : item)));
    return of(task);
  }
  setCompleted(id: string, completed: boolean): Observable<Task> {
    const task = this.requireTask(id);
    const updated = { ...task, completed };
    this.tasks.next(this.tasks.value.map((item) => (item.id === id ? updated : item)));
    return of(updated);
  }
  archive(id: string): Observable<Task> {
    const task = this.requireTask(id);
    const updated = { ...task, archived: true };
    this.tasks.next(this.tasks.value.map((item) => (item.id === id ? updated : item)));
    return of(updated);
  }
  private requireTask(id: string): Task {
    const task = this.tasks.value.find((item) => item.id === id);
    if (!task) throw new Error('Task not found');
    return task;
  }
}
