import { firstValueFrom } from 'rxjs';
import { MockApplicationRepository } from './mock-application.repository';

describe('MockApplicationRepository', () => {
  it('supports create, update, and delete', async () => {
    const repository = new MockApplicationRepository();
    const draft = {
      company: 'Test Co',
      role: 'Engineer',
      location: 'Remote',
      status: 'Applied' as const,
      appliedDate: '2026-07-31',
      source: 'Test',
      notes: '',
    };
    const created = await firstValueFrom(repository.create(draft));
    expect((await firstValueFrom(repository.getAll())).some((item) => item.id === created.id)).toBe(
      true,
    );
    await firstValueFrom(repository.update(created.id, { ...draft, status: 'Interview' }));
    expect(
      (await firstValueFrom(repository.getAll())).find((item) => item.id === created.id)?.status,
    ).toBe('Interview');
    await firstValueFrom(repository.delete(created.id));
    expect((await firstValueFrom(repository.getAll())).some((item) => item.id === created.id)).toBe(
      false,
    );
  });
});
