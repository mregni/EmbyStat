import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { SpinnerOverlayComponent } from '../components/spinner-overlay/spinner-overlay.component';

@Injectable()
export class SpinnerOverlayService {
  private overlayRef: OverlayRef = null;

  constructor(private overlay: Overlay) { }

  public show() {
    if (!this.overlayRef) {
      this.overlayRef = this.overlay.create();
    }

    const spinnerOverlayPortal = new ComponentPortal(SpinnerOverlayComponent);
    const component = this.overlayRef.attach(spinnerOverlayPortal); 
  }

  public hide() {
    if (!!this.overlayRef) {
      this.overlayRef.detach();
    }
  }
}
