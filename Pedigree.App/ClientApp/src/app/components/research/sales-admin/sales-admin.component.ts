import {AfterViewInit, Component, ViewChild, Inject} from '@angular/core';
import { Router } from '@angular/router';
import {formatDate} from '@angular/common';
import { merge, of as observableOf, Subscription } from 'rxjs';
import {MatPaginator, MatSort, MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import {MatTableDataSource} from '@angular/material/table';
import { AuctionService } from 'src/app/_services/auction.service';

import {FormControl, FormGroupDirective, NgForm, Validators} from '@angular/forms';
import {ErrorStateMatcher} from '@angular/material/core';
import { Auction } from 'src/app/_models/auction.model';
import { ConfirmComponent } from 'src/app/_shared/confirm/confirm.component';

/** Error when invalid control is dirty, touched, or submitted. */
export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}

export interface DialogData {
    date: string;
    name: string;
  }

@Component({
  selector: 'app-sales-admin',
  templateUrl: './sales-admin.component.html',
  styleUrls: ['./sales-admin.component.css']
})

export class SalesAdmin implements AfterViewInit {
    displayedColumns: string[] = ['date', 'name', 'count', 'actions'];
    dataSource = new MatTableDataSource<Auction>();
    loadingData: boolean = true;
    private subscription: Subscription;
    
    closeResult: string;

    @ViewChild(MatPaginator, {static : false}) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    constructor(
        private auctionService: AuctionService,
        public dialog: MatDialog
    ) {

    }
    ngAfterViewInit() {
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        
        setTimeout(() => {
            this.loadData();
        })
    }

    loadData() {
        this.loadingData = true;
        this.subscription = this.auctionService!.getCurrentSales()
            .subscribe(
                (data:any) => {
                    this.dataSource.data = data;
                    this.loadingData = false;
                },
                error => {
                    console.error(error);
                    this.loadingData = false;
                })
        
    }

    onDelete(auction: any, index: number) {
        var name = auction.auctionName;
        var message = "Are you sure you want to delete this auction?";
        const deleteDialogRef = this.dialog.open(ConfirmComponent, {
            width: '450px',
            data: { name: name, message: message, showAlert: 1 }
        });

        deleteDialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.auctionService!.deleteAuction(auction.id)
                    .subscribe(
                        data => {
                            this.dataSource.data.splice(index, 1);
                            this.dataSource._updateChangeSubscription();
                        },
                        error => {
                            console.log(error);
                        }
                    )
            } else {
            //   this.deletingHorse[index] = false;
            }
        });
    }

    saleModal() {
        const dialogRef = this.dialog.open(SaleDialog, {
            width: '400px',
          });
      
          dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
          });
    }

}


@Component({
    selector: 'sales-dialog',
    templateUrl: './sales-dialog.component.html',
})
export class SaleDialog {

    saledate = "";
    salename = "";

    nameFormControl = new FormControl('', [
        Validators.required,
    ]);

    dateFormControl = new FormControl('', [
        Validators.required,
    ]);
    
    matcher = new MyErrorStateMatcher();

    constructor(        
        public dialogRef: MatDialogRef<SaleDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        public dialog: MatDialog
    ) {}

    onNoClick(): void {
        this.dialogRef.close();
    }

    saleCreate() {
        if(this.saledate != "" && this.salename != "") {
            this.dialogRef.close();
            const dialogRef = this.dialog.open(BulkConfirm, {
                data: {
                    date : this.saledate,
                    name : this.salename
                }
            });
        
            dialogRef.afterClosed().subscribe(result => {
                console.log('The dialog was closed');
            });
        }
    }

}

@Component({
    selector: 'sales-bulk-confirm',
    templateUrl: './sales-bulk-confirm.component.html'
})

export class BulkConfirm {

    constructor(
        public dialogRef: MatDialogRef<BulkConfirm>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private router: Router,
        private auctionService: AuctionService,
    ) {}

    onNoClick(): void {
        this.dialogRef.close();
    }

    onBulkAdd(date, name) {
        date = formatDate(date, 'yyyy-MM-dd', 'en')
        console.log(date)
        this.auctionService!.addAuction(date, name)
            .subscribe(
                data => {
                    this.dialogRef.close();
                    console.log(data)
                    var auctionId = data.id;
                    this.router.navigate([`research/admin/sales_admin/add/bulk/${auctionId}`]);
                },
                error => {
                    console.error(error);
                }
            )
    }
}
