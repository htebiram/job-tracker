import { TestBed } from '@angular/core/testing';
import { SettingsPage } from './settings-page';
import { MockSettingsRepository } from '../data/mock-settings.repository';
import { SettingsRepository } from '../data/settings.repository';

describe('SettingsPage', () => {
  it('renders local settings and confirms saved changes', async () => {
    await TestBed.configureTestingModule({ imports: [SettingsPage] }).compileComponents();
    const fixture = TestBed.createComponent(SettingsPage);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    expect(fixture.debugElement.injector.get(SettingsRepository)).toBe(
      TestBed.inject(MockSettingsRepository),
    );

    expect(element.querySelectorAll('.panel')).toHaveLength(3);
    expect((element.querySelector('[formControlName=displayName]') as HTMLInputElement).value).toBe(
      'Jordan Davis',
    );

    (element.querySelector('button[type=submit]') as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(element.querySelector('[role=status]')?.textContent).toContain('saved locally');
  });
});
