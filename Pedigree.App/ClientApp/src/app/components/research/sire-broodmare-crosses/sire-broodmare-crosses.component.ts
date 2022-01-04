import { Component, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { SireCrossData } from 'src/app/_models/sire-cross-data.model';
import { HorseService } from 'src/app/_services/horse.service';
import { SireBroodmareCrossesModal } from './sire-broodmare-crosses.modal.component';

@Component({
    selector: 'app-research-sire-broodmare-crosses',
    templateUrl: './sire-broodmare-crosses.component.html',
    styleUrls: ['./sire-broodmare-crosses.component.css']
})

export class SireBroodmareCrossesComponent implements AfterViewInit {
    private subscription: Subscription;
    loading = false;
    enableSearch: boolean = false;
    showSearchClear: boolean = false;

    searchSiresCtrl = new FormControl();
    searchBmSiresCtrl = new FormControl();

    isLoadingSires = false;
    errorLoadingSires: string;
    isLoadingBmSires = false;
    errorLoadingBmSires: string;

    filteredSires: Horse[];
    filteredBmSires: Horse[];
    
    crossData: SireCrossData = null;

    constructor(
        private horseService: HorseService,
        public dialog: MatDialog
    ) {

    }

    ngAfterViewInit() {
        this.horseSearchSetup();
    }

    loadData() {
        if (this.searchSiresCtrl.value == null || this.searchBmSiresCtrl.value == null) return;
        
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;
        this.subscription = this.horseService!.getSiresCrossData(this.searchSiresCtrl.value.id, this.searchBmSiresCtrl.value.id)
            .subscribe(data => {
                this.loading = false;
                this.crossData = data;
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

        this.searchBmSiresCtrl.valueChanges
            .pipe(
                debounceTime(500),
                tap(() => {
                    this.isLoadingBmSires = true;
                    this.filteredBmSires = [];
                }),
                switchMap(value => this.horseService.searchHorseStartsWith(value)
                    .pipe(
                        finalize(() => {
                            this.isLoadingBmSires = false;
                        })
                    )
                )
            ).subscribe(
                data => {
                    if (data == undefined || data == null || data.length == 0) {
                        this.errorLoadingBmSires = 'No matches';
                        this.filteredBmSires = [];
                    }
                    else 
                    {
                        this.errorLoadingBmSires = null;
                        this.filteredBmSires = data.filter(h => h.sex == 'Male');
                    }
                },
                error => {
                    this.errorLoadingBmSires = error;
                    console.log(error);
                }
            );
    }

    displayHorse(horse: Horse): string {
        return horse && horse.name ? horse.name : '';
    }

    selectHorse(horse: Horse, type: string) {
        this.enableSearch = this.searchSiresCtrl.value && this.searchBmSiresCtrl.value;
    }

    search() {
        if (this.searchSiresCtrl.value != null && this.searchBmSiresCtrl.value != null) {
            this.showSearchClear = true;
            this.loadData();
        }
    }
    clearSearchBox() {
        this.searchSiresCtrl.setValue(null);
        this.searchBmSiresCtrl.setValue(null);
        this.showSearchClear = false;
        this.enableSearch = false;
        this.crossData = null;
    }

    onClickCrosses(si1: number, si2: number) {
        const dialogRef = this.dialog.open(SireBroodmareCrossesModal, {
            width: '1050px',
            data: {
                sire1: this.crossData.sires1[si1],
                sire2: this.crossData.sires2[si2],
                horses: this.crossData.crosses[si2 * this.crossData.sires2.length + si1]
            }
          });
      
          dialogRef.afterClosed().subscribe(result => {
              
          });
    }
}