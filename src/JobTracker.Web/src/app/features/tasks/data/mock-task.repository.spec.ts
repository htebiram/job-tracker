import { firstValueFrom } from 'rxjs';
import { MockTaskRepository } from './mock-task.repository';

describe('MockTaskRepository', () => {
  it('creates, completes, updates, and archives tasks', async () => {
    const repository = new MockTaskRepository();
    const draft = {
      title: 'Test',
      description: '',
      priority: 'High' as const,
      category: 'Follow-up' as const,
      dueDate: '2026-08-10',
    };
    const created = await firstValueFrom(repository.create(draft));
    expect((await firstValueFrom(repository.setCompleted(created.id, true))).completed).toBe(true);
    expect(
      (await firstValueFrom(repository.update(created.id, { ...draft, title: 'Updated' }))).title,
    ).toBe('Updated');
    expect((await firstValueFrom(repository.archive(created.id))).archived).toBe(true);
  });
});
