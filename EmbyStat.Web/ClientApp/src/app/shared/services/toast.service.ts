import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material';

@Injectable()
export class ToastService {
  constructor(private http: HttpClient, public snackBar: MatSnackBar) {

  }

  pushError(error: string) {
    this.snackBar.open(error, 'Hide', { duration: 10000 });
  }
}
