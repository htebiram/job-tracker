import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  ApplicationDraft,
  ApplicationSort,
  ApplicationStatus,
  JobApplication,
} from '../models/application.models';
import { ApplicationRepository } from '../data/application.repository';
import { MockApplicationRepository } from '../data/mock-application.repository';

const PAGE_SIZE = 5;
const STATUSES: readonly ApplicationStatus[] = ['Applied', 'Interview', 'Offer', 'Rejected'];

@Component({
  selector: 'app-applications-page',
  imports: [ReactiveFormsModule],
  providers: [{ provide: ApplicationRepository, useClass: MockApplicationRepository }],
  templateUrl: './applications-page.html',
  styleUrl: './applications-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationsPage {
  private readonly repository = inject(ApplicationRepository);
  private readonly applications = toSignal(this.repository.getAll(), { initialValue: [] });

  protected readonly statuses = STATUSES;
  protected readonly search = signal('');
  protected readonly status = signal<ApplicationStatus | 'All'>('All');
  protected readonly sort = signal<ApplicationSort>('newest');
  protected readonly page = signal(1);
  protected readonly selected = signal<JobApplication | null>(null);
  protected readonly editing = signal<JobApplication | null>(null);
  protected readonly isFormOpen = signal(false);
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

  protected applyFilters(): void {
    this.page.set(1);
  }
  protected updateSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
    this.applyFilters();
  }
  protected updateStatus(event: Event): void {
    this.status.set((event.target as HTMLSelectElement).value as ApplicationStatus | 'All');
    this.applyFilters();
  }
  protected updateSort(event: Event): void {
    this.sort.set((event.target as HTMLSelectElement).value as ApplicationSort);
  }
  protected statusClass(status: ApplicationStatus): string {
    return status.toLowerCase();
  }
  protected changePage(delta: number): void {
    this.page.update((page) => Math.min(this.pageCount(), Math.max(1, page + delta)));
  }
  protected openCreate(): void {
    this.editing.set(null);
    this.form.reset({ status: 'Applied', appliedDate: new Date().toISOString().slice(0, 10) });
    this.isFormOpen.set(true);
  }
  protected openEdit(application: JobApplication): void {
    this.editing.set(application);
    this.form.reset(application);
    this.isFormOpen.set(true);
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const draft = this.form.getRawValue() as ApplicationDraft;
    const editing = this.editing();
    const request = editing
      ? this.repository.update(editing.id, draft)
      : this.repository.create(draft);
    request.subscribe((application) => {
      this.selected.set(application);
      this.isFormOpen.set(false);
    });
  }
  protected confirmDelete(): void {
    const application = this.pendingDelete();
    if (!application) return;
    this.repository.delete(application.id).subscribe(() => {
      if (this.selected()?.id === application.id) this.selected.set(null);
      this.pendingDelete.set(null);
      this.page.set(Math.min(this.page(), this.pageCount()));
    });
  }
}
