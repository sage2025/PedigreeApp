import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { FormControl } from '@angular/forms';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap, debounceTime, tap, finalize } from 'rxjs/operators';
import { Horse } from '../../../_models/horse.model';
import { RelationshipService } from '../../../_services/relationship.service';
import { HorseService } from '../../../_services/horse.service';

@Component({
    selector: 'app-inbreeding-list',
    templateUrl: './inbreeding-list.component.html',
    styleUrls: ['./inbreeding-list.component.scss']
})

export class InbreedingListComponent implements AfterViewInit {
    displayedColumns: string[] = ['no', 'pedigree', 'name', 'breeding', 'inbreed_to', 'inbreeding'];
    private subscription: Subscription;
    currentHorse: Horse;
    dataSource = new MatTableDataSource<Horse>();
    loading: boolean = false;
    loadingHorses: boolean = false;
    resultsLength = 0;
    searchHorsesCtrl = new FormControl();
    filteredHorses: Horse[];

    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;
    constructor(
        private relationshipService: RelationshipService,
        private horseService: HorseService
    ) {

    }
    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
        this.horseSearchSetup();
    }

    horseSearchSetup() {
        this.searchHorsesCtrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.loadingHorses = true;
                    this.filteredHorses = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.loadingHorses = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null) {
                        this.filteredHorses = [];
                    }
                    else {
                        this.filteredHorses = data;
                    }
                },
                error => {
                    console.log(error);
                }
            );
    }

    loadData() {
        if (!this.currentHorse) return;

        this.subscription = merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    this.loading = true;
                    return this.relationshipService!.getInbreedings(this.currentHorse.oId, this.sort.active, this.sort.direction, this.paginator.pageIndex);
                }),
                map(data => {
                    this.loading = false;
                    this.resultsLength = data.total;
                    return data.horses;
                }),
                catchError((error) => {
                    console.log(error);
                    this.loading = false;
                    return observableOf([]);
                })
            ).subscribe(data => this.dataSource.data = data);
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }

    selectHorse(horse: Horse) {
        this.currentHorse = horse;
    }

    searchItems() {
        if (this.subscription) this.subscription.unsubscribe();
        this.loadData();
    }
}
