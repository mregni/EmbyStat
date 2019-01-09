import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import * as signalR from '@aspnet/signalr';

import { ConfigurationFacade } from './configuration/state/facade.configuration';
import { WizardStateService } from './wizard/services/wizard-state.service';
import { TaskSignalService } from './shared/services/task-signal.service';
import { SideBarService } from './shared/services/side-bar.service';
import { Task } from './task/models/task';
import { ProgressLog } from './task/models/progress-log';

const SMALL_WIDTH_BREAKPOINT = 768;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private configChangedSub: Subscription;
  private configLoadSub: Subscription;
  private wizardStateSub: Subscription;
  openMenu = true;

  constructor(
    private zone: NgZone,
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private router: Router,
    private wizardStateService: WizardStateService,
    private taskSignalService: TaskSignalService,
    private sideBarService: SideBarService) {
    this.mediaMatcher.addListener(mql => zone.run(() => this.mediaMatcher = mql));

    translate.setDefaultLang('en-US');
    translate.addLangs(['en-US', 'nl-NL']);

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/tasksignal')
      .build();
    hubConnection.start().catch(err => document.write(err));

    hubConnection.on('ReceiveInfo', (data: Task[]) => {
      taskSignalService.updateTasksInfo(data);
    });

    hubConnection.on('ReceiveLog', (data: ProgressLog) => {
      taskSignalService.updateTasksLogs(data.value, data.type);
    });

    sideBarService.menuVisibleSubject.subscribe((state: boolean) => {
      this.openMenu = state;
    });
  }

  ngOnInit(): void {
    this.configLoadSub = this.configurationFacade.getConfiguration().subscribe(config => {
      if (!config.wizardFinished) {
        this.router.navigate(['/wizard']);
        this.openMenu = false;
      }
    });

    this.wizardStateSub = this.wizardStateService.finished.subscribe(finished => {
      if (finished) {
        this.openMenu = true;
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

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
