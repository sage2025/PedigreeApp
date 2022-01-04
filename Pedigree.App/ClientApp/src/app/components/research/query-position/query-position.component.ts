import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { MatDialog, MatSort, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';
import { AlertComponent } from 'src/app/_shared/alert/alert.component';

@Component({
    selector: 'app-research-query-position',
    templateUrl: './query-position.component.html',
    styleUrls: ['./query-position.component.css']
})

export class QueryPositionComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'motherName', 'bestRaceClass', 'mtDNATitle'];
    horseData = new MatTableDataSource<Horse>();

    sireIndexes = [0,2,3,6,7,8,9,14,15,16,17,18,19,20,21];
    damIndexes = [1,4,5,10,11,12,13,22,23,24,25,26,27,28,29];

    private subscription: Subscription;

    searches = {}; // index => horse

    showSearchClear = false;
    showSearchResult = false;
    loading = false;

    @ViewChild(MatSort, { static: false }) sort: MatSort;
    constructor(
        public dialog: MatDialog,
        private horseService: HorseService
    ) {
    }
    ngAfterViewInit() {
        this.horseData.sort = this.sort;
    }

    selectHorse(i, horse) {
        if (typeof horse == 'string' || horse == null) delete this.searches[i];
        else this.searches[i] = horse;
    }

    disableSireSide(index) {
        return (Object.keys(this.searches).indexOf(index.toString()) == -1) && Object.keys(this.searches).filter(x => this.sireIndexes.indexOf(parseInt(x)) > -1).length == 2;
    }

    disableDamSide(index) {
        return (Object.keys(this.searches).indexOf(index.toString()) == -1) && Object.keys(this.searches).filter(x => this.damIndexes.indexOf(parseInt(x)) > -1).length == 2;
    }

    enableSearch() {
        return (
            Object.keys(this.searches).filter(x => this.sireIndexes.indexOf(parseInt(x)) > -1).length > 0 &&
            Object.keys(this.searches).filter(x => this.damIndexes.indexOf(parseInt(x)) > -1).length > 0
        );
    }

    search() {
        if (!this.enableSearch()) return;

        this.showSearchClear = true;
        this.loadData();
    }

    clearSearchBox() {
        this.searches = {};
        this.horseData.data = [];
        this.showSearchClear = false;
        this.showSearchResult = false;
    }

    loadData() {
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;

        const searches = {}
        Object.keys(this.searches).forEach(i => {
            searches[i] = this.searches[i].id;
        })
        this.subscription = this.horseService!.getHorsesForWildcardQueryByPosition(searches)
            .subscribe(data => {
                this.loading = false;
                this.horseData.data = data;
                this.showSearchResult = true;
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