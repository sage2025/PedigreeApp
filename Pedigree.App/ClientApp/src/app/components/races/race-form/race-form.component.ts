import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material';
import { Race } from '../../../_models/race.model';
import { RACE_COUNTRIES, RACE_DISTANCES, RACE_SURFACES, RACE_TYPES, RACE_STATUSES } from '../../../constants';
import { RaceService } from '../../../_services/race.service';
import { NotificationSnackComponent } from '../../../_shared/notification-snack/notification-snack.component';
import { toISOString } from 'src/app/helpers';

@Component({
    selector: 'app-race-form',
    templateUrl: './race-form.component.html',
    styleUrls: ['./race-form.component.scss']
})
export class RaceFormComponent implements OnInit {
    @Input() raceId: number;
    raceForm: FormGroup;
    currentRace: Race = new Race();
    countries: string[];
    distances: object[];
    surfaces: string[];
    types: string[];
    statuses: string[];
    loading: boolean = false;
    submitting: boolean = false;
    error: string = '';

    constructor(
        private formBuilder: FormBuilder,
        private router: Router,
        private raceService: RaceService,
        private _snackBar: MatSnackBar,
    ) {

    }

    ngOnInit() {
        this.countries = RACE_COUNTRIES;
        this.surfaces = RACE_SURFACES;
        this.distances = RACE_DISTANCES;
        this.types = RACE_TYPES;
        this.statuses = RACE_STATUSES;

        this.loadData();
    }
    populateForm(result: Race) {
        this.raceForm = this.formBuilder.group({
            name: [result.name, Validators.required],
            date: [result.date],
            country: [result.country],
            distance: [result.distance],
            surface: [result.surface],
            type: [result.type],
            status: [result.status],
        });
    }
    loadData() {
        this.currentRace = new Race();
        this.populateForm(this.currentRace);

        if (this.raceId > 0) {
            this.loading = true;
            this.raceService.getRace(this.raceId).pipe(first())
                .subscribe(result => {
                    this.currentRace = result;
                    this.populateForm(this.currentRace);
                    this.loading = false;
                });
        }
    }
    onRaceSubmit() {
        if (this.raceForm.invalid) {
            return;
        }

        this.submitting = true;

        var raceData = this.raceForm.value as Race; 
        raceData.date = toISOString(new Date(raceData.date));
        if (this.raceId > 0) {
            raceData.id = this.raceId;
            this.raceService.updateRace(raceData)
              .subscribe(
                data => {
                    this.submitting = false;
                    this.openSnackBar("Updated successfully!", 2);
                },
                error => {
                  this.error = error;
                  this.submitting = false;
                  this.openSnackBar(error.error, 1);
                  console.error(error);
                });
        } else {
            this.raceService.addRace(raceData)
                .pipe(first())
                .subscribe(
                    id => {
                        this.submitting = false;
                        this.openSnackBar("Added successfully!", 2);
                        // Navitage to edit form
                        this.router.navigate([`races/${id}`]);
                    },
                    error => {
                        this.error = error;
                        this.submitting = false;
                        this.openSnackBar(error.error, 1);
                        console.error(error);
                    });
        }
    }

    openSnackBar(message: string, showSuccess: number = 1) {
        this._snackBar.openFromComponent(NotificationSnackComponent, {
            duration: 5000,
            data: { message: message, showAlert: showSuccess } // 2 stands for success 1 for error
        });
    }
}
