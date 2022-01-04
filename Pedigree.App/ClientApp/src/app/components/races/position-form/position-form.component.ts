import { Component, Input, Optional, Host, Output, EventEmitter } from '@angular/core';
import { SatPopover } from '@ncstate/sat-popover';
import { filter, first, debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { Horse } from '../../../_models/horse.model';
import { Race } from '../../../_models/race.model';
import { Position } from '../../../_models/position.model';
import { HorseService } from '../../../_services/horse.service';
import { PositionService } from '../../../_services/position.service';
import { AlertComponent } from '../../../_shared/alert/alert.component';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';

@Component({
    selector: 'app-position-form',
    styleUrls: ['./position-form.component.scss'],
    templateUrl: './position-form.component.html'
})

export class PositionFormComponent {

    /** Overrides the comment and provides a reset value when changes are cancelled. */
    @Input() position: Position;
    @Input() race: Race;
    @Output() submitted = new EventEmitter<object>();

    positionForm: FormGroup;
    searchHorsesCtrl = new FormControl();
    filteredHorses: Horse[];
    isLoadingHorses: boolean = false;
    isSubmitting: boolean = false;
    errorMsg: string = '';


    constructor(@Optional() @Host() public popover: SatPopover,
        private formBuilder: FormBuilder,
        private horseService: HorseService,
        private positionService: PositionService,
        public dialog: MatDialog,
    ) {

    }

    ngOnInit() {
        // subscribe to cancellations and reset form value
        if (this.popover) {
            this.popover.closed.pipe(filter(val => val == null))
                .subscribe(() => {

                });
        }

        this.horseSearchSetup();
        this.positionForm = this.formBuilder.group({
            place: this.position ? this.position.place : null,            
        })
        if (this.position) {
            this.searchHorsesCtrl.setValue({
                oId: this.position.horseOId,
                name: this.position.horseName
            })
        }
    }

    horseSearchSetup() {
        this.searchHorsesCtrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingHorses = true;
                    this.filteredHorses = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingHorses = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null) {
                        this.errorMsg = "Oops, try again.";
                        this.filteredHorses = [];
                    }
                    else {
                        this.errorMsg = "";
                        this.filteredHorses = data;
                    }
                },
                error => {
                    console.log(error);
                }
            );
    }

    selectHorse(horse: Horse) {

        if (this.race.type == '2YO Fillies' || this.race.type == '3YO Fillies' || this.race.type == '3YO + F&M' || this.race.type == '4YO+ Mares only') {
            if (horse.sex == 'Male') {
                const dialogRef = this.dialog.open(AlertComponent, {
                    width: '350px',
                    data: { title: 'Alert', message: 'Cannot attach a male in a female only race', showAlert: 1 }
                });

                dialogRef.afterClosed().subscribe(result => {
                    this.searchHorsesCtrl.setValue(null);
                });
                return;
            }
        }

        if (new Date(this.race.date).getFullYear() - horse.age > 10) {
            const dialogRef = this.dialog.open(ConfirmComponent, {
                width: '350px',
                data: { name: null, message: `Horse is 10 years older than racedate, is this the correct horse?`, showAlert: 1 }
            });

            dialogRef.afterClosed().subscribe(result => {
                if (!result) {
                    this.searchHorsesCtrl.setValue(null);
                }
            });
        }
    }

    onPositionSubmit() {
        if (!this.positionForm.value.place || !this.searchHorsesCtrl.value || this.searchHorsesCtrl.value == "") {
            console.log('invalid')
            return;
        }

        this.isSubmitting = true;
        if (this.position != undefined) {
            this.position.place = this.positionForm.value.place;
            this.position.horseOId = this.searchHorsesCtrl.value.oId;
            this.positionService.updatePosition(this.position)
            .pipe(first())
            .subscribe(
                res => {
                    this.isSubmitting = false;
                    this.positionForm.reset();
                    this.searchHorsesCtrl.setValue(null);

                    this.submitted.emit({message: "Updated successfully"});
                    if (this.popover) {
                        this.popover.close();
                    }
                },
                error => {
                    this.isSubmitting = false;
                    this.errorMsg = error; console.error('>>>>>>>>>>', error);
                    this.submitted.emit({error: error.error});
                    console.log(error);
                });
        } else {
            const position = new Position();
            position.raceId = this.race.id;
            position.place = this.positionForm.value.place;
            position.horseOId = this.searchHorsesCtrl.value.oId;
            this.positionService.addPosition(position)
            .pipe(first())
            .subscribe(
                res => {
                    this.isSubmitting = false;
                    this.positionForm.reset();
                    this.searchHorsesCtrl.setValue(null);

                    this.submitted.emit({message: "Added successfully"});
                    if (this.popover) {
                        this.popover.close();
                    }
                },
                error => {
                    this.isSubmitting = false;
                    this.errorMsg = error;
                    this.submitted.emit({error: error.error});
                    console.log(error);
                });
        }
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }
}