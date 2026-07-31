import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { MockSettingsRepository } from './mock-settings.repository';

describe('MockSettingsRepository', () => {
  it('updates local workspace settings', async () => {
    const repository = TestBed.runInInjectionContext(() => new MockSettingsRepository());
    const settings = await firstValueFrom(repository.get());
    await firstValueFrom(repository.update({ ...settings, weeklySummary: true }));

    expect((await firstValueFrom(repository.get())).weeklySummary).toBe(true);
  });
});
