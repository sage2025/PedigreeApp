import { Component, AfterViewInit, ViewChild, Input } from '@angular/core';
import { MatSort, MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { Ancestry } from '../../../_models/ancestry.model';
import { HorseService } from '../../../_services/horse.service';
import { HaploGroup } from 'src/app/_models/haplo-group.model';

@Component({
    selector: 'app-ancestry-ancestor-values',
    templateUrl: './ancestor-values.component.html',
    styleUrls: ['./ancestor-values.component.scss']
})

export class AncestryAncestorValuesComponent implements AfterViewInit {
    private subscription: Subscription;
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'country', 'mtDNATitle', 'avgMC'];
    dataSource = new MatTableDataSource<Ancestry>();
    summary: any = {};
    loadingData: boolean = false;

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private horseService: HorseService,
    ) {

    }
    
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;

        setTimeout(() => this.loadData());
    }

    loadData() {
        this.loadingData = true;
        this.subscription = this.horseService!.getAncestorData()
            .subscribe(
                data => {
                    this.dataSource.data = data.ancestries;
                    this.summary = {
                        horsesCount: data.horsesCount,
                        ancestorsCount: data.ancestorsCount,
                        genomePercent: data.genomePercent
                    };
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                }); 
    }
}
