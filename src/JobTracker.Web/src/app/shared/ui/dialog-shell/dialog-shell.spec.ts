import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { DialogShell } from './dialog-shell';

@Component({
  imports: [DialogShell],
  template: `
    <app-dialog-shell labelledBy="test-dialog-title" (dismissed)="closed = true">
      <h2 id="test-dialog-title">Test dialog</h2>
    </app-dialog-shell>
  `,
})
class DialogShellHost {
  closed = false;
}

describe('DialogShell', () => {
  it('receives initial focus and emits dismissal on Escape', async () => {
    await TestBed.configureTestingModule({ imports: [DialogShellHost] }).compileComponents();
    const fixture = TestBed.createComponent(DialogShellHost);
    fixture.detectChanges();
    await fixture.whenStable();

    const dialog = fixture.nativeElement.querySelector('[role="dialog"]') as HTMLElement;
    expect(document.activeElement).toBe(dialog);

    dialog.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));

    expect(fixture.componentInstance.closed).toBe(true);
  });
});
