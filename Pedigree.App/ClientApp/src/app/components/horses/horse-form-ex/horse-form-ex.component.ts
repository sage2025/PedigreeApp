import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { Validators } from "@angular/forms";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MatDialog, MatDialogRef, MatSelectionList, MatSelectionListChange, MAT_DIALOG_DATA } from "@angular/material";
import { first } from "rxjs/operators";
import { START_YEAR } from "src/app/constants";
import { DialogData } from "src/app/_models/diaog-data";
import { Horse } from "src/app/_models/horse.model";
import { HorseService } from "src/app/_services/horse.service";
import { RelationshipService } from "src/app/_services/relationship.service";
import { ConfirmComponent } from "src/app/_shared/confirm/confirm.component";

@Component({
    selector: 'app-horse-form-ex',
    templateUrl: './horse-form-ex.component.html',
    styleUrls: ['./horse-form-ex.component.scss']
})

export class HorseFormExComponent implements OnInit {
    horse: Horse;
    horses: Horse[] = [];
    isSelecting: boolean = false;
    isEditing: boolean = false;
    isSaving: boolean = false;
    horseForm: FormGroup;
    yearArray = [];
    sire: Horse;
    dam: Horse;
    sireWarning: string;
    damWarning: string;
    errorMessage: string;

    @ViewChild(MatSelectionList, { static: false }) selectionList: MatSelectionList;

    constructor(
        private formBuilder: FormBuilder,
        private horseService: HorseService,
        private relationshipService: RelationshipService,
        public dialogRef: MatDialogRef<HorseFormExComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: DialogData
    ) {
        this.horse = (<any>this.data).horse;
        this.horses = (<any>this.data).horses;
        this.isSelecting = this.horses.length > 0;
        this.isEditing = this.horses.length == 0;
    }

    ngOnInit() {
        this.loadYearsFrom();
        this.horseForm = this.formBuilder.group({
            name: [this.horse.name, Validators.required],
            sex: [this.horse.sex, Validators.required],
            age: [this.horse.age],
            country: [this.horse.country],
        });

        // Load sire
        this.horseService.searchHorseStartsWith(this.horse.fatherName).subscribe(data => {
            const availables = data.filter(d => this.horse.age - d.age <= 25);
            if (availables.length > 0) {
                this.sire = availables[0];
                if (availables.length > 1) {
                    this.sireWarning = 'There are one more sires with same name.';
                }
            } else {
                this.sireWarning = 'There is no sire matched with name.';
            }
        });

        // Load dam
        this.horseService.searchHorseStartsWith(this.horse.motherName).subscribe(data => {
            const availables = data.filter(d => this.horse.age - d.age <= 25);
            if (availables.length > 0) {
                this.dam = availables[0];
                if (availables.length > 1) {
                    this.damWarning = 'There are one more dams with same name.';
                }
            } else {
                this.damWarning = 'There is no dam matched with name.';
            }
        });
    }

    ngAfterViewInit() {
        if (this.selectionList) {
            this.selectionList.selectionChange.subscribe((s: MatSelectionListChange) => {
                this.selectionList.deselectAll();
                s.option.selected = true;
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

    onSelect(): void {
        if (this.selectionList.selectedOptions.selected.length == 1) {
            this.dialogRef.close(this.selectionList.selectedOptions.selected[0].value);
        }
    }

    onEdit(): void {
        this.isEditing = true;
        this.isSelecting = false;
    }

    selectParent(horse: Horse, parentType: String) {
        if (horse == null) return;

        if (this.horseForm.value.age - horse.age > 25) {
            const confirmDialogRef = this.dialog.open(ConfirmComponent, {
                width: '250px',
                data: { name: null, message: `${parentType}-Child relationship is greater than 25 years, is this correct?`, showAlert: 1 }
            });

            confirmDialogRef.afterClosed().subscribe(result => {
                if (!result) {
                    if (parentType == 'Father') {
                        this.sire = null;
                    }
                    else if (parentType == 'Mother') {
                        this.dam = null;
                    }
                }
            });
        }
    }

    onHorseSubmit() {
        if (this.horseForm.invalid) return;

        const horseData = this.horseForm.value as Horse;

        if (this.sire != undefined) {
            horseData.fatherOId = this.sire.oId;
            horseData.fatherName = this.sire.name;
        }

        if (this.dam != undefined) {
            horseData.motherOId = this.dam.oId;
            horseData.motherName = this.dam.name;

            this.relationshipService.getOffsprings(this.dam.id)
                .subscribe(result => {

                    if (result.horses.some(h => h.age == horseData.age)) {
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
        this.isSaving = true;
        this.horseService.addHorse(horseData)
            .pipe(first())
            .subscribe(
                data => {
                    this.isSaving = false;
                    this.dialogRef.close(data);
                },
                error => {
                    this.isSaving = false;
                    this.errorMessage = error.error;
                    console.error(error);
                }
            );
    }
}