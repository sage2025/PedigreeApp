import { Component, ViewChild, AfterViewInit } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, first, map, startWith, switchMap } from 'rxjs/operators';
import { Weight } from '../../../_models/weight.model';
import { WeightService } from '../../../_services/weight.service';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';
import { RACE_COUNTRIES, RACE_DISTANCES, RACE_SURFACES, RACE_TYPES, RACE_STATUSES } from '../../../constants';

@Component({
    selector: 'app-weight-list',
    templateUrl: './weight-list.component.html',
    styleUrls: ['./weight-list.component.css']
})

export class WeightListComponent implements AfterViewInit {
    private subscription: Subscription;
    displayedColumns: string[] = ['country', 'distance', 'surface', 'type', 'status', 'weight', 'actions'];
    resultsLength: number = 0;
    weightData = new MatTableDataSource<Weight>();
    loading: boolean = true;
    filter: object = {};
    
    countries: string[] = RACE_COUNTRIES;
    distances: object[] = RACE_DISTANCES;
    surfaces: string[] = RACE_SURFACES;
    types: string[] = RACE_TYPES;
    statuses: string[] = RACE_STATUSES;

    showSearchClear: boolean = false;

    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(
        private router: Router,
        private weightService: WeightService,
        public dialog: MatDialog
    ) {

    }

    ngAfterViewInit() {
        this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
        this.loadData();
    }
    onAdd() {
        this.router.navigate([`weights/add`]);
    }

    loadData(isLoadFlag = true) {
        this.loading = true;
        this.subscription = merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                startWith({}),
                switchMap(() => {
                    if (!isLoadFlag) {
                        this.loading = false;
                        return [];
                    }
                    else {
                        return this.weightService!.getWeights(this.filter, this.sort.active, this.sort.direction, this.paginator.pageIndex);
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
            ).subscribe(data => this.weightData.data = data);
    }

    onEdit(weightId: string) {
        this.router.navigate([`weights/${weightId}`]);
    }

    onDelete(weightId: number, name: string) {

        this.loading = true;
        // Call the API to get if this horse as children, and show prompt accoringly
        const deleteDialogRef = this.dialog.open(ConfirmComponent, {
            width: '250px',
            data: { name: name, message: 'Are you sure to delete this weight', showAlert: 1 }
        });

        deleteDialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.loading = true;
                this.weightService.deleteWeight(weightId).pipe(first())
                    .subscribe(
                        data => {
                            this.loading = false;
                            var itemIndex = this.weightData.data.findIndex(h => h.id == weightId);
                            this.weightData.data.splice(itemIndex, 1);
                            this.paginator.length = this.weightData.data.length;
                            this.weightData._updateChangeSubscription();
                        },
                        error => {
                            this.loading = false;
                            console.log(error);
                        })
            }
        });
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
  
  