import { Component, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { StallionService } from 'src/app/_services/stallion.service';
import { StallionRating } from 'src/app/_models/stallion-rating.model';

@Component({
  selector: 'app-ratings-list',
  templateUrl: './ratings-list.component.html',
  styleUrls: ['./ratings-list.component.css']
})
export class RatingsListComponent implements AfterViewInit {
  data = new MatTableDataSource<StallionRating>();

  private subscription: Subscription;
  resultsLength = 0;
  isLoadingData = true;
  searchQuery = '';
  showSearchClear = false;

  displayedColumns: string[];

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  @Input() dataType: string;
  @Output() dataLoaded = new EventEmitter<object>();

  constructor(private route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    private stallionService: StallionService)
  {
    
  }

  ngOnInit() {
    this.makeDisplayedColumns();
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    setTimeout(() => this.loadData());
  }

  showSireCount() {
    switch (this.dataType) {
      case 'current-sr':
      case 'historical-sr':
      case 'current-bms-sr':
      case 'historical-bms-sr':
        return false;
      case 'current-sos-sr':
      case 'historical-sos-sr':
      case 'current-bmsos-sr':
      case 'historical-bmsos-sr':
      default:
        return true;
    }
  }

  makeDisplayedColumns() {
    const columns = this.showSireCount() ? ['name', 'age', 'sCount', 'rCount', 'zCount', 'rating'] : ['name', 'age', 'rCount', 'zCount', 'rating'];

    if (this.dataType == 'current-sr' || this.dataType == 'historical-sr') {
      columns.push('iv');
      columns.push('ae');
      columns.push('prb2');
    }
    this.displayedColumns = columns;

  }

  loadData() {
    this.subscription = merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingData = true;
          return this.stallionService!.getStallionRatings(this.dataType, this.searchQuery, this.sort.active, this.sort.direction, this.paginator.pageIndex);
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingData = false;
          this.resultsLength = data.total;

          return data.data;
        }),
        catchError((error) => {
          console.error(error);
          this.isLoadingData = false;
          return observableOf([]);
        })
      ).subscribe(data => this.data.data = data);
  }

  searchItems() {
    this.subscription.unsubscribe();
    this.loadData();
    this.showSearchClear = true;
  }

  clearSearchBox() {
    this.searchQuery = '';
    this.showSearchClear = false;
    this.loadData();
  }
}
