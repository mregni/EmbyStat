import { NgModule } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';

@NgModule({
  imports: [
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatCardModule,
    MatStepperModule,
    MatRippleModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatDialogModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTabsModule,
    MatTableModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatSortModule
  ],
  exports: [
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatCardModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatRippleModule,
    MatMenuModule,
    MatDialogModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTabsModule,
    MatTableModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatSortModule
  ],
})

export class MaterialModule { }
