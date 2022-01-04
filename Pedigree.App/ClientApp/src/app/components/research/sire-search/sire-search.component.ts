import { ViewChild } from '@angular/core';
import { Component, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSort, MatTableDataSource } from '@angular/material';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-research-sire-search',
    templateUrl: './sire-search.component.html',
    styleUrls: ['./sire-search.component.css']
})

export class SireSearchComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'father', 'mother', 'bestRaceClass', 'mtDNATitle'];
    horseData = new MatTableDataSource<Horse>();

    private subscription: Subscription;
    resultsLength = 0;
    loading = false;
    enableSearch: boolean = false;
    showSearchClear: boolean = false;

    searchSiresCtrl = new FormControl();
    searchDamsCtrl = new FormControl();

    isLoadingSires = false;
    errorLoadingSires: string;
    isLoadingDams = false;
    errorLoadingDams: string;

    filteredSires: Horse[];
    filteredDams: Horse[];

    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(
        private horseService: HorseService
    ) {

    }

    ngAfterViewInit() {
        this.horseData.sort = this.sort;
        this.horseSearchSetup();
    }

    loadData() {
        if (this.searchSiresCtrl.value == null || this.searchDamsCtrl.value == null) return;
        
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;
        this.subscription = this.horseService!.getHorsesForSireSearch(this.searchSiresCtrl.value.id, this.searchDamsCtrl.value.id)
            .subscribe(data => {
                this.loading = false;
                this.horseData.data = data;
            },
                err => {
                    this.loading = false;
                    console.error(err);
                });
    }

    horseSearchSetup() {
        this.searchSiresCtrl.valueChanges
            .pipe(
                debounceTime(500),
                tap((value) => {
                    this.isLoadingSires = true;
                    this.filteredSires = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingSires = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingSires = 'No matches';
                        this.filteredSires = [];
                    }
                    else 
                    {
                        this.errorLoadingSires = null;
                        this.filteredSires = data.filter(h => h.sex == 'Male');
                    }
                },
                error => {
                    this.errorLoadingSires = error;
                    console.log(error);
                }
            );

        this.searchDamsCtrl.valueChanges
            .pipe(
                debounceTime(500),
                tap(() => {
                    this.isLoadingDams = true;
                    this.filteredDams = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingDams = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingDams = 'No matches';
                        this.filteredDams = [];
                    }
                    else 
                    {
                        this.errorLoadingDams = null;
                        this.filteredDams = data.filter(h => h.sex == 'Female');
                    }
                },
                error => {
                    this.errorLoadingDams = error;
                    console.log(error);
                }
            );
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }

    selectHorse(horse: Horse, type: string) {
        this.enableSearch = this.searchSiresCtrl.value && this.searchDamsCtrl.value;
    }

    search() {
        if (this.searchSiresCtrl.value != null && this.searchDamsCtrl.value != null) {
            this.showSearchClear = true;
            this.loadData();
        }
    }
    clearSearchBox() {
        this.searchSiresCtrl.setValue(null);
        this.searchDamsCtrl.setValue(null);
        this.horseData.data = [];
        this.showSearchClear = false;
        this.enableSearch = false;
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