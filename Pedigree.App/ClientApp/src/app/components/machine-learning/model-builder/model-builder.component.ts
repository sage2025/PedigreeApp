import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material';
import { ML_COLUMNS } from 'src/app/constants';
import { MLModel } from 'src/app/_models/ml-model.model';
import { MLService } from 'src/app/_services/ml.service';

@Component({
    selector: 'app-ml-model-builder',
    templateUrl: './model-builder.component.html',
    styleUrls: ['./model-builder.component.css']
})

export class MLModelBuilderComponent implements AfterViewInit {
    lastModel: MLModel;
    columns: object[] = ML_COLUMNS;
    categories: string[] = ML_COLUMNS.map((c: any) => c.category).filter((v, i, a) => a.indexOf(v) === i);
    selectedColumns: object = {};
    completed: boolean[] = [false, false, false];
    isNewTraining = false;
    isTraining = false;
    trainResult: MLModel;
    isDeploying = false;
    isEvaluating = false;

    currentEvaluatedValue: number;
    newEvaluatedValue: number;

    @ViewChild('stepper', { static: false }) private stepper: MatStepper;

    currentEvaluateForm: FormGroup;
    newEvaluateForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private mlService: MLService
    ) {
        this.columns.forEach((c: any) => {
            this.selectedColumns[c.name] = c.checked;
        });
    }

    ngAfterViewInit() {
        this.mlService.getLastModel()
            .subscribe(
                data => {
                    this.lastModel = data;
                    this.buildCurrentEvaluateFormGroup();
                },
                error => {
                    console.error(error);
                }
            )
    }

    columnsInCategory(category: string) {
        return this.columns.filter((c: any) => c.category == category);
    }

    toggleCategoryAllCheck(event: any, category: string) {
        this.columns.filter((c: any) => c.category == category).forEach((c: any) => this.selectedColumns[c.name] = event.checked);
    }

    onTrainModel() {
        this.isTraining = true;
        this.mlService.trainModel(this.selectedColumns)
            .subscribe(
                (data: any) => {
                    console.log(data)
                    this.isTraining = false;
                    this.trainResult = data;

                    this.completed[0] = true;
                    setTimeout(() => {
                        this.stepper.next();
                    }, 100);
                },
                error => {
                    this.isTraining = false;
                }
            );
    }

    onDiscardModel() {
        this.completed[0] = false;
        this.completed[1] = false;
        this.completed[2] = false;
        setTimeout(() => {
            this.stepper.reset();
        }, 100);
    }

    gotoEvaluate() {
        this.completed[1] = true;
        this.buildNewEvaluateFormGroup();
        setTimeout(() => {
            this.stepper.next();
        }, 100);
    }

    onDeployModel() {
        if (!this.trainResult) return;

        this.isDeploying = true;
        this.mlService.deployModel(this.trainResult.id)
            .subscribe(
                data => {
                    this.isDeploying = false;
                    this.lastModel = this.trainResult;
                    this.trainResult = null;
                    this.isNewTraining = false;
                    this.buildCurrentEvaluateFormGroup();
                },
                error => {
                    this.isDeploying = false;
                }
            );
    }

    currentModelFeatureColumns() {
        return this.lastModel.features.split(", ");
    }

    buildCurrentEvaluateFormGroup() {
        const groupOptions = {};
        this.currentModelFeatureColumns().forEach(c => groupOptions[c] = []);
        this.currentEvaluateForm = this.formBuilder.group(groupOptions);
    }

    onEvaluateCurrentModel() {
        if (!this.lastModel || !this.currentEvaluateForm) return;
        
        const data = this.currentEvaluateForm.value;

        this.isEvaluating = true;
        this.mlService.evaluateModel(this.lastModel.id, data)
            .subscribe(
                (data: any) => {
                    this.isEvaluating = false;
                    this.currentEvaluatedValue = data;
                },
                error => {
                    this.isEvaluating = false;
                }
            );
    }

    newModelFeatureColumns() {
        return this.trainResult.features.split(", ");
    }

    buildNewEvaluateFormGroup() {
        const groupOptions = {};
        this.newModelFeatureColumns().forEach(c => groupOptions[c] = []);
        this.newEvaluateForm = this.formBuilder.group(groupOptions);
    }

    onEvaluateNewModel() {
        if (!this.trainResult || !this.newEvaluateForm) return;
        
        const data = this.newEvaluateForm.value;

        this.isEvaluating = true;
        this.mlService.evaluateModel(this.trainResult.id, data)
            .subscribe(
                (data: any) => {
                    this.isEvaluating = false;
                    this.newEvaluatedValue = data;
                },
                error => {
                    this.isEvaluating = false;
                }
            );
    }

    gotoNewTrain() {
        this.isNewTraining = true;
    }
    cancelNewTrain() {
        this.isNewTraining = false;
    }
}
