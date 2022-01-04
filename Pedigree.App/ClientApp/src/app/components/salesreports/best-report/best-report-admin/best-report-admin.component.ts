import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA } from "@angular/material";
import { ReportDialogData } from "src/app/_models/report-dialog-data.model";
import { AuctionService } from "src/app/_services/auction.service";
import { HorseService } from "src/app/_services/horse.service";
import { MLService } from "src/app/_services/ml.service";
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import htmlToPdfmake from 'html-to-pdfmake';

@Component({
    selector: 'app-best-report',
    templateUrl: './best-report-admin.component.html',
    styleUrls: ['./best-report-admin.component.css']
})
export class BestReportComponent {
    auctionId: number;
    auctionDate: string;
    auctionName: string;
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ReportDialogData,
        private auctionService: AuctionService,
        private horseService: HorseService,
        private mlService: MLService,
    ) {
        this.auctionId = data.auctionId;
    }

    ngAfterViewInit(){
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
    }

    onPDFDownload() {
        var titleHtml = document.getElementById('title').innerHTML;
        var tableHtml = document.getElementById('tables');
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