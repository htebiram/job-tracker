import { ChangeDetectionStrategy, Component, OnInit, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  ApplicationDraft,
  ApplicationStatus,
  JobApplication,
} from '../../models/application.models';
import { DialogShell } from '@shared/ui/dialog-shell/dialog-shell';

const STATUSES: readonly ApplicationStatus[] = ['Applied', 'Interview', 'Offer', 'Rejected'];

@Component({
  selector: 'app-application-form-dialog',
  imports: [DialogShell, ReactiveFormsModule],
  templateUrl: './application-form-dialog.html',
  styleUrl: './application-form-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationFormDialog implements OnInit {
  readonly application = input<JobApplication | null>(null);
  readonly saved = output<ApplicationDraft>();
  readonly cancelled = output<void>();
  protected readonly statuses = STATUSES;
  protected readonly form = new FormGroup({
    company: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(80)],
    }),
    role: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    location: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    status: new FormControl<ApplicationStatus>('Applied', { nonNullable: true }),
    appliedDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    source: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(500)] }),
  });

  ngOnInit(): void {
    const application = this.application();
    this.form.reset(
      application ?? { status: 'Applied', appliedDate: new Date().toISOString().slice(0, 10) },
    );
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saved.emit(this.form.getRawValue() as ApplicationDraft);
  }
}
