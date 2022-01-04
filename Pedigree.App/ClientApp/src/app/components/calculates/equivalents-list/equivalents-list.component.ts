import { Component, ViewChild, AfterViewInit, Input } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { Par3Item } from '../../../_models/par3-item.model';
import { HorseService } from '../../../_services/horse.service';


@Component({
    selector: 'app-equivalents-list',
    templateUrl: './equivalents-list.component.html',
    styleUrls: ['./equivalents-list.component.scss']
})

export class EquivalentsListComponent implements AfterViewInit {
    @Input() horseId: number;
    @Input() maleHorseId: number;
    @Input() femaleHorseId: number;
    private subscription: Subscription;
    displayedColumns: string[] = ['no', 'horseName1', 'horseName2', 'coi', 'commonAncestors'];
    dataSource = new MatTableDataSource<Par3Item>();
    resultsLength: number = 0;
    loading: boolean = false;

    @ViewChild(MatSort, { static: false }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

    constructor(private horseService: HorseService) {

    }

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        setTimeout(() => this.loadData());
    }

    loadData() {
        this.loading = true;
        if (this.horseId) {
            this.subscription = this.horseService!.getEquivalents(this.horseId)
                .subscribe(
                    data => {
                        this.loading = false;
                        this.dataSource.data = data
                    },
                    error => {
                        this.loading = false;
                    });
        } else if (this.maleHorseId && this.femaleHorseId) {
            this.subscription = this.horseService!.getEquivalentsForHypothetical(this.maleHorseId, this.femaleHorseId)
                .subscribe(
                    data => {
                        this.loading = false;
                        this.dataSource.data = data
                    },
                    error => {
                        this.loading = false;
                    });            
        }
    }

}