import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, first, map, startWith, switchMap, tap } from 'rxjs/operators';
import { FormGroup, FormControl } from '@angular/forms';
import { Race } from '../../../_models/race.model';
import { RaceService } from '../../../_services/race.service';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';
import { RACE_COUNTRIES, RACE_DISTANCES, RACE_SURFACES, RACE_TYPES, RACE_STATUSES } from '../../../constants';

@Component({
    selector: 'app-race-list',
    templateUrl: './race-list.component.html',
    styleUrls: ['./race-list.component.css']
})

export class RaceListComponent implements AfterViewInit {
    private subscription: Subscription;
    displayedColumns: string[] = ['date', 'country', 'name', 'distance', 'surface', 'type', 'status', 'weight', 'rnrs', 'BPR', 'actions'];
    resultsLength: number = 0;
    raceData = new MatTableDataSource<Race>();
    loading: boolean = true;
    filter: object = {};

    years: number[] = Array(50).fill(0).map((_, idx) => new Date().getFullYear() - 49 + idx).reverse();
    countries: string[] = RACE_COUNTRIES;
    distances: object[] = RACE_DISTANCES;
    surfaces: string[] = RACE_SURFACES;
    types: string[] = RACE_TYPES;
    statuses: string[] = RACE_STATUSES;

    showSearchClear: boolean = false;
    raceSearchForm: FormGroup = new FormGroup({
        searchRaceNamesCtrl: new FormControl('')
    });

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private raceService: RaceService,
        public dialog: MatDialog
    ) {

    }

    ngOnInit() {
        this.setRaceNameFilterCtrl();
    }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
        this.loadData();
    }

    onSearchRaceByName(value: string) {
        console.log(value)
    }
    setRaceNameFilterCtrl() {
        this.raceSearchForm.get('searchRaceNamesCtrl').valueChanges
            .pipe(
                debounceTime(400),
                distinctUntilChanged()
            )
            .subscribe(value => {
                this.filter = {...this.filter, Name: value};
                this.searchItems();
            });
    }

    onAdd() {
        this.router.navigate([`races/add`]);
    }

    loadData(isLoadFlag = true) {
        this.subscription = merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    if (!isLoadFlag) {
                        this.loading = false;
                        return [];
                    }
                    else {
                        this.loading = true;
                        return this.raceService!.getRaces(this.filter, this.sort.active, this.sort.direction, this.paginator.pageIndex);
                    }
                }),
                map(response => {
                    // Flip flag to show that loading has finished.
                    this.loading = false;

                    this.resultsLength = response.total;
                    return response.data;
                }),
                catchError((error) => {
                    console.error(error);
                    this.loading = false;
                    return observableOf([]);
                })
            ).subscribe(data => this.raceData.data = data);
    }

    onEdit(raceId: string) {
        this.router.navigate([`races/${raceId}`]);
    }

    onDelete(raceId: number, name: string) {

        this.loading = true;
        // Call the API to get if this horse as children, and show prompt accoringly
        this.raceService.checkHasResult(raceId).pipe(first())
            .subscribe(
                result => {
                    this.loading = false;
                    var message = "";
                    if (result) {
                        message = "This race has position result. Do you want to delete this race?";
                        name = name + " (has result)";
                    }
                    else {
                        message = "Are you sure you want to delete this race?";
                    }

                    const deleteDialogRef = this.dialog.open(ConfirmComponent, {
                        width: '250px',
                        data: { name: name, message: message, showAlert: 1 }
                    });

                    deleteDialogRef.afterClosed().subscribe(result => {
                        if (result) {
                            this.loading = true;
                            this.raceService.deleteRace(raceId).pipe(first())
                                .subscribe(
                                    data => {
                                        this.loading = false;
                                        var itemIndex = this.raceData.data.findIndex(h => h.id == raceId);
                                        this.raceData.data.splice(itemIndex, 1);
                                        this.paginator.length = this.raceData.data.length;
                                        this.raceData._updateChangeSubscription();
                                    },
                                    error => {
                                        this.loading = false;
                                        console.log(error);
                                    })
                        }
                    });
                },
                error => {
                    this.loading = false;
                    console.log(error);
                });
    }

    onView(raceId) {
        this.router.navigate(['positions'], { queryParams: { raceId } });
    }

    searchItems() {
        this.subscription.unsubscribe();
        this.loadData();
        if (Object.keys(this.filter).length > 0) this.showSearchClear = true;
    }

    clearSearchBox() {
        this.showSearchClear = false;
        this.filter = {};
        this.loadData();
    }
}

