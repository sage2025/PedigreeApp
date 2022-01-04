import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material';
import { Weight } from '../../../_models/weight.model';
import { RACE_COUNTRIES, RACE_DISTANCES, RACE_SURFACES, RACE_TYPES, RACE_STATUSES } from '../../../constants';
import { WeightService } from '../../../_services/weight.service';
import { NotificationSnackComponent } from '../../../_shared/notification-snack/notification-snack.component';

@Component({
    selector: 'app-weight-form',
    templateUrl: './weight-form.component.html',
    styleUrls: ['./weight-form.component.scss']
})
export class WeightFormComponent implements OnInit {
    @Input() weightId: number;
    weightForm: FormGroup;
    currentWeight: Weight = new Weight();
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
        private weightService: WeightService,
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

    populateForm(result: Weight) {
        this.weightForm = this.formBuilder.group({
            country: [result.country],
            distance: [result.distance],
            surface: [result.surface],
            type: [result.type],
            status: [result.status],
            value: [result.value],
        });
    }
    
    loadData() {
        this.currentWeight = new Weight();
        this.populateForm(this.currentWeight);

        if (this.weightId > 0) {
            this.loading = true;
            this.weightService.getWeight(this.weightId).pipe(first())
                .subscribe(result => {
                    this.currentWeight = result;
                    this.populateForm(this.currentWeight);
                    this.loading = false;
                });
        }
    }
    onWeightSubmit() {
        console.log('>>>>>>>>>> onWeightSubmit')
        if (this.weightForm.invalid) {
            return;
        }

        this.submitting = true;

        if (this.weightId > 0) {
            var weightData = this.weightForm.value as Weight; 
            weightData.id = this.weightId;
            this.weightService.updateWeight(weightData)
              .subscribe(
                data => {
                    this.submitting = false;
                    this.openSnackBar("Updated successfully!", 2);
                },
                error => {
                  this.error = error;
                  this.submitting = false;
                  console.error(error);
                });
        } else {
            this.weightService.addWeight(this.weightForm.value)
                .pipe(first())
                .subscribe(
                    id => {
                        this.submitting = false;
                        this.openSnackBar("Added successfully!", 2);
                        // Navitage to edit form
                        this.router.navigate([`weights/${id}`]);
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
