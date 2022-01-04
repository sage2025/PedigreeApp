import { AfterViewInit, Component, ViewChild } from "@angular/core";
import { MatTableDataSource, MatPaginator, MatSort, MatSnackBar, MatDialog } from "@angular/material";
import { Subscription } from "rxjs";
import { Auction } from "src/app/_models/auction.model";
import { AuctionService } from "src/app/_services/auction.service";
import { AuctionDetail } from "src/app/_models/auction-detail.model";
import { SummaryReportComponent } from "../summary-report/summary-report.component";
import { BestReportComponent } from "../best-report/best-report-admin/best-report-admin.component";
import { OrderReportComponent } from "../order-report-dialog/order-report-dialog.component";

@Component({
    selector: 'app-active-sales',
    templateUrl: './active-sales.component.html',
    styleUrls: ['./active-sales.component.css']
})

export class ActiveSalesComponent implements AfterViewInit{
    saleData : Auction[];
    loadingData: boolean = true;
    private subscription: Subscription;
    displayColumns: string[] = ['no', 'name', 'yearling', 'yob', 'sex', 'country', 'sire', 'dam', 'mtDNAHap', 'order_report'];
    horseData = new MatTableDataSource<AuctionDetail>();
    auctionId: number;
    auctionDate: string;
    auctionName: string;
    closeResult: string;
    isReportAvailable: boolean = false;

    @ViewChild(MatPaginator, {static : false}) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private auctionService: AuctionService,
        public dialog: MatDialog,
    ) {

    }
    ngAfterViewInit() {        
        this.horseData.sort = this.sort;
        setTimeout(() => {
            this.loadData();
        })
    }

    loadData() {
        this.loadingData = true;
        this.subscription = this.auctionService!.getCurrentSales()
            .subscribe(
                (data:any) => {
                    this.saleData = data;
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
        
    }

    onGetAuctionDetail($event, auctionId:number) {
        this.loadingData = true;
        this.isReportAvailable = true;
        this.auctionService!.getAuction(auctionId)
            .subscribe(
                data => {
                    this.auctionDate = data['auctionDate'];
                    this.auctionName = data['auctionName'];
                },
                error => {
                    console.log(error);
                }
            )
        this.auctionService!.getAuctionDetail(auctionId)
            .subscribe(
                (data:any) => {
                    this.horseData.data = data;
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
    }

    onSummaryReport() {
        
        const dialogRef = this.dialog.open(SummaryReportComponent, {
            data: {
                auctionId: this.auctionId
            },
            width: '85%'
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log(`Dialog result: ${result}`);
        });
    }

    onBestReport() {
        const dialogRef = this.dialog.open(BestReportComponent, {
            data: {
                auctionId: this.auctionId
            },
            width: '85%'
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log(`Dialog result: ${result}`);
        });
    }

    onOrderReport(maleHorseId, femaleHorseId, horseName, auctionDate, auctionName) {
        const dialogRef = this.dialog.open(OrderReportComponent, {
            data: {
                maleHorseId : maleHorseId,
                femaleHorseId: femaleHorseId,
                horseName: horseName,
                auctionDate: auctionDate,
                auctionName: auctionName
            },
            width: '85%',
            autoFocus: false,
            maxHeight: '90vh' //you can adjust the value as per your view
          });
      
          dialogRef.afterClosed().subscribe(result => {
              console.log(`Dialog result: ${result}`);
          });
    }
}