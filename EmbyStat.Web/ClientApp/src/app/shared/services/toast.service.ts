import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class ToastService {
  private hideTranslation: string;

  constructor(private http: HttpClient,
              private snackBar: MatSnackBar,
              private translate: TranslateService) {
    this.translate.get('COMMON.HIDE').subscribe(translation => this.hideTranslation = translation);
  }

  pushError(error: string) {
    this.translate.get(error).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 10000, horizontalPosition: 'right', panelClass: 'toast__fail' });
    });
  }

  pushWarning(message: string) {
    this.translate.get(message).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 10000, horizontalPosition: 'right', panelClass: 'toast__warning' });
    });
  }

  pushSuccess(message: string) {
    this.translate.get(message).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 5000, horizontalPosition: 'right', panelClass: 'toast__success' });
    });
  }
}
