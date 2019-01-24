import { Injectable } from '@angular/core';

import { SystemService } from './system.service';

@Injectable()
export class UpdateService {
  private backendIsOnline = true;
  private intervalId;
  constructor(private systemService: SystemService) {

  }

  startPing() {
    if (this.intervalId === undefined) {
      this.intervalId = setInterval(() => {
        this.systemService.ping().subscribe(() => {
          if (!this.backendIsOnline) {
            window.location.reload(true);
          }
        }, error => {
          this.backendIsOnline = false;
        });
      }, 5000);
    }
  }

}
