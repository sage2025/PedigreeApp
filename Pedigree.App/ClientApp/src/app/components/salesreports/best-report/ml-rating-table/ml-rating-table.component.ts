import { Component, Inject, ViewChild } from "@angular/core";
import { MatSort, MatTableDataSource, MAT_DIALOG_DATA } from "@angular/material";
import { AuctionDetail } from "src/app/_models/auction-detail.model";
import { MLModel } from "src/app/_models/ml-model.model";
import { ReportDialogData } from "src/app/_models/report-dialog-data.model";
import { AuctionService } from "src/app/_services/auction.service";
import { HorseService } from "src/app/_services/horse.service";
import { MLService } from "src/app/_services/ml.service";
import { first } from 'rxjs/operators';

@Component({
    selector: 'app-ml-rating-table',
    templateUrl: './ml-rating-table.component.html',
    styleUrls: ['./ml-rating-table.component.css']
})
export class MlRatingTableComponent {
    auctionId: number;
    displayColumns: string[] = ['no', 'name', 'yearling', 'yob', 'sex', 'country', 'sire', 'dam', 'mtDNAHap', 'mlScore'];
    horseData = new MatTableDataSource<AuctionDetail>();
    loadingData = true;
    lastModel: MLModel;

    @ViewChild(MatSort, { static: false}) sort: MatSort;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ReportDialogData,
        private auctionService: AuctionService,
        private horseService: HorseService,
        private mlService: MLService,
    ) {
        this.auctionId = data.auctionId;
    }

    ngAfterViewInit() {
        this.horseData.sort = this.sort;
        this.mlService.getLastModel()
            .subscribe(
                data => {
                    this.lastModel = data;
                },
                error => {
                    console.error(error);
                }
            )
        
            
        var newData = Array();
        this.auctionService!.getAuctionDetail(this.auctionId)
            .subscribe(
                (data:any) => {
                    data.forEach(item => {
                      if(item.mlScore > 1) {
                        newData.push(item);
                      }
                      this.horseData.data = newData;
                      this.loadingData = false;
                    });
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
    }
}