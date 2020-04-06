import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class ToastService {
  private hideTranslation: string;

  constructor(private snackBar: MatSnackBar,
              private translate: TranslateService) {
    this.translate.get('COMMON.HIDE').subscribe(translation => this.hideTranslation = translation);
  }

  showError(error: string) {
    this.translate.get(error).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 10000, horizontalPosition: 'right', panelClass: 'toast-fail' });
    });
  }

  showWarning(message: string) {
    this.translate.get(message).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 10000, horizontalPosition: 'right', panelClass: 'toast-warning' });
    });
  }

  showSuccess(message: string) {
    this.translate.get(message).subscribe(translation => {
      this.snackBar.open(translation, this.hideTranslation, { duration: 5000, horizontalPosition: 'right', panelClass: 'toast-success' });
    });
  }
}
