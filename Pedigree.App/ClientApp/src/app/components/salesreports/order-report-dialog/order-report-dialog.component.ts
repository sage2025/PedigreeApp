import { Component, Inject, ElementRef } from "@angular/core";
import {formatDate} from '@angular/common';
import { MAT_DIALOG_DATA } from "@angular/material";
import { HorseHeirarchy } from "src/app/_models/horse-heirarchy.model";
import { HaploGroup } from "src/app/_models/haplo-group.model";
import { HorseService } from "src/app/_services/horse.service";
import { first } from "rxjs/operators";
import { MtDnaService } from "src/app/_services/mtdna.service";
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import htmlToPdfmake from 'html-to-pdfmake';
import html2canvas from 'html2canvas';
import { MLService } from "src/app/_services/ml.service";

export interface DialogData {
    maleHorseId: number,
    femaleHorseId: number,
    horseName: string,
    auctionDate: string,
    auctionName: string
}

@Component({
    selector: 'app-order-report-dialog',
    templateUrl: './order-report-dialog.component.html',
    styleUrls: ['./order-report-dialog.component.css']
})
export class OrderReportComponent {
    maleHorseId: number;
    femaleHorseId: number;
    treeData: HorseHeirarchy;
    loadingPopulationHaploGroups = false;
    populationHaploGroups: HaploGroup[] = [];
    loadingHorseHaploGroups = false;
    horseHaploGroups: HaploGroup[] = [];
    loadingChartData = false;
    horseName: string = "";
    broodmireName: string = "";
    sireName: string = "";
    sireCountry: string = "";
    damName: string = "";
    damCountry: string = "";
    date = formatDate(new Date(), 'yyyy/MM/dd', 'en')
    broodmire:HorseHeirarchy;
    grandsireOfBroodmire:HorseHeirarchy;
    greatGrandSire:HorseHeirarchy;
    mtDNATitle: string;
    subject_haplo_type: string;
    subject_haplo_group: string;
    greatGrandSireId:number;
    grandsireOfBroodmireId:number;
    haploData: any;
    mlScore: number;
    loadingData: boolean = true;
    auctionDate: string;
    auctionName: string;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private horseService: HorseService,
        private mtDNAService: MtDnaService,
        private mlService: MLService
    ) {
        this.maleHorseId = data.maleHorseId;
        this.femaleHorseId = data.femaleHorseId;
        this.horseName = data.horseName;
        this.auctionDate = data.auctionDate;
        this.auctionName = data.auctionName;
    }

    ngAfterViewInit() {
      this.horseService.getHypotheticalHorsHeirarchy(this.maleHorseId, this.femaleHorseId).pipe(first())
        .subscribe(data => {
          this.treeData = data;
          this.updateNamesForHeader(data);
          this.loadingChartData = false;
          this.broodmire = this.treeData.children[1].children[0];
    
          this.grandsireOfBroodmire = this.broodmire.children[0].children[0]; 

          this.greatGrandSire =  this.treeData.children[0].children[0];
          
          this.mtDNATitle = this.treeData.children[1].mtDNATitle;

          let searchService;
          searchService = this.horseService!.getHorsesForWildcard1Search(Number(this.greatGrandSire.id), Number(this.grandsireOfBroodmire.id));
          searchService
            .subscribe(data => {
              this.haploData = data;
            },
            err => {
                console.error(err);
            });
          
        }, error => {
          console.log(error);
        })
      
      this.loadingHorseHaploGroups = true;
      this.mtDNAService.getHypotheticalHorseHaploGroups(this.maleHorseId, this.femaleHorseId)
        .subscribe(
            data => {
              this.loadingHorseHaploGroups = false;
              this.horseHaploGroups = data;
            },
            error => {
              this.loadingHorseHaploGroups = false;
              console.error(error);
            }
        );

      this.loadingPopulationHaploGroups = true;
      this.mtDNAService.getHaploGroups()
        .subscribe(
            data => {
            this.loadingPopulationHaploGroups = false;
            this.populationHaploGroups = data;
            },
            error => {
              this.loadingPopulationHaploGroups = false;
              console.error(error);
            }
        );

      this.mlService.getLastModel()
        .subscribe(
            (mlModel: any) => {
              var mlFeatures = mlModel.features.split(", ");

              this.mlService.getHypotheticalMLScore(this.maleHorseId, this.femaleHorseId, mlFeatures, mlModel.id).subscribe(
                data => {
                  this.mlScore = data;
                  this.loadingData = false;
                },
                error => {
                  console.error(error);
                }
              );
            },
            error => {
                console.error(error);
            }
        )

    }

    updateNamesForHeader(data: HorseHeirarchy) {
        // the root data has no details so first childrens are the actual horses
        if(this.horseName == null) {
            this.horseName = data.name;
        }
        
        this.broodmireName = data.children[1].children[0].name;
        let sire = data.children.filter(m => m.sex == 'Male')[0];
        this.sireName = sire.name;
        this.sireCountry = sire.country;
        let dam = data.children.filter(m => m.sex == 'Female')[0];
        this.damName = dam.name;
        this.damCountry = dam.country;
    }

    downloadPdfFile() {
        let data = document.getElementById("graph"); 
        var table_html = document.getElementById('haplo_table').innerHTML;
        var dialog_html = '';
        dialog_html += document.getElementById('horse_title').innerHTML;
        html2canvas(data).then(canvas => {
          const contentDataURL = canvas.toDataURL('image/jpeg', 1.0)
          var img_html = `<div><img src="${contentDataURL}" style="width: 26cm; height: 16cm;"></div><br>`;
          dialog_html += img_html;
          dialog_html += table_html;
          var html = htmlToPdfmake(dialog_html);
          
          const documentDefinition = {
            content: [html],
            defaultStyle: {
              fontSize: 12,
              bold: true
            },
            pageOrientation: 'landscape'
          };

          let filename = 'order_report.pdf';
          // if (this.horseName != '(Unnamed)') filename = `${this.horseName}.pdf`;
          // else filename = `${this.sireName}(${this.sireCountry}) EX ${this.damName}(${this.damCountry}).pdf`;
          pdfMake.createPdf(documentDefinition).download(filename); 
        });  
      }
    
}