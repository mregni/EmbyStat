import { TitleService } from 'src/app/shared/services/title.service';

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-movie-container',
  templateUrl: './movie-container.component.html',
  styleUrls: ['./movie-container.component.scss']
})
export class MovieContainerComponent implements OnInit {

  constructor(private readonly titleService: TitleService) {
    this.titleService.updateTitle('Movies');
  }

  ngOnInit() {
  }

}
