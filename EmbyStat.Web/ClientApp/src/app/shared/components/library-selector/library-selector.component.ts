import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'es-library-selector',
  templateUrl: './library-selector.component.html',
  styleUrls: ['./library-selector.component.scss']
})
export class LibrarySelectorComponent {
  private privateList: number[];
  public form: FormGroup;

  public movieTypeControl = new FormControl(false, []);
  public showTypeControl = new FormControl(false, []);
  public mixedTypeControl = new FormControl(false, []);
  public musicTypeControl = new FormControl(false, []);
  public musicVideoTypeControl = new FormControl(false, []);
  public trailersTypeControl = new FormControl(false, []);
  public homeTypeControl = new FormControl(false, []);
  public boxTypeControl = new FormControl(false, []);
  public bookTypeControl = new FormControl(false, []);
  public photoTypeControl = new FormControl(false, []);
  public gameTypeControl = new FormControl(false, []);
  public liveTypeControl = new FormControl(false, []);
  public playTypeControl = new FormControl(false, []);
  public folderTypeControl = new FormControl(false, []);

  @Output() newList = new EventEmitter<number[]>();
  @Input() set list(value: number[]) {
    this.newList.emit(value);
    this.privateList = [];
    value.forEach(x => this.privateList.push(x));

    this.mixedTypeControl.setValue(value.some(x => x === 0));
    this.movieTypeControl.setValue(value.some(x => x === 1));
    this.showTypeControl.setValue(value.some(x => x === 2));
    this.musicTypeControl.setValue(value.some(x => x === 3));
    this.musicVideoTypeControl.setValue(value.some(x => x === 4));
    this.trailersTypeControl.setValue(value.some(x => x === 5));
    this.homeTypeControl.setValue(value.some(x => x === 6));
    this.bookTypeControl.setValue(value.some(x => x === 8));
    this.photoTypeControl.setValue(value.some(x => x === 9));
    this.gameTypeControl.setValue(value.some(x => x === 10));
    this.liveTypeControl.setValue(value.some(x => x === 11));
    this.playTypeControl.setValue(value.some(x => x === 12));
    this.folderTypeControl.setValue(value.some(x => x === 13));
  }

  constructor() {
    this.form = new FormGroup({
      movieType: this.movieTypeControl,
      showType: this.showTypeControl,
      mixedType: this.mixedTypeControl,
      musicType: this.musicTypeControl,
      musicVideoType: this.musicVideoTypeControl,
      trailersType: this.trailersTypeControl,
      homeType: this.homeTypeControl,
      bookType: this.bookTypeControl,
      photoType: this.photoTypeControl,
      gameType: this.gameTypeControl,
      liveType: this.liveTypeControl,
      playType: this.playTypeControl,
      folderType: this.folderTypeControl
    });
  }

  onChange(event): void {
    const checkbox = event.source.value;
    const index = this.privateList.indexOf(+checkbox, 0);


    if (event.checked) {
      if (index === -1) {
        this.privateList.push(+checkbox);
      }
    } else {
      if (index > -1) {
        this.privateList.splice(index, 1);
      }
    }
    this.newList.emit(this.privateList);
  }
}
