import { Component, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { Horse } from '../../_models/horse.model';
import { RelationshipService } from '../../_services/relationship.service';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
  selector: 'app-horse-list-view',
  templateUrl: './horse-list-view.component.html',
  styleUrls: ['./horse-list-view.component.css']
})
export class HorseListViewComponent implements AfterViewInit {
  displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'country', 'father', 'mother', 'mtDNA'];
  horseData = new MatTableDataSource<Horse>();

  private subscription: Subscription;
  resultsLength = 0;
  isLoadingResults = true;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  @Input() horseId: number;
  @Input() maleHorseId: number;
  @Input() femaleHorseId: number;
  @Input() dataType: string;
  @Output() dataLoaded = new EventEmitter<object>();

  constructor(private route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    private relationshipService: RelationshipService,
    private horseService: HorseService) {
  }

  ngAfterViewInit(): void {
    this.horseData.sort = this.sort;
    this.horseData.paginator = this.paginator;
    if (this.horseId || this.maleHorseId && this.femaleHorseId) {
      setTimeout(() => this.loadData());
    }

    if (this.dataType.toLocaleLowerCase() == 'femaletail') {
      this.displayedColumns.push('family');
    } else if (this.dataType.toLocaleLowerCase() != 'uniqueancestors') {
      this.displayedColumns.push('bestRaceClass');
    }
  }

  loadData() {
    const fetchService = () => {
      switch (this.dataType.toLowerCase()) {
        case "offsprings":
          return this.relationshipService!.getOffsprings(this.horseId);
        case "siblings":
          return this.relationshipService!.getSiblings(this.horseId);
        case "femaletail":
          return this.relationshipService!.getFemailTail(this.horseId);
        case "uniqueancestors":
          {
            if (this.horseId) {
              return this.relationshipService!.getUniqueAncestors(this.horseId);
            } else if (this.maleHorseId && this.femaleHorseId) {
              return this.relationshipService!.getUniqueAncestorsForHypothetical(this.maleHorseId, this.femaleHorseId);
            }
          }
      }
    }


    this.isLoadingResults = true;
    if (this.subscription) this.subscription.unsubscribe();
    this.subscription = fetchService().subscribe(
      data => {
        this.isLoadingResults = false;
        this.horseData.data = data.horses;
        this.resultsLength = data.horses.length;

        if (this.dataType.toLowerCase() == 'uniqueancestors') {
          this.dataLoaded.emit({
            type: 'uniqueancestors',
            count: data.horses.length
          });
        }
      },
      error => {
        this.isLoadingResults = false;
        console.error(error);
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
