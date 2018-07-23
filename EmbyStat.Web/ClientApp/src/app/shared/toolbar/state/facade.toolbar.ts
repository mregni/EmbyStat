import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';

import { EmbyService } from '../../services/emby.service';
import { EmbyStatus } from '../../models/emby/embyStatus';

@Injectable()
export class ToolbarFacade {
  constructor(private embyService: EmbyService) { }

  getEmbyStatus(): Observable<EmbyStatus> {
    return this.embyService.getEmbyStatus();
  }
}
