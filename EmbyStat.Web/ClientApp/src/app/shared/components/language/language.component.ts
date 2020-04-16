import { Component, Input } from '@angular/core';

import { Language } from '../../models/language';

@Component({
  selector: 'es-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss']
})
export class LanguageComponent {
  @Input() language: Language;
}
