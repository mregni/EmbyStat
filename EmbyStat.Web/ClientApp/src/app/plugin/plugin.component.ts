import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PluginFacade } from './state/facade.plugin';
import { EmbyPlugin } from './models/embyPlugin';

@Component({
  selector: 'app-plugin',
  templateUrl: './plugin.component.html',
  styleUrls: ['./plugin.component.scss']
})
export class PluginComponent implements OnInit {
  plugins$: Observable<EmbyPlugin>;

  constructor(private pluginFacade: PluginFacade) { }

  ngOnInit() {
  }

}
