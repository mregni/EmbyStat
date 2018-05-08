import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class ToastService {
  constructor(private http: HttpClient,
              private snackBar: MatSnackBar,
              private translate: TranslateService) {

  }

  pushError(error: string) {
    this.translate.get(error).subscribe(translation => {
      this.snackBar.open(translation, 'Hide', { duration: 10000, horizontalPosition: "right" });
    });
  }
}
