import { Component, AfterViewInit, Input, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { ChartService } from 'src/app/chart.service';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-ancestry-population-values',
    templateUrl: './population-values.component.html',
    styleUrls: ['./population-values.component.scss']
})

export class AncestryPopulationValuesComponent implements AfterViewInit {
    private subscription: Subscription;
    loadingData: boolean = false;
    loadedData: boolean = false;
    @Input() selected: boolean = false;

    constructor(
        private horseService: HorseService,
        private chartService: ChartService
    ) {

    }
    
    ngAfterViewInit() {
        
    }

    ngOnChanges() {
        if (this.selected && !this.loadedData) this.loadData();
    }

    loadData() {
        this.loadingData = true;
        this.subscription = this.horseService!.getPopulationData()
            .subscribe(
                data => {
                    this.loadingData = false;
                    this.loadedData = true;
                    this.chartService.drawPlotChart({
                        yAxisLabel: 'Ballou',
                        chartAreaElId: 'ancestry-population-chart-area',
                        chartElId: 'population-bal-chart',
                        data: data.filter(d => d.category == 'Ballou'),
                        chartHeight: '300',
                        boxWidth: 100
                    });

                    this.chartService.drawPlotChart({
                        yAxisLabel: 'AHC',
                        chartAreaElId: 'ancestry-population-chart-area',
                        chartElId: 'population-ahc-chart',
                        data: data.filter(d => d.category == 'AHC'),
                        chartHeight: '300',
                        boxWidth: 100
                    });

                    this.chartService.drawPlotChart({
                        yAxisLabel: 'Kalinowski',
                        chartAreaElId: 'ancestry-population-chart-area',
                        chartElId: 'population-kal-chart',
                        data: data.filter(d => d.category == 'Kalinowski'),
                        chartHeight: '300',
                        boxWidth: 100
                    });
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                });
    }
}
