import { Component, AfterViewInit, Input, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { HaploGroupStallion } from 'src/app/_models/haplo-group-stallion.model';
import { Horse } from 'src/app/_models/horse.model';
import { MtDnaService } from 'src/app/_services/mtdna.service';

@Component({
    selector: 'app-mtdna-stallion',
    templateUrl: './mtdna-stallion.component.html',
    styleUrls: ['./mtdna-stallion.component.scss']
})

export class MtDNAStallionComponent implements AfterViewInit {
    displayedColumns: string[] = ['group', 'ref_pop_count', 'ref_pop_count_percent', 'rated_horses', 'rated_horses_percent', 'three_plus_stars', 'three_plus_stars_percent', 'g1wnr', 'g1wnr_percent', 'g2wnr', 'g2wnr_percent', 'g3wnr', 'g3wnr_percent', 'swnr', 'swnr_percent'];
    dataSource = new MatTableDataSource<HaploGroupStallion>();
    loadingData: boolean = false;
    horse: Horse | string;
    showSearchClear: boolean = false;
    
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private mtDnaService: MtDnaService
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;     
    }

    enableSearch() {
      return this.horse && (<Horse>this.horse).id;
    }

    search() {
        if (!this.enableSearch()) return;

        this.loadData();
        this.showSearchClear = true;
    }

    clearSearchBox() {
        this.horse = null;
        this.dataSource.data = [];
        this.showSearchClear = false;
    }

    loadData() {
      this.loadingData = true;
      this.mtDnaService!.getHaploGroupsStallion((<Horse>this.horse).id)
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
