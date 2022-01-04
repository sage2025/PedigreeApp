import { Component, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'add-haplotype-dialog',
  templateUrl: 'add-haplotype-dialog.html',
})
export class AddHaploTypeDialog {

  constructor(
    public dialogRef: MatDialogRef<AddHaploTypeDialog>,
    @Inject(MAT_DIALOG_DATA) public data: object) { }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onKeyDownEnter(value): void {
    this.dialogRef.close(value);
  }
}