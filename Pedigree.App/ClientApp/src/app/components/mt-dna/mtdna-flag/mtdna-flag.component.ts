import { Component, AfterViewInit } from '@angular/core';
import { MatDialog, MatTableDataSource } from '@angular/material';
import { Horse } from 'src/app/_models/horse.model';
import { MtDNAFlag } from 'src/app/_models/mtdna-flag.model';
import { MtDnaService } from 'src/app/_services/mtdna.service';
import { ConfirmComponent } from 'src/app/_shared/confirm/confirm.component';

@Component({
    selector: 'app-mtdna-flag',
    templateUrl: './mtdna-flag.component.html',
    styleUrls: ['./mtdna-flag.component.css']
})

export class MtDNAFlagComponent implements AfterViewInit {
    startHorse: Horse;
    endHorse: Horse;

    displayedColumns: string[] = ['no', 'start_horse', 'end_horse', 'actions'];
    dataSource = new MatTableDataSource<any>();
    loadingData: boolean = false;
    adding: boolean = false;

    deletingFlag: object = {};
    updatingFlag: object = {};
    
    constructor(
        public dialog: MatDialog,
        private mtDNAService: MtDnaService
    ) {
        
    }

    ngOnInit() {
        this.loadData();
    }
    ngAfterViewInit() {

    }

    loadData() {
        this.loadingData = true;
        this.mtDNAService.getMtDNAFlags()
            .subscribe(
                data => {
                    this.dataSource.data = data;
                    this.loadingData = false;
                }, 
                error => {
                    console.error(error);
                    this.loadingData = false;
                }
            )
    }
    onAdd() {
        if (!this.startHorse) return;

        this.adding = true;
        this.mtDNAService.addMtDNAFlags(this.startHorse.oId, this.endHorse ? this.endHorse.oId : null)
            .subscribe(
                (data) => {
                    this.adding = false;
                    this.dataSource.data.push({
                        id: data,
                        startHorseName: this.startHorse.name,
                        startHorseAge: this.startHorse.age,
                        startHorseCountry: this.startHorse.country,                        
                        endHorseName: this.endHorse.name,
                        endHorseAge: this.endHorse.age,
                        endHorseCountry: this.endHorse.country,
                    });
                    this.dataSource._updateChangeSubscription();
                },
                error => {
                    this.adding = false;
                    console.error(error);
                }
            )
    }

    onDelete(flag: any, index: number) {
        this.deletingFlag[index] = true;

        const deleteDialogRef = this.dialog.open(ConfirmComponent, {
            width: '350px',
            data: { message: 'Are you sure you want to delete this flags?', showAlert: 1 }
          });

          deleteDialogRef.afterClosed().subscribe(result => {
            if (result) {
          
                this.mtDNAService.deleteMtDNAFlags(flag.id)
                    .subscribe(
                        data => {
                            this.deletingFlag[index] = false;
                            this.dataSource.data.splice(index, 1);
                            this.dataSource._updateChangeSubscription();
                        },
                        error => {
                            this.deletingFlag[index] = false;
                            console.error(error);
                        }
                    );
            } else {
              this.deletingFlag[index] = false;
            }
          });
    }
}