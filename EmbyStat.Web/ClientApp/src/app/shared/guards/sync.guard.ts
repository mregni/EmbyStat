import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatDialog } from '@angular/material';

import { SyncIsRunningDialog } from '../dialogs/sync-is-running/sync-is-running.component';
import { JobService } from '../../jobs/service/job.service';

@Injectable()
export class SyncGuard implements CanActivate {
  constructor(
    private router: Router,
    private jobService: JobService,
    private dialog: MatDialog) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.jobService.getMediaSyncJob().map(job => {
      if (job.state === 1) {
        this.dialog.open(SyncIsRunningDialog,
          {
            width: '550px'
          });
        return false;
      }

      return true;
    });
  }
}
