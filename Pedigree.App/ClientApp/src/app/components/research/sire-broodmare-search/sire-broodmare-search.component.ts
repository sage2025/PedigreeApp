import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSort, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-research-sire-broodmare-search',
    templateUrl: './sire-broodmare-search.component.html',
    styleUrls: ['./sire-broodmare-search.component.css'],
})

export class SireBroodmareSearchComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'bmFatherName', 'bestRaceClass', 'mtDNATitle'];
    horseData = new MatTableDataSource<Horse>();

    private subscription: Subscription;
    loading = false;
    showSearchClear: boolean = false;

    searchTypeCtrl = new FormControl('sire');
    searchSiresCtrl = new FormControl();

    isLoadingSires = false;
    errorLoadingSires: string;

    filteredSires: Horse[];

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
                    else {
                        this.errorLoadingSires = null;
                        this.filteredSires = data.filter(h => h.sex == 'Male');
                    }
                },
                error => {
                    this.errorLoadingSires = error;
                    console.log(error);
                }
            );

    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }

    search() {
        if (this.searchSiresCtrl.value != null) {
            this.showSearchClear = true;
            this.loadData();
        }
    }

    clearSearchBox() {
        this.searchSiresCtrl.setValue(null);
        this.horseData.data = [];
        this.showSearchClear = false;
    }

    loadData() {
        if (this.searchSiresCtrl.value == null) return;

        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;
        this.subscription = this.horseService!.getHorsesForSireBroodmareSireSearch(this.searchTypeCtrl.value, this.searchSiresCtrl.value.id)
            .subscribe(data => {
                this.loading = false;
                this.horseData.data = data;
            },
                err => {
                    this.loading = false;
                    console.error(err);
                });
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