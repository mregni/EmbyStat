import { Injectable } from '@angular/core';

import { SystemService } from './system.service';
import { UpdateOverlayService } from './update-overlay.service';

@Injectable()
export class UpdateService {
  private backendIsOnline = true;
  private intervalId;

  constructor(
    private systemService: SystemService,
    private updateOverlayService: UpdateOverlayService) {

  }

  setUiToUpdateState(value: boolean): void {
    this.updateOverlayService.show(value);

    if (value && this.intervalId === undefined) {
      this.intervalId = setInterval(() => {
        this.systemService.ping().subscribe(() => {
          if (!this.backendIsOnline) {
            window.location.reload();
          }
        }, error => {
          this.backendIsOnline = false;
        });
      }, 5000);
    } else if (this.intervalId !== undefined) {
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }
}
