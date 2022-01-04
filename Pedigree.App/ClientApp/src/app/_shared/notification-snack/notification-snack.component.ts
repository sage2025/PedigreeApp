import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_SNACK_BAR_DATA } from '@angular/material';
import { DialogData } from 'src/app/_models/diaog-data';

@Component({
  selector: 'app-notify-snack',
  templateUrl: './notification-snack.component.html',
  styleUrls: ['./notification-snack.component.css']
})
export class NotificationSnackComponent implements OnInit {
  messageToShow: string = "Oops something went wrong";
  showMessageColor: number = 1;
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: DialogData) {
    if (data.message != null) {
      this.messageToShow = data.message;
    }
    if (data.showAlert != null) {
      this.showMessageColor = data.showAlert;
    }
  }

  ngOnInit() {
  }

}
