import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ApplicationSort, ApplicationStatus } from '../../models/application.models';

@Component({
  selector: 'app-application-filters',
  template: `
    <div class="filters" aria-label="Application filters">
      <label>
        Search
        <input
          type="search"
          placeholder="Company or role"
          [value]="search()"
          (input)="searchChange.emit(valueFrom($event))"
        />
      </label>
      <label>
        Status
        <select [value]="status()" (change)="statusChange.emit(statusFrom($event))">
          <option>All</option>
          @for (item of statuses(); track item) {
            <option>{{ item }}</option>
          }
        </select>
      </label>
      <label>
        Sort
        <select [value]="sort()" (change)="sortChange.emit(sortFrom($event))">
          <option value="newest">Newest first</option>
          <option value="oldest">Oldest first</option>
          <option value="company">Company</option>
        </select>
      </label>
    </div>
  `,
  styles: `
    .filters {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr;
      gap: var(--space-4);
      margin: var(--space-6) 0;
    }
    label {
      display: grid;
      gap: var(--space-2);
      color: var(--color-text-muted);
      font-size: var(--font-size-xs);
      font-weight: 700;
    }
    input,
    select {
      inline-size: 100%;
      padding: 0.75rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-surface);
      color: var(--color-text);
    }
    @media (max-width: 48rem) {
      .filters {
        grid-template-columns: 1fr;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationFilters {
  readonly statuses = input.required<readonly ApplicationStatus[]>();
  readonly search = input.required<string>();
  readonly status = input.required<ApplicationStatus | 'All'>();
  readonly sort = input.required<ApplicationSort>();
  readonly searchChange = output<string>();
  readonly statusChange = output<ApplicationStatus | 'All'>();
  readonly sortChange = output<ApplicationSort>();

  protected valueFrom(event: Event): string {
    return (event.target as HTMLInputElement).value;
  }
  protected statusFrom(event: Event): ApplicationStatus | 'All' {
    return (event.target as HTMLSelectElement).value as ApplicationStatus | 'All';
  }
  protected sortFrom(event: Event): ApplicationSort {
    return (event.target as HTMLSelectElement).value as ApplicationSort;
  }
}
