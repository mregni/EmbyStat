import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatDialog } from '@angular/material';

import { SyncIsRunningDialog } from '../dialogs/sync-is-running/sync-is-running.component';
import { TaskSignalService } from '../services/signalR/task-signal.service';
import { Task } from '../../task/models/task';

@Injectable()
export class SyncGuard implements CanActivate {
  constructor(
    private router: Router,
    private taskSignalService: TaskSignalService,
    private dialog: MatDialog) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.taskSignalService.isSyncRunning) {
      this.dialog.open(SyncIsRunningDialog,
        {
          width: '550px'
        });
      return false;
    }

    return true;
  }
}
