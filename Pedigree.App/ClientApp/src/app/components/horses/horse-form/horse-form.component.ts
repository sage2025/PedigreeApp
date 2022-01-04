import { Component, OnInit, Input, Inject, ViewChild, EventEmitter, Output } from '@angular/core';
import { Horse } from '../../../_models/horse.model';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HorseService } from '../../../_services/horse.service';
import { HorseWithParentChildren } from '../../../_models/horse-with-parent-children.model';
import { first, debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef, MatSnackBar, MatSelect, MatAutocomplete } from '@angular/material';
import { RelationshipService } from '../../../_services/relationship.service';
import { AlertComponent } from '../../../_shared/alert/alert.component';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';
import { NotificationSnackComponent } from '../../../_shared/notification-snack/notification-snack.component';
import { Relationship } from '../../../_models/relationship.model';
import { START_YEAR } from 'src/app/constants';

@Component({
  selector: 'app-horse-form',
  templateUrl: './horse-form.component.html',
  styleUrls: ['./horse-form.component.css']
})
export class HorseFormComponent implements OnInit {
  panelOpenState = false;
  @Input() horseId: number;
  @Output() horseChanged = new EventEmitter<object>();
  horseForm: FormGroup;
  currentHorse: HorseWithParentChildren = new HorseWithParentChildren();
  loading = false;
  isLoadingResults = false;
  submitted = false;
  returnUrl: string;
  error = '';
  yearArray = [];
  yob: number;
  isEdit: boolean = false;
  searchSiresCtrl = new FormControl();
  searchDamsCtrl = new FormControl();
  filteredSires: Horse[];
  filteredDams: Horse[];
  isLoading = false;
  errorMsg: string;
  showProgency: boolean = true;
  displayedColumns: string[] = ['name', 'age', 'sex', 'country', 'family', 'action'];

  constructor(private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private horseService: HorseService,
    private relationshipService: RelationshipService,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar) {
  }

  ngOnInit() {
    this.loadHorseData();
    this.loadYearsFrom();
    this.horseSearchSetup();
  }

  loadHorseData() {
    this.currentHorse.mainHorse = new Horse();
    this.populateForm(this.currentHorse.mainHorse);
    if (this.horseId > 0) {
      this.isEdit = true;
      this.isLoadingResults = true;
      this.horseService.getHorseWithPC(this.horseId).pipe(first())
        .subscribe(result => {
          this.currentHorse.mainHorse = result.mainHorse;
          this.currentHorse.children = result.children;
          this.currentHorse.parents = result.parents;
          this.populateForm(this.currentHorse.mainHorse);
          this.isLoadingResults = false;
          this.searchSiresCtrl.setValue(result.parents.find(h => h.sex == 'Male'));
          this.selectParent(result.parents.find(h => h.sex == 'Male'), 'Father');
          this.searchDamsCtrl.setValue(result.parents.find(h => h.sex == 'Female'));
          this.selectParent(result.parents.find(h => h.sex == 'Female'), 'Mother');
        });
    } else {
      this.isLoadingResults = false;
      this.showProgency = false;
    }
  }

  horseSearchSetup() {
    this.searchSiresCtrl.valueChanges
      .pipe(
        debounceTime(500),
        tap((value) => {
          this.errorMsg = "";
          this.isLoading = true;
          this.filteredSires = [];
        }),
        switchMap(value => this.horseService.searchHorseStartsWith(value)
          .pipe(
            finalize(() => {
              this.isLoading = false;
            })
          )
        )
      ).subscribe(
        data => {
          if (data == undefined || data == null) {
            this.errorMsg = "Oops, try again.";
            this.filteredSires = [];
          }
          else {
            this.errorMsg = "";
            this.filteredSires = data.filter(h => h.sex == 'Male' && h.age < this.horseForm.value.age);
          }
        },
        error => {
          console.log(error);
        }
      );

    this.searchDamsCtrl.valueChanges
      .pipe(
        debounceTime(500),
        tap(() => {
          this.errorMsg = "";
          this.isLoading = true;
          this.filteredDams = [];
        }),
        switchMap(value => this.horseService.searchHorseStartsWith(value)
          .pipe(
            finalize(() => {
              this.isLoading = false;
            })
          )
        )
      ).subscribe(
        data => {
          if (data == undefined || data == null) {
            this.errorMsg = "Oops, try again.";
            this.filteredDams = [];
          }
          else {
            this.errorMsg = "";
            this.filteredDams = data.filter(h => h.sex == 'Female' && h.age < this.horseForm.value.age);
          }
        },
        error => {
          console.log(error);
        }
      );
  }

  displayHorse(horse: Horse): string {
    return horse && horse.name ? horse.name : '';
  }

  populateForm(result: Horse) {
    this.horseForm = this.formBuilder.group({
      name: [result.name, Validators.required],
      sex: [result.sex, Validators.required],
      age: [result.age],
      //active: [result.active],
      //elite: [result.elite],
      family: [result.family],
      country: [result.country],
      mtDNATitle: [result.mtDNATitle],
    });
    this.yob = result.age;
  }

  selectParent(horse: Horse, parentType: String) {
    if (horse == null) return;

    if (this.currentHorse.mainHorse.isFounder) {
      this.dialog.open(AlertComponent, {
        width: '450px',
        data: { title: 'Warning', message: 'This horse is a founder. Please deselect Founder checkbox before adding parents', showAlert: 1 }
      });
      return;
    }

    if (this.yob - horse.age > 25) {
      const confirmDialogRef = this.dialog.open(ConfirmComponent, {
        width: '250px',
        data: { name: null, message: `${parentType}-Child relationship is greater than 25 years, is this correct?`, showAlert: 1 }
      });

      confirmDialogRef.afterClosed().subscribe(result => {
        if (!result) {
          if (parentType == 'Father') {
            this.searchSiresCtrl.setValue(null);
          }
          else if (parentType == 'Mother') {
            this.searchDamsCtrl.setValue(null);
          }
        }
      });
    }
  }

