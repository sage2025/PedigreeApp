import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';
import { HaploGroup } from '../../../_models/haplo-group.model';
import { HorseService } from '../../../_services/horse.service';
import { MtDnaService } from '../../../_services/mtdna.service';
import { mergeMap } from 'rxjs/operators';
@Component({
  selector: 'app-hierarchy-view',
  templateUrl: './hierarchy-view.component.html',
  styleUrls: ['./hierarchy-view.component.css']
})
export class HierarchyViewComponent implements OnInit {
  horseId: number;
  currentHorse: HorseHeirarchy;
  treeData: HorseHeirarchy;
  loadingChartData = false;
  uniqueAncestorsCount: number = null;

  loadingPopulationHaploGroups = false;
  populationHaploGroups: HaploGroup[] = [];

  loadingHorseHaploGroups = false;
  horseHaploGroups: HaploGroup[] = [];

  constructor(
    private route: ActivatedRoute,
    private horseService: HorseService,
    private mtDNAService: MtDnaService,
  ) {

  }

  ngOnInit() {

    var qParam = this.route.snapshot.paramMap.get("horseId");
    if (qParam != undefined) {
      this.horseId = Number(qParam);
      this.loadingChartData = true;

      this.horseService.getHorsHeirarchy(this.horseId)
        .subscribe(data => {
          this.loadingChartData = false;
          this.treeData = data;
          this.updateCurrentHorse(data);
        }, error => {
          this.loadingChartData = false;
          console.log(error);
        });

      this.loadingHorseHaploGroups = true;
      this.mtDNAService.getHorseHaploGroups(this.horseId)
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

  updateCurrentHorse(data: HorseHeirarchy) {
    if (!data) return;
    this.currentHorse = data;
    var maleArray = data.children.filter(m => m.sex == 'Male');
    this.currentHorse.father = (maleArray.length > 0) ? maleArray[0].name : "";
    var femaleArray = data.children.filter(m => m.sex == 'Female');
    this.currentHorse.mother = (femaleArray.length > 0) ? femaleArray[0].name : "";
    var sireSideGrandParents = (femaleArray.length > 0) ? femaleArray[0].children : null;
    if (sireSideGrandParents != null) {
      var tripple = sireSideGrandParents.filter(m => m.sex == 'Male');
      if (tripple.length > 0) {
        this.currentHorse.sireOfDam = tripple[0].name;
      }
    }

  }

  update(data) {
    if (data.type == 'uniqueancestors') this.uniqueAncestorsCount = data.count;
  }
}
