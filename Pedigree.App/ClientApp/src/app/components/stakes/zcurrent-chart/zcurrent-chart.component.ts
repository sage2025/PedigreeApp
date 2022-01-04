import { Component, AfterViewInit, Input, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { ChartService } from 'src/app/chart.service';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-stakes-zcurrent-chart',
    templateUrl: './zcurrent-chart.component.html',
    styleUrls: ['./zcurrent-chart.component.scss']
})

export class StakesZCurrentChartComponent implements AfterViewInit {
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
        //setTimeout(() => this.loadData());
    }

    ngOnChanges() {
        if (this.selected && !this.loadedData) this.loadData();
    }

    loadData() {
        this.loadingData = true;
        this.subscription = this.horseService!.getZCurrentPlotData()
            .subscribe(
                data => {
                    this.loadingData = false;
                    this.loadedData = true;

                    let year = new Date().getFullYear();
                    while (year >= 1990) {
                        const startYear = parseInt(`${year / 10}`) * 10;
                        const endYear = parseInt(`${year / 10}`) * 10 + 9;

                        const filteredData = data.filter(d => d.title >= `${startYear}` && d.title <= `${endYear}`);
                        if (filteredData.length > 0) {
                            this.chartService.drawPlotChart({
                                xAxisLabel: `YOB with count(${startYear} - ${endYear})`,
                                yAxisLabel: 'ZCurrentBPR',
                                chartAreaElId: 'zcurrent-chart-area',
                                chartElId: `stakes-population-zcurrent-chart-${startYear}`,
                                data: filteredData,
                                boxWidth: 30,
                                xAxisExtraInfoShow: true,
                                chartHeight: '300'
                            });
                        }
                        year -= 10;
                    }
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                });
    }


}
