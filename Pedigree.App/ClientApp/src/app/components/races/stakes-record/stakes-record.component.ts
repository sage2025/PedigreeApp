import { Component, ViewChild, AfterViewInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSort, MatTableDataSource, MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';
import { Race } from '../../../_models/race.model';
import { HorseService } from '../../../_services/horse.service';

@Component({
    selector: 'app-stakes-record',
    templateUrl: './stakes-record.component.html',
    styleUrls: ['./stakes-record.component.css']
})

export class StakesRecordComponent implements AfterViewInit {
    private subscription: Subscription;
    displayedColumns: string[] = ['place', 'date', 'country', 'name', 'distance', 'surface', 'type', 'status', 'bpr', 'actions'];
    @Input() horseId: number;
    data = new MatTableDataSource<Race>();
    loading: boolean = false;

    @ViewChild(MatSort, { static: false }) sort: MatSort;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private horseService: HorseService
    ) {

    }

    ngAfterViewInit() {
        this.data.sort = this.sort;
        setTimeout(() =>this.loadData());
    }


    loadData() {
        this.loading = true;
        this.subscription =
            this.horseService!.getRacesForHorse(this.horseId, this.sort.active, this.sort.direction)
                .subscribe(
                    data => {
                        this.loading = false;
                        this.data.data = data
                    }, err => {
                        this.loading = false;
                        console.error(err);
                    });
    }

    onView(raceId) {
        this.router.navigate(['positions'], { queryParams: { raceId } });
    }
}

