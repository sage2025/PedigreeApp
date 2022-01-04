import { Component, Inject, ViewChild } from "@angular/core";
import { MatDialogRef, MatSort, MatTableDataSource, MAT_DIALOG_DATA } from "@angular/material";
import { DialogData } from "src/app/_models/diaog-data";
import { Horse } from "src/app/_models/horse.model";
import { HorseService } from "src/app/_services/horse.service";

@Component({
    selector: 'app-research-sire-broodmare-crosses-modal',
    templateUrl: 'sire-broodmare-crosses.modal.component.html',
})

export class SireBroodmareCrossesModal {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'bmFatherName', 'bestRaceClass', 'mtDNATitle'];
    horseData = new MatTableDataSource<Horse>();
    
    @ViewChild(MatSort, { static: false }) sort: MatSort;
    constructor(
        public dialogRef: MatDialogRef<SireBroodmareCrossesModal>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private horseService: HorseService
    ) { 
        this.horseData.data = this.data.horses;
    }

    ngAfterViewInit() {
        this.horseData.sort = this.sort;
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