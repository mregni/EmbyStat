import { Component, Input, OnChanges, OnInit } from '@angular/core';

import { Chart } from '../../../models/charts/chart';

@Component({
  selector: 'app-simple-bar-chart',
  templateUrl: './simple-bar-chart.component.html',
  styleUrls: ['./simple-bar-chart.component.scss']
})
export class SimpleBarChartComponent implements OnInit, OnChanges {
  @Input() chart: Chart;

  data = {
    labels: [],
    datasets: []
  };

  options = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true,
          fontColor: '#CCC'
        }
      }],
      xAxes: [{
        gridLines: {
          display: false,
        },
        ticks: {
          autoSkip: false,
          fontColor: '#CCC'
        },
      }]
    },
    legend: { display: false },
    title: { display: false },
    tooltips: {
      callbacks: {
        label: function(tooltipItem, data) {
            return ' ' + Math.round(tooltipItem.yLabel * 100) / 100;
        }
    }
    }
  };

  constructor() {
  }

  ngOnChanges(): void {
    this.data.labels = this.chart.labels;
    this.chart.dataSets.forEach(x => this.data.datasets.push({ label: 'test', data: x, backgroundColor: 'rgba(0,60,100,1)', borderColor: 'rgba(10,150,132,1)' }));
    console.log(this.chart);
  }

  ngOnInit() {
  }
}
