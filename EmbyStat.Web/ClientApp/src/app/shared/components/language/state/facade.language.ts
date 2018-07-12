import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { LanguageService } from '../service/language.service';
import { Language } from '../models/language';

@Injectable()
export class LanguageFacade {

  constructor(private languageService: LanguageService) { }

  getLanguages(): Observable<Language[]> {
    return this.languageService.getLanguages();
  }
}
