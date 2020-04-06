import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { Options } from '../../../components/charts/options/options';
import { Chart } from '../../../models/charts/chart';
import { OptionsService, Title } from '../options/options';

@Component({
  selector: 'es-simple-chart',
  templateUrl: './simple-chart.component.html',
  styleUrls: ['./simple-chart.component.scss']
})
export class SimpleChartComponent implements OnInit, OnChanges, OnDestroy {
  translateSub: Subscription;
  @Input() chart: Chart;
  @Input() type = 'bar';
  @Input() height = 350;
  @Input() options: Options;

  data = {
    labels: [],
    datasets: []
  };

  constructor(
    private readonly translateService: TranslateService,
    private readonly optionsService: OptionsService) {
  }

  ngOnChanges(): void {
    if (this.options.title !== undefined) {
      this.translateSub = this.translateService.get(this.chart.title).subscribe((value: string) => {
        this.options.title.text = value;
      });
    }

    this.data.labels = this.chart.labels;
    this.chart.dataSets.forEach(x =>
      this.data.datasets.push({
        label: 't',
        data: x,
        backgroundColor: this.optionsService.getColors(x.length),
        borderColor: this.optionsService.getColors(x.length)
      }));
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    if (this.translateSub !== undefined) {
      this.translateSub.unsubscribe();
    }
  }
}
