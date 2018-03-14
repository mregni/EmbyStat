import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { SystemService } from '../service/system.service';

@Injectable()
export class SystemFacade {
  constructor(private systemService: SystemService) { }

  shutdownServer(): Observable<void> {
    return this.systemService.shutdown();
  }

}

