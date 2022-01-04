import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatTableDataSource, MatDialog, MatSnackBar } from '@angular/material';
import { of as observableOf, Subscription } from 'rxjs';
import { catchError, first, map, startWith, switchMap } from 'rxjs/operators';
import { Position } from '../../../_models/position.model';
import { PositionService } from '../../../_services/position.service';
import { RaceService } from '../../../_services/race.service';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';
import { NotificationSnackComponent } from '../../../_shared/notification-snack/notification-snack.component';
import { Race } from 'src/app/_models/race.model';

@Component({
    selector: 'app-position-list',
    templateUrl: './position-list.component.html',
    styleUrls: ['./position-list.component.css']
})

export class PositionListComponent implements AfterViewInit {
    private subscription: Subscription;
    raceId: number;
    race: Race;
    displayedColumns: string[] = ['pedigree', 'place', 'name', 'sex', 'age', 'father', 'mother', 'actions'];
    resultsLength: number = 0;
    positionData = new MatTableDataSource<Position>();
    loading: boolean = true;
    searchQuery: string = '';
    showSearchClear: boolean = false;
    isLoading: boolean = false;
    errorMsg: string = '';

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private positionService: PositionService,
        private raceService: RaceService,
        public dialog: MatDialog,
        private _snackBar: MatSnackBar
    ) {
        this.raceId = parseInt(this.route.snapshot.queryParams.raceId);

        this.raceService.getRace(this.raceId).pipe(first())
            .subscribe(result => {
                this.race = result;
            });
    }

    ngAfterViewInit() {

        this.loadData();
    }

    loadData(isLoadFlag = true) {
        this.loading = true;

        this.subscription = this.paginator.page
            .pipe(
                startWith({}),
                switchMap(() => {
                    if (!isLoadFlag) {
                        this.loading = false;
                        return [];
                    }
                    else {
                        return this.positionService!.getPositions(this.raceId, this.searchQuery, this.paginator.pageIndex);
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
            ).subscribe(data => this.positionData.data = data);
    }

    onEdit(raceId: string) {
        this.router.navigate([`races/${raceId}`]);
    }

    onDelete(positionId: number, horseName: string) {
        const deleteDialogRef = this.dialog.open(ConfirmComponent, {
            width: '350px',
            data: { name: name, message: `Are you sure delete ${horseName}'s result record`, showAlert: 1 }
        });

        deleteDialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.loading = true;
                this.positionService.deletePosition(positionId).pipe(first())
                    .subscribe(
                        data => {
                            this.loading = false;
                            var itemIndex = this.positionData.data.findIndex(h => h.id == positionId);
                            this.positionData.data.splice(itemIndex, 1);
                            this.paginator.length = this.positionData.data.length;
                            this.positionData._updateChangeSubscription();
                        },
                        error => {
                            this.loading = false;
                            console.log(error);
                        })
            }
        });
    }


    update(data) {        
        this.openSnackBar(data.message|| data.error, data.error ? 1 : 2);
        this.loadData();
    }

    searchItems() {
        this.subscription.unsubscribe();
        this.loadData();
        if (this.searchQuery.length > 0) this.showSearchClear = true;
    }

    clearSearchBox() {
        this.showSearchClear = false;
        this.searchQuery = '';
        this.loadData();
    }

    openSnackBar(message: string, showSuccess: number = 1) {
        this._snackBar.openFromComponent(NotificationSnackComponent, {
            duration: 5000,
            data: { message: message, showAlert: showSuccess } // 2 stands for success 1 for error
        });
    }
}

