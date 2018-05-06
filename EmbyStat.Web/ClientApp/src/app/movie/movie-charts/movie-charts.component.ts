import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit {
  private _selectedCollections: string[];

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    console.log(collection);
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
  }

  public view: number[] = [700, 350];
  public doughnut: boolean = false;
  public data: any = [
    {
      "name": "Germany",
      "value": 40632
    },
    {
      "name": "France",
      "value": 36745
    },
    {
      "name": "United Kingdom",
      "value": 36240
    },
    {
      "name": "Spain",
      "value": 33000
    },
    {
      "name": "Uganda",
      "value": 19294
    },
    {
      "name": "Bonaire, Sint Eustatius and Saba",
      "value": 50184
    },
    {
      "name": "India",
      "value": 20303
    }
  ];
  constructor() { }

  ngOnInit() {
  }

}
