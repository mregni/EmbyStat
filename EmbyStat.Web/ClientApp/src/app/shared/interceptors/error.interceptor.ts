import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { ToastService } from '../services/toast.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private readonly injector: Injector) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(catchError((err, caught) => {
      const toaster = this.injector.get(ToastService);

      if (err.status === 500) {
        const error = JSON.parse(err.error);

        if (error.IsError) {
          toaster.showError(`EXCEPTIONS.${error.Message}`);
        } else {
          toaster.showError('EXCEPTIONS.UNHANDLED');
        }
      }

      if (err.status === 404) {
        toaster.showError('EXCEPTIONS.NOT_FOUND');
      }

      return throwError(err);
    }) as any);
  }
}
