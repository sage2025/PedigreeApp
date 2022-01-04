import { Component, AfterViewInit, ViewChild, Input } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { merge, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { HorseList } from 'src/app/_models/horse-list.model';
import { Horse } from '../../../_models/horse.model';
import { HorseService } from '../../../_services/horse.service';
import { HaploGroup } from 'src/app/_models/haplo-group.model';
import { HaploType } from 'src/app/_models/haplo-type.model';

@Component({
    selector: 'app-founder-list',
    templateUrl: './founder-list.component.html',
    styleUrls: ['./founder-list.component.scss']
})

export class FounderListComponent implements AfterViewInit {
    @Input() haploGroups: HaploGroup[];

    displayedColumns: string[] = ['no', 'name', 'sex', 'age', 'country', 'family', 'founder', 'mtdna'];
    data: HorseList = null;
    dataSource = new MatTableDataSource<Horse>();
    loadingData: boolean = false;
    resultsLength = 0;
    filters = {
        searchQuery: '',
        isFounder: false
    };

    

    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private horseService: HorseService,
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;

        setTimeout(() => {
            this.loadData();
        })
    }

    loadData() {
        this.loadingData = true;
        this.horseService!.getFounders()
            .subscribe(
                data => {
                    this.data = data;
                    this.dataSource.data = this.data.horses;
                    this.resultsLength = this.data.total;
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                }); 
    }

    setFamily(family, row) {
        row.family = family;
        this.horseService!.updateHorse(row)
            .subscribe(
                data => {
                    this.dataSource.data.find(h => h.id == row.id).family = family;
                },
                error => {
                    console.error(error);
                })
    }

    setFounder($event, row) {
        const checked = $event.checked;
        this.horseService!.setAsFounder(row.id, checked)
            .subscribe(
                data => {
                    this.dataSource.data.find(h => h.id == row.id).isFounder = checked;
                },
                error => {
                    console.error(error);
                })
    }

    changeFilter(filterName: string, filterValue: any) {
        if (filterName == 'searchQuery') this.filters.searchQuery = filterValue;
        else if (filterName == 'isFounder') this.filters.isFounder = filterValue;

        const data = new HorseList();
        data.horses = this.data.horses.filter(h => {
            let flag = true;
            if (this.filters.searchQuery.length > 0) {
                flag = flag && h.name.toLowerCase().indexOf(this.filters.searchQuery.toLowerCase()) > -1;
            }

            if (this.filters.isFounder) {
                flag = flag && h.isFounder == true;
            }
            return flag;
        });
        data.total = data.horses.length;

        this.dataSource.data = data.horses;
        this.resultsLength = data.total;
    }

    changeMtDNA(horse, $event) {
        this.horseService!.setMtDNA(horse.id, $event.value)
            .subscribe(
                data => {
                    
                },
                error => {
                    console.error(error);
                });
    }
}
