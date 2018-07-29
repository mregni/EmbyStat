import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Activity } from '../models/activity';
import { ActivityService } from '../service/activity.service';

@Injectable()
export class ActivityFacade {
  constructor( private activityService: ActivityService ) { }

  getActivities(): Observable<Activity> {
    return this.activityService.getActivities();
  }
}

