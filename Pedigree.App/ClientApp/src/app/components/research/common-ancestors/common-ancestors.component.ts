import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { FormControl } from '@angular/forms';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap, debounceTime, tap, finalize } from 'rxjs/operators';
import { Horse } from '../../../_models/horse.model';
import { RelationshipService } from '../../../_services/relationship.service';
import { HorseService } from '../../../_services/horse.service';

@Component({
    selector: 'app-common-ancestors',
    templateUrl: './common-ancestors.component.html',
    styleUrls: ['./common-ancestors.component.scss']
})

export class CommonAncestorsComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'country', 'father', 'mother', 'family'];
    private subscription: Subscription;
    currentHorse1: Horse;
    currentHorse2: Horse;
    dataSource = new MatTableDataSource<Horse>();
    loadingData: boolean = false;
    loadingHorses: boolean = false;
    resultsLength = 0;
    searchHorsesCtrl = new FormControl();
    filteredHorses: Horse[];

    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    constructor(
        private relationshipService: RelationshipService,
        private horseService: HorseService
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
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
        if (!this.currentHorse1 || !this.currentHorse2) return;

        this.loadingData = true;
        this.subscription = merge(this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    return this.horseService!.getCommonAncestors(this.currentHorse1.id, this.currentHorse2.id);
                }),
                map(data => {
                    // Flip flag to show that loading has finished.
                    this.loadingData = false;
                    this.resultsLength = data.horses.length;
                    return data.horses;
                }),
                catchError((error) => {
                    console.log(error);
                    this.loadingData = false;
                    return observableOf([]);
                })
            ).subscribe(data => {
                this.dataSource.data = data;
            });
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }
    
    selectHorse1(horse: Horse) {
        this.currentHorse1 = horse;
    }
    selectHorse2(horse: Horse) {
        this.currentHorse2 = horse;
    }

    searchItems() {
      if (this.subscription) this.subscription.unsubscribe();
      this.loadData();
    }
}
