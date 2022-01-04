import { Component, ViewChild, AfterViewInit, Input } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { LinebreedingItem } from '../../../_models/linebreeding-item.model';
import { HorseService } from '../../../_services/horse.service';


@Component({
    selector: 'app-linebreeding-list',
    templateUrl: './linebreeding-list.component.html',
    styleUrls: ['./linebreeding-list.component.scss']
})

export class LinebreedingListComponent implements AfterViewInit {
    @Input() horseId: number;
    @Input() femaleHorseId: number;
    @Input() maleHorseId: number;

    private subscription: Subscription;
    displayedColumns: string[] = ['no', 'name', 'stats', 'crosses', 'inbreed', 'relation'];
    linebreedingData = new MatTableDataSource<LinebreedingItem>();
    resultsLength: number = 0;
    loading: boolean = false;

    @ViewChild(MatSort, { static: false }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

    constructor(private horseService: HorseService) {

    }

    ngAfterViewInit() {
        this.linebreedingData.paginator = this.paginator;
        this.linebreedingData.sort = this.sort;
        setTimeout(() => this.loadData());
    }

    loadData() {
        this.loading = true;
        if (this.horseId) {
            this.subscription = this.horseService!.getLinebreedings(this.horseId)
                .subscribe(
                    data => {
                        this.loading = false;
                        this.linebreedingData.data = data
                    },
                    error => {
                        this.loading = false;
                    });
        } else if (this.maleHorseId && this.femaleHorseId) {            
            this.subscription = this.horseService!.getLinebreedingsForHypothetical(this.maleHorseId, this.femaleHorseId)
                .subscribe(
                    data => {
                        this.loading = false;
                        this.linebreedingData.data = data
                    },
                    error => {
                        this.loading = false;
                    });
        }
    }

}