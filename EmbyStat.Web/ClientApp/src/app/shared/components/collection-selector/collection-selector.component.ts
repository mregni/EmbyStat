import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';


@Component({
  selector: 'app-collection-selector',
  templateUrl: './collection-selector.component.html',
  styleUrls: ['./collection-selector.component.scss']
})
export class CollectionSelectorComponent implements OnInit {
  private prvList: number[];
  public form: FormGroup;

  public movieType: FormControl = new FormControl('', []);
  public showType: FormControl = new FormControl('', []);
  public mixedType: FormControl = new FormControl('', []);
  public musicType: FormControl = new FormControl('', []);
  public musicVideoType: FormControl = new FormControl('', []);
  public trailersType: FormControl = new FormControl('', []);
  public homeType: FormControl = new FormControl('', []);
  public boxType: FormControl = new FormControl('', []);
  public bookType: FormControl = new FormControl('', []);
  public photoType: FormControl = new FormControl('', []);
  public gameType: FormControl = new FormControl('', []);
  public liveType: FormControl = new FormControl('', []);
  public playType: FormControl = new FormControl('', []);
  public folderType: FormControl = new FormControl('', []);

  @Output() listChange: EventEmitter<number[]> = new EventEmitter<number[]>();
  @Input() set list(value: number[]) {
    console.log(value);
    this.prvList = value;
    this.form.setValue({ mixedType: value.some(x => x === 0) });
    this.form.setValue({ movieType: value.some(x => x === 1) });
    this.form.setValue({ showType: value.some(x => x === 2) });
    this.form.setValue({ musicType: value.some(x => x === 3) });
    this.form.setValue({ musicVideoType: value.some(x => x === 4) });
    this.form.setValue({ trailerType: value.some(x => x === 5) });
    this.form.setValue({ homeType: value.some(x => x === 6) });
    this.form.setValue({ boxType: value.some(x => x === 7) });
    this.form.setValue({ bookType: value.some(x => x === 8) });
    this.form.setValue({ photoType: value.some(x => x === 9) });
    this.form.setValue({ gameType: value.some(x => x === 10) });
    this.form.setValue({ liveType: value.some(x => x === 11) });
    this.form.setValue({ playType: value.some(x => x === 12) });
    this.form.setValue({ folderType: value.some(x => x === 13) });
  }
  constructor() {
    this.form = new FormGroup({
      movieType: this.movieType,
      showType: this.showType,
      mixedType: this.mixedType,
      musicType: this.musicType,
      musicVideoType: this.musicVideoType,
      trailerType: this.trailersType,
      homeType: this.homeType,
      boxType: this.boxType,
      bookType: this.bookType,
      photoType: this.photoType,
      gameType: this.gameType,
      liveType: this.liveType,
      playType: this.playType,
      folderType: this.folderType
    });
  }

  onChange(event) {
    console.log(event);
  }

  ngOnInit() {
  }

}
