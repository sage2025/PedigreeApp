import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { Horse } from '../../../_models/horse.model';
import { HorseService } from '../../../_services/horse.service';

@Component({
    selector: 'app-twin-view',
    templateUrl: './twin-view.component.html',
    styleUrls: ['./twin-view.component.scss']
})

export class TwinViewComponent implements AfterViewInit {
    displayedColumns: string[] = ['no', 'pedigree', 'name', 'age', 'country', 'fatherName', 'motherName', 'twinYear', 'twins'];
    private subscription: Subscription;
    dataSource = new MatTableDataSource<Horse>();
    loadingData: boolean = false;
    resultsLength = 0;
    
    
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private horseService: HorseService
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        
        setTimeout(() => {
            this.loadData();
        })
    }

    loadData() {        
        this.loadingData = true;
        this.subscription = this.horseService!.getTwinDams()
            .subscribe(
                data => {
                    this.dataSource.data = data;
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
    }
}
