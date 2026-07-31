import { firstValueFrom } from 'rxjs';
import { MockCalendarRepository } from './mock-calendar.repository';
describe('MockCalendarRepository', () => {
  it('returns every supported event type', async () => {
    const events = await firstValueFrom(new MockCalendarRepository().getAll());
    expect(new Set(events.map((event) => event.type))).toEqual(
      new Set(['Interview', 'Reminder', 'Deadline', 'Application']),
    );
  });
});
