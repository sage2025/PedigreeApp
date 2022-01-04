import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { START_YEAR } from 'src/app/constants';
import { Horse } from '../../../_models/horse.model';
import { HorseService } from '../../../_services/horse.service';

@Component({
    selector: 'app-ten-generations',
    templateUrl: './ten-generations.component.html',
    styleUrls: ['./ten-generations.component.scss']
})

export class TenGenerationsComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'country', 'fatherName', 'motherName', 'family'];
    private subscription: Subscription;
    dataSource = new MatTableDataSource<Horse>();
    loadingData: boolean = false;
    resultsLength = 0;
    yearArray = [];
    selectedYear = new Date().getFullYear();
    
    
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private horseService: HorseService
    ) {

    }
    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
        setTimeout(() => {
            this.loadYearsFrom();
        })
    }

    loadYearsFrom() {
      var d = new Date();
      var tempArray = [];
      for (var i = START_YEAR; i <= d.getFullYear(); i++) {
        tempArray.push(i);
      }
      this.yearArray = tempArray.reverse();
    }

    loadData() {        
        this.loadingData = true;
        this.subscription = merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    return this.horseService!.getIncompletedPedigreeHorses(this.selectedYear, this.sort.active, this.sort.direction, this.paginator.pageIndex);
                }),
                map(data => {
                    // Flip flag to show that loading has finished.
                    this.loadingData = false;
                    this.resultsLength = data.total;
                    return data.horses;
                }),
                catchError((error) => {
                    console.log(error);
                    this.loadingData = false;
                    this.resultsLength = 0;
                    return observableOf([]);
                })
            ).subscribe(data => {
                this.dataSource.data = data;
            });
    }

    selectYear(event) {
        if (!event.source.selected) return;
        this.selectedYear = event.source.value;
        if (this.subscription) this.subscription.unsubscribe();
        this.loadData();
    }
}
