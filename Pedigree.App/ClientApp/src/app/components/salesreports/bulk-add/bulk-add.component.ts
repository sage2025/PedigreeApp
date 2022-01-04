import { Component, OnInit } from "@angular/core";
import {MatDialog, MatSnackBar} from '@angular/material';
import {MatTableDataSource} from '@angular/material/table';
import { Horse } from 'src/app/_models/horse.model';
import { AuctionService } from "src/app/_services/auction.service";
import { ConfirmComponent } from 'src/app/_shared/confirm/confirm.component';
import { NotificationSnackComponent } from 'src/app/_shared/notification-snack/notification-snack.component';
import { ActivatedRoute } from '@angular/router';
import { AuctionDetail } from "src/app/_models/auction-detail.model";

@Component({
    selector: 'app-sales-bulk-add',
    templateUrl: './bulk-add.component.html',
    styleUrls: ['./bulk-add.component.css']
})

export class SalesAdminBulkAddComponent implements OnInit {
    displayColumns: string[] = ['no', 'name', 'type', 'yob', 'sex', 'country', 'sire', 'dam', 'mtDNAHap', 'state', 'actions'];
    horseData = new MatTableDataSource<AuctionDetail>();
    loadingData = false;
    auctionId: number;
    checkingHorse: object = {};
  
    constructor(
      public dialog: MatDialog,
      private _snackBar: MatSnackBar,
      private route: ActivatedRoute,
      private auctionService: AuctionService,
    ) { }
  
    ngOnInit() {
      var qParam = this.route.snapshot.paramMap.get("auctionId");
      if (qParam != undefined) {
        this.auctionId = Number(qParam);
      }
    }
  
    csvInputChange(fileInputEvent: any) {
      const me = this;
      const files = fileInputEvent.target.files;
      if (files.length > 0) {
        if (typeof (FileReader) != "undefined") {
          const reader = new FileReader();
  
          const data = [];
          me.loadingData = true;
          reader.onload = function (e) {
            const result = reader.result as string;
            const rows = result.split('\n');
            for (var i = 0; i < rows.length; i++) {
              if (rows[i]) {
                const cells = rows[i].split(',');
                
                if (cells.length == 9) {
                  const item = {
                    auctionId: me.auctionId,
                    number: parseInt(cells[0].trim()),
                    name: cells[1].trim(),
                    type: cells[2].trim(),
                    yob: parseInt(cells[3].trim()),
                    sex: cells[4].trim(),
                    country: cells[5].trim(),
                    sire: cells[6].trim(),
                    dam: cells[7].trim(),
                  };
      
                  data.push(item);
                }
                
              }
            }
            
            me.horseData.data = data;
            me.horseData._updateChangeSubscription();
            me.loadingData = false;
          }
          reader.readAsText(files[0]);
          
        } else {
          alert("This browser does not support HTML5.");
        }
      }
    }

    onSaveHorse(index: number) {
      var data = this.horseData.data[index];
      var horseData = {
        auctionId : this.auctionId,
        ...data,
      }
      
      this.checkingHorse[index] = true;
      this.auctionService!.addHorse(horseData.auctionId, horseData.number, horseData.name, horseData.type, horseData.yob, horseData.sex, horseData.country, horseData.sire, horseData.dam)
        .subscribe(
          data => {
            this.horseData.data[index].horseId = data['horseId'];
            this.horseData.data[index].sireId = data['sireId'];
            this.horseData.data[index].damId = data['damId'];
            this.horseData.data[index].pedigcomp = data['pedigcomp'];
            this.horseData.data[index].mtDNAHapId = data['mtDNA'];
            this.horseData.data[index].mtDNAColor = data['mtDNAColor'];
            this.horseData.data[index].mtDNATitle = data['mtDNATitle'];
            this.horseData.data[index].mlScore = data['mlScore'];
            
            this.horseData._updateChangeSubscription();
            this.checkingHorse[index] = false;      
          },
          error => {
            console.error(error);
            this.checkingHorse[index] = false;      
          }
        )
    }

    saleItems() {
      return this.horseData.data.filter(d => d.pedigcomp && d.pedigcomp >= 95);
    }

    onRefresh(row: AuctionDetail, index: number) {
      this.auctionService!.checkPedigComp(row.horseId)
        .subscribe(
          (data:number) => {
            console.log(data);
            this.horseData.data[index].pedigcomp = data;
            this.horseData._updateChangeSubscription();
          },
          error => {
            console.log(error);
          }
        )
    }
  
    onDelete(horse: Horse, index: number) {
      if(horse.id) {
        var name = horse.name;
        var message = "Are you sure you want to delete this horse?";
        const deleteDialogRef = this.dialog.open(ConfirmComponent, {
            width: '450px',
            data: { name: name, message: message, showAlert: 1 }
        });

        deleteDialogRef.afterClosed().subscribe(result => {
            if (result) {
              this.auctionService!.deleteAuctionDetail(horse.id)
                .subscribe(
                  data => {
                    this.horseData.data.splice(index, 1);
                    this.horseData._updateChangeSubscription();
                    if(this.horseData.data.length === 0) {
                      this.checkAllSaves();
                    }
                  },
                  error => {
                    console.log(error);
                  }
                )
            } else {
            //   this.deletingHorse[index] = false;
            }
        });
        
      } else {
        this.horseData.data.splice(index, 1);
        this.horseData._updateChangeSubscription();
        this.checkAllSaves();
      }
    }

    onSale() {
      this.auctionService!.addAuctionDetails(this.saleItems())
        .subscribe(
          data => {            
            this._snackBar.openFromComponent(NotificationSnackComponent, {
              duration: 5000,
              data: { message: 'Successfully saved.' } // 2 stands for success 1 for error
            });
          },
          error => {
            console.error(error);
          }
        )
    }
  
    checkAllSaves() {
      if (this.horseData.data.every(d => d.id != undefined)) {
        this._snackBar.openFromComponent(NotificationSnackComponent, {
          duration: 5000,
          data: { message: 'All records added' } // 2 stands for success 1 for error
        });
      }
    }
  }