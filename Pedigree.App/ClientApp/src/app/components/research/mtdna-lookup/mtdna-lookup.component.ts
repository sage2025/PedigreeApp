import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { MatSort, MatTableDataSource } from '@angular/material';
import { HaploGroup } from 'src/app/_models/haplo-group.model';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';
import { MtDnaService } from 'src/app/_services/mtdna.service';

@Component({
    selector: 'app-research-mtdna-lookup',
    templateUrl: './mtdna-lookup.component.html',
    styleUrls: ['./mtdna-lookup.component.css']
})

export class MtDNALookupComponent implements AfterViewInit {
  displayedColumns: string[] = ['pedigree', 'name', 'age', 'fatherName', 'motherName', 'mtDNATitle', 'g1Wnrs', 'g2Wnrs', 'g3Wnrs', 'lrWnrs', 'totalWnrs']
    dataSource = new MatTableDataSource<Horse>();

    loading: boolean;
    haploGroups: HaploGroup[] = [];

    @ViewChild(MatSort, { static: false }) sort: MatSort;
    constructor(
        private mtDNAService: MtDnaService,
        private horseService: HorseService,
    ) {
        
    }

    ngOnInit() {

        this.loading = true;
        this.mtDNAService.getSimpleHaploGroups()
          .subscribe(
            data => {
              this.loading = false;
              this.haploGroups = data;
            },
            error => {
              this.loading = false;
              console.error(error);
            }
          );
    }
    ngAfterViewInit() {

        this.dataSource.sort = this.sort;
    }

    selectHaploGroup($event) {
        const selectedHaploGroup = $event.value;

        this.loading = true;
        this.horseService.getHorsesForMtDNALookup(selectedHaploGroup.id)
            .subscribe(
                data => {
                    this.loading = false;
                    this.dataSource.data = data;
                },
                error => {
                    console.error(error);
                    this.loading = false;
                }
            );
    }
}
