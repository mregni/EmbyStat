import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationFacade } from './configuration/state/facade.configuration';
import { WizardStateService } from './wizard/services/wizard-state.service';

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
  private configLoadSub: Subscription;
  private wizardStateSub: Subscription;
  public closeForWizard = false;

  constructor(
    private zone: NgZone,
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private router: Router,
    private wizardStateService: WizardStateService) {
    this.mediaMatcher.addListener(mql => zone.run(() => this.mediaMatcher = mql));

    translate.setDefaultLang('en-US');
    translate.addLangs(['en-US', 'nl-NL']);
  }

  ngOnInit(): void {
    this.configLoadSub = this.configurationFacade.getConfiguration().subscribe(config => {
      if (!config.wizardFinished) {
        this.router.navigate(['/wizard']);
        this.closeForWizard = true;
      }
    });

    this.wizardStateSub = this.wizardStateService.finished.subscribe(finished => {
      if (finished) {
        this.closeForWizard = false;
        this.router.navigate(['']);
      }
    });

    this.configChangedSub = this.configurationFacade.configuration$.subscribe(config => {
      this.translate.use(config.language);
    });
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }

    if (this.configLoadSub !== undefined) {
      this.configLoadSub.unsubscribe();
    }

    if (this.wizardStateSub !== undefined) {
      this.wizardStateSub.unsubscribe();
    }
  }

  public isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
