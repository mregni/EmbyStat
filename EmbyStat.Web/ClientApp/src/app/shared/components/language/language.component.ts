import { Component, Input, OnInit } from '@angular/core';

import { Language } from '../../models/language';

@Component({
  selector: 'es-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss']
})
export class LanguageComponent implements OnInit {
  @Input() language: Language;
  constructor() { }

  ngOnInit() {
  }

}
