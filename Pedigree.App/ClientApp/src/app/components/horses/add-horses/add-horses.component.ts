import { Component, OnInit } from '@angular/core';
import { MatDialog, MatSnackBar, MatTableDataSource } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';
import { AlertComponent } from 'src/app/_shared/alert/alert.component';
import { ConfirmComponent } from 'src/app/_shared/confirm/confirm.component';
import { NotificationSnackComponent } from 'src/app/_shared/notification-snack/notification-snack.component';
import { HorseFormExComponent } from '../horse-form-ex/horse-form-ex.component';

@Component({
  selector: 'app-add-horses',
  templateUrl: './add-horses.component.html',
  styleUrls: ['./add-horses.component.css']
})
export class AddHorsesComponent implements OnInit {
  displayColumns: string[] = ['no', 'name', 'age', 'sex', 'country', 'sire', 'dam', 'actions'];
  horseData = new MatTableDataSource<Horse>();
  loadingData = false;
  deletingHorse: object = {};

  constructor(
    public dialog: MatDialog,
    private horseService: HorseService,
    private _snackBar: MatSnackBar,
  ) { }

  ngOnInit() {

  }

  csvInputChange(fileInputEvent: any) {
    const me = this;
    const files = fileInputEvent.target.files;
    if (files.length > 0) {
      if (typeof (FileReader) != "undefined") {
        const reader = new FileReader();

        const data = [];
        me.loadingData = true;
        reader.onload = function (e) {
          const result = reader.result as string;
          const rows = result.split('\n');
          for (var i = 1; i < rows.length; i++) {
            if (rows[i]) {
              const cells = rows[i].split(',');
              if (cells.length == 6) {
                data.push({
                  name: cells[0].trim(),
                  age: parseInt(cells[1].trim()),
                  sex: cells[2].trim(),
                  country: cells[3].trim(),
                  fatherName: cells[4].trim(),
                  motherName: cells[5].trim()
                });
              }
            }
          }

          me.horseData.data = data;
          me.loadingData = false;
        }

        reader.readAsText(files[0]);

      } else {
        alert("This browser does not support HTML5.");
      }
    }
  }

  openDialog(horse: Horse, index: number) {
    this.loadingData = true;
    this.horseService.searchHorseStartsWith(horse.name).subscribe(data => {

      const dialogRef = this.dialog.open(HorseFormExComponent, {
        width: '650px',
        data: { horse: horse, horses: data }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.horseData.data[index] = result;
          this.horseData._updateChangeSubscription();
          this.checkAllSaves();
        }
      });
    },
      error => {

      }
    ).add(() => {
      this.loadingData = false;
    });
  }

  onDelete(horse: Horse, index: number) {
    if (horse.oId) {
      this.deletingHorse[index] = true;
      this.horseService.checkHorseLinkage(horse.oId).pipe(first())
        .subscribe(result => {
          if (result['hasRaceResult']) {
            this.dialog.open(AlertComponent, {
              width: '350px',
              data: { title: 'Warning', message: 'Cannot delete a horse attached to a race result. Please remove that result before deleting this horse.', showAlert: 1 }
            });
            this.deletingHorse[index] = false;
          } else {
            var message = "";
            var name = horse.name;
            if (result['hasChildren']) {
              // Children exists
              message = "This record has child records. Do you want to delete this record?";
              name = name + " (has children)";
            }
            else {
              message = "Are you sure you want to delete this horse?";
            }

            const deleteDialogRef = this.dialog.open(ConfirmComponent, {
              width: '350px',
              data: { name: name, message: message, showAlert: 1 }
            });

            deleteDialogRef.afterClosed().subscribe(result => {
              if (result) {
                this.horseService.deleteHorse(horse.id).pipe(first())
                  .subscribe(data => {
                    this.horseData.data.splice(index, 1);
                    this.horseData._updateChangeSubscription();
                    this.deletingHorse[index] = false;
                    this.checkAllSaves();
                  },
                    error => {
                      console.log(error);
                      this.deletingHorse[index] = false;
                    })
              } else {
                this.deletingHorse[index] = false;
              }
            });
          }
        },
          error => {
            this.deletingHorse[index] = false;
            console.log(error);
          });
    }
    else {
      this.horseData.data.splice(index, 1);
      this.horseData._updateChangeSubscription();
      this.checkAllSaves();
    }
  }

  checkAllSaves() {
    if (this.horseData.data.every(d => d.id != undefined)) {
      this._snackBar.openFromComponent(NotificationSnackComponent, {
        duration: 5000,
        data: { message: 'All records added' } // 2 stands for success 1 for error
      });
    }
  }
}
