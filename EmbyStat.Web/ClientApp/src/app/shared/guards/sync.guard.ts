import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';

import { SyncIsRunningDialog } from '../dialogs/sync-is-running/sync-is-running.component';
import { Job } from '../models/jobs/job';
import { JobService } from '../services/job.service';

@Injectable()
export class SyncGuard implements CanActivate {
  constructor(
    private router: Router,
    private jobService: JobService,
    private dialog: MatDialog) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.jobService.getMediaSyncJob().pipe(
      map((job: Job) => {
      if (job.state === 1) {
        this.dialog.open(SyncIsRunningDialog,
          {
            width: '550px'
          });
        return false;
      }

      return true;
    }));
  }
}
