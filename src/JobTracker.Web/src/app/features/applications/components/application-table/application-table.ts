import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ApplicationStatus, JobApplication } from '../../models/application.models';
import { Badge, BadgeTone } from '@shared/ui/badge/badge';
import { EmptyState } from '@shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-application-table',
  imports: [Badge, EmptyState],
  templateUrl: './application-table.html',
  styleUrl: './application-table.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationTable {
  readonly applications = input.required<readonly JobApplication[]>();
  readonly total = input.required<number>();
  readonly page = input.required<number>();
  readonly pageCount = input.required<number>();
  readonly selected = output<JobApplication>();
  readonly edited = output<JobApplication>();
  readonly deleted = output<JobApplication>();
  readonly pageChanged = output<number>();

  protected statusTone(status: ApplicationStatus): BadgeTone {
    const tones: Record<ApplicationStatus, BadgeTone> = {
      Applied: 'primary',
      Interview: 'info',
      Offer: 'success',
      Rejected: 'danger',
    };
    return tones[status];
  }
}
