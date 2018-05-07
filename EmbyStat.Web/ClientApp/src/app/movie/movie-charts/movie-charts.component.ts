import { Component, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MovieChartsService } from '../service/movie-charts.service';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit  {
  private _selectedCollections: string[];

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
  }

  public data: any;

  constructor(private http: HttpClient, private movieChartsService: MovieChartsService) {
    this.data = [];

    movieChartsService.open.subscribe(value => {
      if (value) {
        this.http.get<any[]>('api/movie/getchartdata').subscribe(x => {
          console.log(x);
          this.data = JSON.parse(x.toString());
        });
      }
    });
  }

  ngOnInit() {

  }
}
