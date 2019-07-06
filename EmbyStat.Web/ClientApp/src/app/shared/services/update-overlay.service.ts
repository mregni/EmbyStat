import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';

//import { UpdateOverlayComponent } from '../components/update-overlay/update-overlay.component';

@Injectable()
export class UpdateOverlayService {
  private overlayRef: OverlayRef = null;

  constructor(private overlay: Overlay) { }

  public show(state: boolean) {
    if (state) {
      if (!this.overlayRef) {
        this.overlayRef = this.overlay.create();
      }

      // const spinnerOverlayPortal = new ComponentPortal(UpdateOverlayComponent);
      // const component = this.overlayRef.attach(spinnerOverlayPortal);
    } else {
      if (!!this.overlayRef) {
        this.overlayRef.detach();
      }
    }
  }
}
