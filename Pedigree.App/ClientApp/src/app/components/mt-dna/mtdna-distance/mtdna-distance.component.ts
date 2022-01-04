import { Component, AfterViewInit, Input, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { HaploGroupDistance } from 'src/app/_models/haplo-group-distance.model';
import { HaploGroupStallion } from 'src/app/_models/haplo-group-stallion.model';
import { MtDnaService } from 'src/app/_services/mtdna.service';

@Component({
    selector: 'app-mtdna-distance',
    templateUrl: './mtdna-distance.component.html',
    styleUrls: ['./mtdna-distance.component.scss']
})

export class MtDNADistanceComponent implements AfterViewInit {
    displayedColumns: string[] = ['group', 'first_place_count', 'sprint_percent', 'sprinter_miler_percent', 'intermediate_percent', 'long_percent', 'extended_percent'];
    dataSource = new MatTableDataSource<HaploGroupDistance>();
    loadingData: boolean = false;
    showSearchClear: boolean = false;
    
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private mtDnaService: MtDnaService
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;  
        setTimeout(() => this.loadData())   ;
    }

    loadData() {
      this.loadingData = true;
      this.mtDnaService!.getHaploGroupsDistance()
        .subscribe(
          data => {
            this.loadingData = false;
            this.dataSource.data = data;
          },
          error => {
            this.loadingData = false;
            console.error(error);
          })
    }
}
