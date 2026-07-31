import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { take } from 'rxjs';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { MockSettingsRepository } from '../data/mock-settings.repository';
import { SettingsRepository } from '../data/settings.repository';
import { WeekStart, WorkspaceSettings } from '../models/settings.models';

@Component({
  selector: 'app-settings-page',
  imports: [PageHeader, ReactiveFormsModule],
  providers: [{ provide: SettingsRepository, useExisting: MockSettingsRepository }],
  templateUrl: './settings-page.html',
  styleUrl: './settings-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsPage {
  private readonly repository = inject(SettingsRepository);
  private readonly settings = toSignal(this.repository.get(), { requireSync: true });
  protected readonly saved = signal(false);
  protected readonly form = new FormGroup({
    displayName: new FormControl(this.settings().displayName, {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(80)],
    }),
    email: new FormControl(this.settings().email, {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    weekStart: new FormControl<WeekStart>(this.settings().weekStart, { nonNullable: true }),
    interviewReminders: new FormControl(this.settings().interviewReminders, { nonNullable: true }),
    taskReminders: new FormControl(this.settings().taskReminders, { nonNullable: true }),
    weeklySummary: new FormControl(this.settings().weeklySummary, { nonNullable: true }),
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.repository
      .update(this.form.getRawValue() as WorkspaceSettings)
      .pipe(take(1))
      .subscribe(() => {
        this.saved.set(true);
        this.form.markAsPristine();
      });
  }
}
