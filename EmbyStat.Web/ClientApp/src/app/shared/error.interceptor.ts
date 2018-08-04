import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ToastService } from './services/toast.service';

import 'rxjs/add/operator/do';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private _injector: Injector) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).do(event => { }, err => {
      const toaster = this._injector.get(ToastService);
      const error = JSON.parse(err.error);

      if (err.status === 500) {
        if (error.IsError) {
          toaster.pushError('EXCEPTIONS.' + error.Message);
        } else {
          toaster.pushError('EXCEPTIONS.UNHANDLED');
        }
      }

      if (err.status === 404) {
        toaster.pushError('EXCEPTIONS.NOT_FOUND');
      }
    });
  }
}
