import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationDetails } from '../components/application-details/application-details';
import { ApplicationFilters } from '../components/application-filters/application-filters';
import { ApplicationFormDialog } from '../components/application-form-dialog/application-form-dialog';
import { ApplicationTable } from '../components/application-table/application-table';
import { DeleteApplicationDialog } from '../components/delete-application-dialog/delete-application-dialog';
import {
  ApplicationDraft,
  ApplicationSort,
  ApplicationStatus,
  JobApplication,
} from '../models/application.models';
import { ApplicationRepository } from '../data/application.repository';
import { MockApplicationRepository } from '../data/mock-application.repository';
import { PageHeader } from '@shared/ui/page-header/page-header';

const PAGE_SIZE = 5;
const STATUSES: readonly ApplicationStatus[] = ['Applied', 'Interview', 'Offer', 'Rejected'];

@Component({
  selector: 'app-applications-page',
  imports: [
    ApplicationDetails,
    ApplicationFilters,
    ApplicationFormDialog,
    ApplicationTable,
    DeleteApplicationDialog,
    PageHeader,
  ],
  providers: [{ provide: ApplicationRepository, useExisting: MockApplicationRepository }],
  templateUrl: './applications-page.html',
  styleUrl: './applications-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationsPage {
  private readonly repository = inject(ApplicationRepository);
  private readonly route = inject(ActivatedRoute);
  private readonly applications = toSignal(this.repository.getAll(), { initialValue: [] });

  protected readonly statuses = STATUSES;
  protected readonly search = signal('');
  protected readonly status = signal<ApplicationStatus | 'All'>('All');
  protected readonly sort = signal<ApplicationSort>('newest');
  protected readonly page = signal(1);
  protected readonly selected = signal<JobApplication | null>(null);
  protected readonly editing = signal<JobApplication | null>(null);
  protected readonly isFormOpen = signal(
    this.route.snapshot.queryParamMap.get('action') === 'create',
  );
  protected readonly pendingDelete = signal<JobApplication | null>(null);

  protected readonly filtered = computed(() => {
    const query = this.search().trim().toLowerCase();
    const status = this.status();
    return [...this.applications()]
      .filter(
        (item) =>
          (status === 'All' || item.status === status) &&
          (!query ||
            item.company.toLowerCase().includes(query) ||
            item.role.toLowerCase().includes(query)),
      )
      .sort((a, b) => {
        if (this.sort() === 'company') return a.company.localeCompare(b.company);
        return this.sort() === 'oldest'
          ? a.appliedDate.localeCompare(b.appliedDate)
          : b.appliedDate.localeCompare(a.appliedDate);
      });
  });
  protected readonly pageCount = computed(() =>
    Math.max(1, Math.ceil(this.filtered().length / PAGE_SIZE)),
  );
  protected readonly visible = computed(() =>
    this.filtered().slice((this.page() - 1) * PAGE_SIZE, this.page() * PAGE_SIZE),
  );

  private applyFilters(): void {
    this.page.set(1);
  }
  protected updateSearch(value: string): void {
    this.search.set(value);
    this.applyFilters();
  }
  protected updateStatus(value: ApplicationStatus | 'All'): void {
    this.status.set(value);
    this.applyFilters();
  }
  protected updateSort(value: ApplicationSort): void {
    this.sort.set(value);
  }
  protected changePage(delta: number): void {
    this.page.update((page) => Math.min(this.pageCount(), Math.max(1, page + delta)));
  }
  protected openCreate(): void {
    this.editing.set(null);
    this.isFormOpen.set(true);
  }
  protected openEdit(application: JobApplication): void {
    this.editing.set(application);
    this.isFormOpen.set(true);
  }
  protected save(draft: ApplicationDraft): void {
    const editing = this.editing();
    const request = editing
      ? this.repository.update(editing.id, draft)
      : this.repository.create(draft);
    request.pipe(take(1)).subscribe((application) => {
      this.selected.set(application);
      this.isFormOpen.set(false);
    });
  }
  protected confirmDelete(): void {
    const application = this.pendingDelete();
    if (!application) return;
    this.repository
      .delete(application.id)
      .pipe(take(1))
      .subscribe(() => {
        if (this.selected()?.id === application.id) this.selected.set(null);
        this.pendingDelete.set(null);
        this.page.set(Math.min(this.page(), this.pageCount()));
      });
  }
}
