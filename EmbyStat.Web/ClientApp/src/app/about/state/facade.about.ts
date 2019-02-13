import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';

import { About } from '../models/about';
import { AboutService } from '../service/about.service';

import { AboutQuery } from './reducer.about';
import { LoadAboutAction } from './actions.about';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class AboutFacade {
  constructor(
    private store: Store<ApplicationState>,
    private aboutService: AboutService
  ) { }

  about$ = this.store.select(AboutQuery.getAbout);

  getAbout(): Observable<About> {
    this.store.dispatch(new LoadAboutAction);
    return this.about$;
  }
}

