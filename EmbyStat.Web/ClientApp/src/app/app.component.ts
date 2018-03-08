import { Component, NgZone } from '@angular/core';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private smallWidthBreakpoint = 720;
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);

  constructor(zone: NgZone) {
    this.mediaMatcher.addListener(mql =>
      zone.run(() => this.mediaMatcher = mql));
  }

  public isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
