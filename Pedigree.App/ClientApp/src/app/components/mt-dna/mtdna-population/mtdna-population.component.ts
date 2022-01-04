import { Component, AfterViewInit, Input, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { HaploGroup } from 'src/app/_models/haplo-group.model';

@Component({
    selector: 'app-mtdna-population',
    templateUrl: './mtdna-population.component.html',
    styleUrls: ['./mtdna-population.component.scss']
})

export class MtDNAPopulationComponent implements AfterViewInit {
    @Input() haploGroups: HaploGroup[];
    @Input() loadingData: boolean = false;

    displayedColumns: string[] = ['group', 'ref_pop_count', 'ref_pop_count_percent', 'rated_horses', 'rated_horses_percent', 'three_plus_stars', 'three_plus_stars_percent', 'elite', 'elite_percent', 'non_elite', 'non_elite_percent'];
    data: HaploGroup[] = [];
    dataSource = new MatTableDataSource<HaploGroup>();
    
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;        
    }

    ngOnChanges() {
        this.dataSource.data = this.haploGroups;
    }
}
