import { Component, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { Horse } from '../../../_models/horse.model';
import { RelationshipService } from '../../../_services/relationship.service';

@Component({
  selector: 'app-research-grandparents',
  templateUrl: './grandparents.component.html',
  styleUrls: ['./grandparents.component.css']
})
export class GrandparentsComponent implements AfterViewInit {
  displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'country', 'fatherName', 'motherName', 'family', 'mtDNA'];
  horseData = new MatTableDataSource<Horse>();

  private subscription: Subscription;
  resultsLength = 0;
  loading = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(
    private relationshipService: RelationshipService) {
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.loadData();
  }

  loadData() {    
    this.subscription = merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.loading = true;    
          return this.relationshipService!.getGrandparents(this.sort.active, this.sort.direction, this.paginator.pageIndex)
        }),
        map(data => {
          console.log(data);
          this.loading = false;
          this.resultsLength = data.total;
          return data.horses;
        }),
        catchError((error) => {
          console.error(error);
          this.loading = false;
          return observableOf([]);
        })
      ).subscribe(data => this.horseData.data = data);
  }
}
