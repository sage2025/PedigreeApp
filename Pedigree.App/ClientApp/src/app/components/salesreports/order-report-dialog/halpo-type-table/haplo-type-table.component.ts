import { Component, ViewChild, Input } from "@angular/core";
import { MatSort, MatTableDataSource } from "@angular/material";
import { Horse } from "src/app/_models/horse.model";
import { HorseHeirarchy } from "src/app/_models/horse-heirarchy.model";
import _ from 'lodash';
import { HorseService } from "src/app/_services/horse.service";

@Component({
    selector: 'app-haplo-type-table',
    templateUrl: './haplo-type-table.component.html',
    styleUrls: ['./haplo-type-table.component.css']
})
export class HaploTypeTableComponent {
    @Input() inputMtDNAHapTitle: string;
    @Input() inputHaploData: any;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'motherName', 'bestRaceClass', 'mtDNATitle'];
    haplotypeData = new MatTableDataSource<Horse>();
    subject_haplo_type: string;
    subject_haplo_group: string;
    loadingData: boolean = true;
    
    public mtDNAHapTitle: string;
    public haploData: any;

    constructor(
        private horseService: HorseService
    ) {}

    ngAfterViewInit() {
        this.haplotypeData.sort = this.sort;
    }
    ngOnChanges() {

        if (!_.isEqual(this.mtDNAHapTitle, this.inputMtDNAHapTitle)) {
            this.mtDNAHapTitle = this.inputMtDNAHapTitle;
            if(this.mtDNAHapTitle === null) {
                this.subject_haplo_group = null;
                this.subject_haplo_type = null;
            } else {
                this.subject_haplo_type = this.mtDNAHapTitle.slice(2);
                this.subject_haplo_group = this.mtDNAHapTitle.substr(0, 2);
            }  
            
            
        }
        if(!_.isEqual(this.haploData, this.inputHaploData)) {
            this.haploData = this.inputHaploData;
            var haplo_type_array = Array();

            this.haploData.forEach(item => {
                if(this.subject_haplo_type != null && this.subject_haplo_group != null) {
                    var item_halpo_type = item.mtDNATitle.slice(2);
                    if(this.subject_haplo_type === item_halpo_type) {
                        haplo_type_array.push(item);
                    }
                }
                
            });
            this.haplotypeData.data = haplo_type_array;
            this.loadingData = false;
        }
    }
}