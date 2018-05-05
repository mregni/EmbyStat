import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit {
  public view: number[] = [700, 400];
  public doughnut: boolean = false;
  public data: any = [
    {
      "name": "Germany",
      "value": 8940000
    },
    {
      "name": "USA",
      "value": 5000000
    },
    {
      "name": "France",
      "value": 7200000
    }
  ];
  constructor() { }

  ngOnInit() {
  }

}
