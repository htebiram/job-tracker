import { Observable } from 'rxjs';
import { Task, TaskDraft } from '../models/task.models';

export abstract class TaskRepository {
  abstract getAll(): Observable<readonly Task[]>;
  abstract create(draft: TaskDraft): Observable<Task>;
  abstract update(id: string, draft: TaskDraft): Observable<Task>;
  abstract setCompleted(id: string, completed: boolean): Observable<Task>;
  abstract archive(id: string): Observable<Task>;
}
