import { AfterViewInit, Component, Inject, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA,  MatTableDataSource, MatSort } from "@angular/material";
import { ReportDialogData } from "src/app/_models/report-dialog-data.model";
import { AuctionService } from "src/app/_services/auction.service";
import { AuctionDetail } from "src/app/_models/auction-detail.model";
import { HorseService } from "src/app/_services/horse.service";
import { first } from 'rxjs/operators';
import { MLService } from "src/app/_services/ml.service";
import { MLModel } from "src/app/_models/ml-model.model";
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import htmlToPdfmake from 'html-to-pdfmake';

@Component({
    selector: 'app-summary-report',
    templateUrl: './summary-report.component.html',
    styleUrls: ['./summary-report.component.css']
})
export class SummaryReportComponent {
    auctionId: number;
    auctionDate: string;
    auctionName: string;
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
        this.auctionService!.getAuction(this.auctionId)
            .subscribe(
                data => {
                    this.auctionDate = data['auctionDate'];
                    this.auctionName = data['auctionName'];
                },
                error => {
                    console.log(error);
                }
            )
            
        var newData = Array();
        this.auctionService!.getAuctionDetail(this.auctionId)
            .subscribe(
                (data:any) => {
                    // data.forEach(item => {
                    //     this.horseService.getHypotheticalHorsHeirarchy(Number(item.sireId), Number(item.damId)).pipe(first())
                    //         .subscribe(
                    //             data => {
                    //                 if(data['ahc'] === null) {
                    //                     var ahc = "0";
                    //                   } else {
                    //                     var ahc = `${data['ahc'].ahc}`;
                    //                   }
                    //                   if(data['bal'] === null) {
                    //                     var bal = "0";
                    //                   } else {
                    //                     var bal = `${data['bal']}`;
                    //                   }
                    //                   if(data['kal'] === null) {
                    //                     var kal = "0";
                    //                   } else {
                    //                     var kal = `${data['kal']}`;
                    //                   }
                    //                   if(data['coi'] == null) {
                    //                     var coi = "0";
                    //                   } else {
                    //                     var coi = `${data['coi']}`;
                    //                   }
                                      
                    //                   var inputDataForMl = {MtDNAGroups: `${data.id}`, AHC:ahc, Bal:bal, Kal:kal, COI:"0.2"};
                            
                    //                   this.mlService.evaluateModel(this.lastModel.id, inputDataForMl)
                    //                           .subscribe(
                    //                               (data: any) => {
                    //                                 item.mlScore  = data;
                    //                                 newData.push(item);
                    //                                 this.horseData.data = newData;
                    //                                 this.loadingData = false;
                    //                               },
                    //                               error => {
                    //                                 console.log(error);
                    //                               }
                    //                           );
                    //             },
                    //             error => {
                    //                 console.log(error);
                    //             }
                    //         )
                    // });
                  this.horseData.data = data
                  this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
    }

    onPDFDownload() {
        var titleHtml = document.getElementById('title').innerHTML;
        var tableHtml = document.getElementById('horseList');
        tableHtml.style.width = '26cm';
        var html = titleHtml + tableHtml.innerHTML;
        var content = htmlToPdfmake(html);
        const documentDefinition = {
            content: [content],
            defaultStyle: {
              fontSize: 12,
              bold: true
            },
            pageOrientation: 'landscape'
          };
        var filename = `${this.auctionDate}-${this.auctionName}.pdf`;
        pdfMake.createPdf(documentDefinition).download(filename); 
    }
}