  removeChildRelationship(cOId) {
    const confirmDialogRef = this.dialog.open(ConfirmComponent, {
      width: '450px',
      data: { name: "Relationship", message: "The relationship will be delete between the horses!", showAlert: 1 }
    });
    confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Delete Relationship
        this.relationshipService.deleteMappging(cOId, this.currentHorse.mainHorse.oId).pipe(first())
          .subscribe(data => {

            var itemIndex = this.currentHorse.children.findIndex(p => p.oId == cOId);
            this.currentHorse.children.splice(itemIndex, 1);
            this.openSnackBar("Removed. Try refresh page if not reflected.", 2);

          },
            error => {
              // Show this error in some popup or somewhere
              this.openSnackBar(error.error);
              console.log(error);
            }
          );
      }
    });
  }

  private confirmParentalRelationship(selectedhorse: Horse) {
    var newRelationship = new Relationship();
    newRelationship.horseOId = this.currentHorse.mainHorse.oId; // The main horse here is the child
    newRelationship.parentOId = selectedhorse.oId; // Select horse is the parent
    newRelationship.parentType = (selectedhorse.sex == "Male") ? "Father" : "Mother"; // depending on selected horse gender father or mother
    this.relationshipService.addRelationship(newRelationship).pipe(first())
      .subscribe(data => {
        this.currentHorse.parents.push(selectedhorse);
        this.filteredSires = [];
        this.openSnackBar("Parent added successfully!", 2);
        this.loadHorseData();
      },
        error => {
          this.openSnackBar("Something went wrong!");
        });
  }

  public removeParentalRelationship(parentType: string) {
    const confirmDialogRef = this.dialog.open(ConfirmComponent, {
      width: '450px',
      data: { name: "Relationship", message: "The relationship will be delete between the horses!", showAlert: 1 }
    });
    confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Delete Relationship
        this.relationshipService.deleteRelationship(this.currentHorse.mainHorse.oId, parentType).pipe(first())
          .subscribe(data => {
            const i = this.currentHorse.parents.findIndex(h => h.sex == (parentType == 'Father' ? 'Male' : 'Female'));
            if (i > -1) this.currentHorse.parents.splice(i, 1);
            this.filteredSires = [];
            this.openSnackBar("Parent removed successfully!", 2);
            this.loadHorseData();
          },
            error => {
              this.openSnackBar("Something went wrong!");
            });
      }
    });
  }

  havingSire() {
    return typeof this.searchSiresCtrl.value == 'object' && this.searchSiresCtrl.value !== null;
  }

  havingDam() {
    return typeof this.searchDamsCtrl.value == 'object' && this.searchDamsCtrl.value !== null;
  }

  onHorseSubmit() {
    // stop here if form is invalid
    if (this.horseForm.invalid) {
      return;
    }

    this.loading = true;

    const horseData = this.horseForm.value as Horse;

    if (this.havingSire()) {
      horseData.fatherOId = this.searchSiresCtrl.value.oId;
    }

    if (this.havingDam()) {
      horseData.motherOId = this.searchDamsCtrl.value.oId;

      this.relationshipService.getOffsprings(this.searchDamsCtrl.value.id)
        .subscribe(result => {

          if (result.horses.some(h => h.age == horseData.age && h.id != this.horseId)) {
            const confirmDialogRef = this.dialog.open(ConfirmComponent, {
              width: '450px',
              data: { name: null, message: `You are adding a twin to this mare, are you sure?`, showAlert: 1 }
            });

            confirmDialogRef.afterClosed().subscribe(result => {
              if (result) {
                this.submitHorseData(horseData);
              }
            });
          } else {
            this.submitHorseData(horseData);
          }
        });

      return;
    }

    this.submitHorseData(horseData);
  }

  submitHorseData(horseData) {
    if (this.isEdit) {
      horseData.id = this.horseId;
      this.horseService.updateHorse(horseData)
        .subscribe(
          data => {
            this.openSnackBar("Updated successfully!", 2);
          },
          error => {
            this.error = error;
            this.loading = false;
            console.error(error);
          });
    } else {
      this.horseService.addHorse(horseData)
        .pipe(first())
        .subscribe((data: any) => {
          this.openSnackBar("Added successfully!", 2);
          // Navitage to edit form
          this.router.navigate([`horses/${data.id}`]);
        },
          error => {
            this.error = error;
            this.loading = false;
            this.openSnackBar(error.error, 1);
            console.error(error);
          });
    }
  }

  loadYearsFrom() {
    var d = new Date();
    var tempArray = [];
    for (var i = START_YEAR; i <= d.getFullYear(); i++) {
      tempArray.push(i);
    }
    this.yearArray = tempArray.reverse();
  }

  reloadForThisHorse(id: number) {
    this.horseId = id;

    this.router.navigate([`horses/${id}`]);
    this.loadHorseData();

    this.horseChanged.emit({horseId: id});
  }

  openSnackBar(message: string, showSuccess: number = 1) {
    this._snackBar.openFromComponent(NotificationSnackComponent, {
      duration: 5000,
      data: { message: message, showAlert: showSuccess } // 2 stands for success 1 for error
    });
  }
}
