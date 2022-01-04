import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import {formatDate} from '@angular/common';
import { HttpEvent } from '@angular/common/http';
import { MatDialog } from '@angular/material';
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';
import { ActivatedRoute } from '@angular/router';
import { HorseService } from '../../../_services/horse.service';
import { MtDnaService } from '../../../_services/mtdna.service';
import { HaploGroup } from '../../../_models/haplo-group.model';
import { first } from 'rxjs/operators';
import { MLService } from 'src/app/_services/ml.service';
import { Subscription } from 'rxjs';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
pdfMake.vfs = pdfFonts.pdfMake.vfs;
import { OrderReportComponent } from '../../salesreports/order-report-dialog/order-report-dialog.component';

@Component({
  selector: 'app-hypothetical-hierarchy-view',
  templateUrl: './hypothetical-hierarchy-view.component.html',
  styleUrls: ['./hypothetical-hierarchy-view.component.css']
})


export class HypotheticalHierarchyViewComponent implements OnInit {
  @ViewChild('reportContent', {static : false}) reportContent: ElementRef;

  private subscription: Subscription;
  
  theDataSource: HttpEvent<any>;

  treeData: HorseHeirarchy;
  
  maleHorseId: number;
  femaleHorseId: number;
  sireName: string = "";
  sireCountry: string = "";
  damName: string = "";
  damCountry: string = "";
  mtDNATitle: string = "";
  loadingChartData = false;
  uniqueAncestorsCount: number = null;

  loadingPopulationHaploGroups = false;
  populationHaploGroups: HaploGroup[] = [];

  loadingHorseHaploGroups = false;
  horseHaploGroups: HaploGroup[] = [];

  mlScore:number;
  broodmire:HorseHeirarchy;
  grandsireOfBroodmire:HorseHeirarchy;
  greatGrandSire:HorseHeirarchy;

  greatGrandSireId:number;
  grandsireOfBroodmireId:number;

  closeResult: string;

  constructor(
    public dialog: MatDialog,
    private route: ActivatedRoute,
    private horseService: HorseService,
    private mtDNAService: MtDnaService,
    private mlService: MLService,
    ) {

  }

  message: string;
  progress: number;
  documentid :number = 2;
  date = formatDate(new Date(), 'yyyy/MM/dd', 'en')

  ngOnInit() {
    var maleqParam = this.route.snapshot.queryParamMap.get('malehorseId');
    var femaleqParam = this.route.snapshot.queryParamMap.get('femalehorseId');
    
    if (maleqParam != undefined && femaleqParam != undefined) {
      this.maleHorseId = Number(maleqParam);
      this.femaleHorseId = Number(femaleqParam);
      // Call API to get data
      
      this.horseService.getHypotheticalHorsHeirarchy(Number(maleqParam), Number(femaleqParam)).pipe(first())
        .subscribe(data => {
          this.treeData = data;
          this.updateNamesForHeader(data);
          this.loadingChartData = false;
          this.broodmire = this.treeData.children[1].children[0];
    
          this.grandsireOfBroodmire = this.broodmire.children[0].children[0]; 

          this.greatGrandSire =  this.treeData.children[0].children[0];
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
    }

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
  }

  updateNamesForHeader(data: HorseHeirarchy) {
    // the root data has no details so first childrens are the actual horses
    let sire = data.children.filter(m => m.sex == 'Male')[0];
    this.sireName = sire.name;
    this.sireCountry = sire.country;
    let dam = data.children.filter(m => m.sex == 'Female')[0];
    this.damName = dam.name;
    this.damCountry = dam.country;
  }

  update(data) {
    if (data.type == 'uniqueancestors') this.uniqueAncestorsCount = data.count;
  }

  reportDialog() {
    const dialogRef = this.dialog.open(OrderReportComponent, {
      data: {
          maleHorseId : this.maleHorseId,
          femaleHorseId: this.femaleHorseId
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

