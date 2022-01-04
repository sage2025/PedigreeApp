import { Component, OnInit } from '@angular/core';
import { TreeData } from '../../../_models/tree-data.model';
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';
import { HorseService } from '../../../_services/horse.service';
import { ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-heirarchy-tree',
  templateUrl: './heirarchy-tree.component.html',
  styleUrls: ['./heirarchy-tree.component.css']
})
export class HeirarchyTreeComponent implements OnInit {
  isLoadingResults = true;
  rootElement: HorseHeirarchy = new HorseHeirarchy();
  horseId: number;
  list: HorseHeirarchy[] = [];
  constructor(private horseService: HorseService, private route: ActivatedRoute) {}

  ngOnInit() {
    var qParam = this.route.snapshot.paramMap.get("horseId");
    if (qParam != undefined) {
      this.horseId = Number(qParam);
    }
  }

  ngAfterViewInit() {
    this.loadHeirarchy();
  }

  loadHeirarchy() {
    this.horseService.getHorsHeirarchy(this.horseId).pipe(first())
      .subscribe(data => {
        this.rootElement = data;
        this.list = data.children;
        this.isLoadingResults = false;
      }, error => {
          console.log(error);
      })
  }

  showTooltip(data: HorseHeirarchy) {
    return data.sex + "(" + data.age + ")";
  }

}
