import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatDialog } from '@angular/material';

import { SyncIsRunningDialog } from '../dialogs/sync-is-running/sync-is-running.component';
import { JobSocketService } from '../services/job-socket.service';

@Injectable()
export class SyncGuard implements CanActivate {
  constructor(
    private router: Router,
    private jobSocketService: JobSocketService,
    private dialog: MatDialog) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.jobSocketService.isSyncRunning) {
      this.dialog.open(SyncIsRunningDialog,
        {
          width: '550px'
        });
      return false;
    }

    return true;
  }
}
