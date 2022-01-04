import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatTableDataSource, MatSort } from '@angular/material';
import { Subscription } from 'rxjs';
import { debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-research-wildcard-search',
    templateUrl: './wildcard-search.component.html',
    styleUrls: ['./wildcard-search.component.css']
})

export class WildcardSearchComponent implements AfterViewInit {

    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'motherName', 'bestRaceClass', 'mtDNATitle'];
    horseData = new MatTableDataSource<Horse>();

    private subscription: Subscription;
    loading = false;
    showSearchClear: boolean = false;

    searchTypeCtrl = new FormControl('wildcard1');

    searchHorse1Ctrl = new FormControl();
    searchHorse2Ctrl = new FormControl();
    searchHorse3Ctrl = new FormControl();
    searchHorse4Ctrl = new FormControl();

    isLoadingHorse1: boolean = false;
    errorLoadingHorse1: string = null;
    filteredHorses1: Horse[];

    isLoadingHorse2: boolean = false;
    errorLoadingHorse2: string = null;
    filteredHorses2: Horse[];

    isLoadingHorse3: boolean = false;
    errorLoadingHorse3: string = null;
    filteredHorses3: Horse[];

    isLoadingHorse4: boolean = false;
    errorLoadingHorse4: string = null;
    filteredHorses4: Horse[];

    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(
        private horseService: HorseService
    ) {

    }
    ngAfterViewInit() {
        this.horseData.sort = this.sort;
        this.horseSearchSetup();
    }

    horseSearchSetup() {
        this.searchHorse1Ctrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingHorse1 = true;
                    this.filteredHorses1 = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingHorse1 = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingHorse1 = 'No matches';
                        this.filteredHorses1 = [];
                    }
                    else {
                        this.errorLoadingHorse1 = null;
                        this.filteredHorses1 = data;
                    }
                },
                error => {
                    this.errorLoadingHorse1 = error;
                    console.log(error);
                }
            );

        this.searchHorse2Ctrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingHorse2 = true;
                    this.filteredHorses2 = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingHorse2 = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingHorse2 = 'No matches';
                        this.filteredHorses2 = [];
                    }
                    else {
                        this.errorLoadingHorse2 = null;
                        this.filteredHorses2 = data;
                    }
                },
                error => {
                    this.errorLoadingHorse2 = error;
                    console.log(error);
                }
            );

        this.searchHorse3Ctrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingHorse3 = true;
                    this.filteredHorses3 = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingHorse3 = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingHorse3 = 'No matches';
                        this.filteredHorses3 = [];
                    }
                    else {
                        this.errorLoadingHorse3 = null;
                        this.filteredHorses3 = data;
                    }
                },
                error => {
                    this.errorLoadingHorse3 = error;
                    console.log(error);
                }
            );

        this.searchHorse4Ctrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingHorse4 = true;
                    this.filteredHorses4 = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingHorse4 = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingHorse4 = 'No matches';
                        this.filteredHorses4 = [];
                    }
                    else {
                        this.errorLoadingHorse4 = null;
                        this.filteredHorses4 = data;
                    }
                },
                error => {
                    this.errorLoadingHorse4 = error;
                    console.log(error);
                }
            );
    }

    enableSearch() {
        return (
            (this.searchTypeCtrl.value == 'wildcard1' && (this.searchHorse1Ctrl.value != null && this.searchHorse1Ctrl.value.id != undefined && this.searchHorse2Ctrl.value != null && this.searchHorse2Ctrl.value.id != undefined))
            ||
            (this.searchTypeCtrl.value == 'wildcard2' && (this.searchHorse1Ctrl.value != null && this.searchHorse1Ctrl.value.id != undefined && this.searchHorse3Ctrl.value != null && this.searchHorse3Ctrl.value.id != undefined))
        )
    }

    search() {
        if (!this.enableSearch()) return;

        this.showSearchClear = true;
        this.loadData();
    }

    clearSearchBox() {
        this.searchHorse1Ctrl.setValue(null);
        this.searchHorse2Ctrl.setValue(null);
        this.searchHorse3Ctrl.setValue(null);
        this.searchHorse4Ctrl.setValue(null);
        this.horseData.data = [];
        this.showSearchClear = false;
    }

    loadData() {
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;
        let searchService;
        if (this.searchTypeCtrl.value == 'wildcard1') {
            searchService = this.horseService!.getHorsesForWildcard1Search(this.searchHorse1Ctrl.value.id, this.searchHorse2Ctrl.value.id);
        } else {
            const horse2Id = this.searchHorse2Ctrl.value && this.searchHorse2Ctrl.value.id ? this.searchHorse2Ctrl.value.id : null;
            const horse4Id = this.searchHorse4Ctrl.value && this.searchHorse4Ctrl.value.id ? this.searchHorse4Ctrl.value.id : null;
            searchService = this.horseService!.getHorsesForWildcard2Search(this.searchHorse1Ctrl.value.id, horse2Id, this.searchHorse3Ctrl.value.id, horse4Id);
        }
        this.subscription = searchService
            .subscribe(data => {
                this.loading = false;
                this.horseData.data = data;
            },
                err => {
                    this.loading = false;
                    console.error(err);
                });
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }

    toggleRaceBubble(tooltip, horse: Horse) {
        if (tooltip.isOpen()) {
            tooltip.close();
        } else {
            this.horseService!.getRacesForHorse(horse.id, 'Date', 'DESC')
                .subscribe(data => {
                    tooltip.open({
                        name: horse.name,
                        races: data
                    });
                })
        }
    }
}