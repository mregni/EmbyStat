import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router, NavigationStart } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationFacade } from './configuration/state/facade.configuration';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private smallWidthBreakpoint = 720;
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private configChangedSub: Subscription;
  public closeForWizard: boolean = false;

  constructor(
    private zone: NgZone,
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private router: Router) {
    this.mediaMatcher.addListener(mql => zone.run(() => this.mediaMatcher = mql));

    translate.setDefaultLang('en');
    translate.addLangs(["en", "nl"]);

    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (event.url !== '/wizard') {
          this.configurationFacade.configuration$.subscribe(config => {
            if (!config.wizardFinished) {
              this.router.navigate(['/wizard']);
              this.closeForWizard = true;
            } else {
              this.closeForWizard = false;
            }
          });
        } else {
          this.closeForWizard = true;
        }
      }
    });
  }

  ngOnInit(): void {
    this.configurationFacade.getConfiguration();

    this.configChangedSub = this.configurationFacade.configuration$.subscribe(config => {
      this.translate.use(config.language);
    });
  }

  ngOnDestroy() {
    this.configChangedSub.unsubscribe();
  }

  public isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
