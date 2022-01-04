import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatPaginator, MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, first, map, startWith, switchMap } from 'rxjs/operators';
import { Horse } from '../../../_models/horse.model';
import { HorseService } from '../../../_services/horse.service';
import { ConfirmComponent } from '../../../_shared/confirm/confirm.component';
import { AlertComponent } from '../../../_shared/alert/alert.component';


@Component({
  selector: 'app-horse-list',
  templateUrl: './horse-list.component.html',
  styleUrls: ['./horse-list.component.css']
})

export class HorseListComponent implements AfterViewInit {

  displayedColumns: string[];
  horseData = new MatTableDataSource<Horse>();

  private subscription: Subscription;
  resultsLength = 0;
  isLoadingResults = false;
  searchQuery: string = "";
  sireSearchQuery = "";
  damSearchQuery = "";
  showSearchClear: boolean = false;
  showHypoSearchClear: boolean = false;
  isHypotheticalSearch = false;
  isHypotheticalResultset = false;
  enableHypotheticalSearch = true;
  hypotheticalMatings: Horse[] = [];
  showGetPedigreeButton = false;
  //getPedigreeUrl: string;
  maleId: any;
  femaleId: any;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(private route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    private horseService: HorseService) {
    this.updateDisplayColumns();
  }

  updateDisplayColumns() {
    if (this.isHypotheticalResultset) {
      this.displayedColumns = ['radio', 'name', 'sex', 'age', 'country', 'father', 'mother', 'bestRaceClass', 'mtDNA']
    }
    else {
      this.displayedColumns = ['pedigree', 'name', 'sex', 'age', 'country', 'father', 'mother', 'bestRaceClass', 'mtDNA', 'actions']
    }
  }

  isGroup(index, item): boolean {
    return item.showHeader;
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.loadData(false);
  }

  loadData(isLoadFlag = true) {

    this.subscription = merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          if (!isLoadFlag) {
            this.isLoadingResults = false;
            return [];
          }
          else {
            this.isLoadingResults = true;
            if (this.isHypotheticalSearch) {
              return this.horseService!.getHorsesForHypotheticalMating(this.sireSearchQuery, this.damSearchQuery,
                this.sort.active, this.sort.direction, this.paginator.pageIndex);

            }
            else {
              return this.horseService!.getHorses(this.searchQuery,
                this.sort.active, this.sort.direction, this.paginator.pageIndex);
            }
          }
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          if (this.isHypotheticalSearch) {
            this.isHypotheticalResultset = true;
            this.updateDisplayColumns();
          }
          else {
            this.isHypotheticalResultset = false; 
            this.updateDisplayColumns();
          }
          this.resultsLength = data.total;
          return data.horses;
        }),
        catchError((error) => {
          console.log(error);
          this.isLoadingResults = false;
          return observableOf([]);
        })
      ).subscribe(data => this.horseData.data = data);
  }

  applyFilter(filterValue: string) {
    this.searchQuery = filterValue;
    if (filterValue != null || filterValue != '') {
      this.showSearchClear = true;
    }
  }

  // Enable hypothetical search button only when both fields are filled
  onKeyupCheck() {
    if (this.sireSearchQuery != "" && this.damSearchQuery != "") {
      this.enableHypotheticalSearch = false;
    }
    else {
      this.enableHypotheticalSearch = true;
    }

  }

  searchHypothetical() {
    if (this.sireSearchQuery != "" && this.damSearchQuery != "") {
      this.isHypotheticalSearch = true;
      this.showHypoSearchClear = true;
      this.hypotheticalMatings = [];
      this.showGetPedigreeButton = false;
      this.subscription.unsubscribe();
      this.loadData();
    }
  }

  rowSelected($event, row) {
    if ($event.checked) {
      this.hypotheticalMatings.push(row);
    }
    else {
      this.hypotheticalMatings = this.hypotheticalMatings.filter(f => f.id != row.id);
    }
    if (this.hypotheticalMatings.length == 2 && this.hypotheticalMatings.find(f => f.sex == 'Male') != null && this.hypotheticalMatings.find(f => f.sex == 'Female') != null) {
      this.maleId = this.hypotheticalMatings.find(f => f.sex == 'Male').id;
      this.femaleId = this.hypotheticalMatings.find(f => f.sex == 'Female').id;
      this.showGetPedigreeButton = true;
    } else {
      this.showGetPedigreeButton = false;
      this.maleId = null;
      this.femaleId = null;
    }
  }

  searchItems() {
    this.clearHyptheticalFlags();
    this.subscription.unsubscribe();
    this.loadData();
    this.showSearchClear = true;
  }

  clearHyptheticalFlags() {
    this.isHypotheticalSearch = false;
    this.damSearchQuery = '';
    this.sireSearchQuery = '';
    this.maleId = null;
    this.femaleId = null;
    this.enableHypotheticalSearch = true;
    this.hypotheticalMatings = [];
    this.showGetPedigreeButton = false;
  }

  clearSearchBox() {
    this.searchQuery = '';
    this.damSearchQuery = '';
    this.sireSearchQuery = '';
    this.showSearchClear = false;
    this.showHypoSearchClear = false;
    this.enableHypotheticalSearch = true;
    this.hypotheticalMatings = [];
    this.showGetPedigreeButton = false;
    this.horseData.data = [];
  }

  triggerSearch(name: string) {
    this.searchQuery = name;
    this.showSearchClear = true;
    this.subscription.unsubscribe();
    this.loadData();
  }

  onAdd() {
    this.router.navigate([`horses/add/new`]);
  }

  onBulkAdd() {
    this.router.navigate([`horses/add/bulk`]);
  }

  onDelete(horseId: number, name: string, oId: string) {

    this.isLoadingResults = true;
    // Call the API to get if this horse as children, and show prompt accoringly
    this.horseService.checkHorseLinkage(oId).pipe(first())
      .subscribe(
        result => {
          this.isLoadingResults = false;
          if (result['hasRaceResult']) {
            this.dialog.open(AlertComponent, {
              width: '350px',
              data: { title: 'Warning', message: 'Cannot delete a horse attached to a race result. Please remove that result before deleting this horse.', showAlert: 1 }
            });
          } else {
            var message = "";
            if (result['hasChildren']) {
              // Children exists
              message = "This record has child records. Do you want to delete this record?";
              name = name + " (has children)";
            }
            else {
              message = "Are you sure you want to delete this horse?";
            }

            const deleteDialogRef = this.dialog.open(ConfirmComponent, {
              width: '350px',
              data: { name: name, message: message, showAlert: 1 }
            });

            deleteDialogRef.afterClosed().subscribe(result => {
              if (result) {
                this.horseService.deleteHorse(horseId).pipe(first())
                  .subscribe(data => {
                    var itemIndex = this.horseData.data.findIndex(h => h.id == horseId);
                    this.horseData.data.splice(itemIndex, 1);
                    this.paginator.length = this.horseData.data.length;
                    this.horseData._updateChangeSubscription();
                  },
                    error => {
                      console.log(error);
                    })
              }
            });
          }
        },
        error => {
          this.isLoadingResults = false;
          console.log(error);
        });
  }

  onEdit(horseId: string) {
    this.router.navigate([`horses/${horseId}`]);
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
