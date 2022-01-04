import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-stallion-ratings-view',
  templateUrl: './stallion-ratings-view.component.html',
  styleUrls: ['./stallion-ratings-view.component.css']
})
export class StallionRatingsViewComponent implements OnInit {
  activeTab: number;
  loadingData: boolean = false;
  constructor(
    private route: ActivatedRoute,
    private router: Router,) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'current-sr':
        this.activeTab = 0;
        break;
      case 'historical-sr':
        this.activeTab = 1;
        break;
      case 'current-bms-sr':
        this.activeTab = 2;
        break;
      case 'historical-bms-sr':
        this.activeTab = 3;
        break;
      case 'current-sos-sr':
        this.activeTab = 4;
        break;
      case 'historical-sos-sr':
        this.activeTab = 5;
        break;
      case 'current-bmsos-sr':
        this.activeTab = 6;
        break;
      case 'historical-bmsos-sr':
        this.activeTab = 7;
        break;
    }

    setTimeout(() => {
      this.loadData();
    })
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.router.navigate(['stallion-ratings/current-sr']);
        break;
      case 1:
        this.router.navigate(['stallion-ratings/historical-sr']);
        break;
      case 2:
        this.router.navigate(['stallion-ratings/current-bms-sr']);
        break;
      case 3:
        this.router.navigate(['stallion-ratings/historical-bms-sr']);
        break;
      case 4:
        this.router.navigate(['stallion-ratings/current-sos-sr']);
        break;
      case 5:
        this.router.navigate(['stallion-ratings/historical-sos-sr']);
        break;
      case 6:
        this.router.navigate(['stallion-ratings/current-bmsos-sr']);
        break;
      case 7:
        this.router.navigate(['stallion-ratings/historical-bmsos-sr']);
        break;
    }
  }

  loadData() {
    // this.loadingData = true;
    // this.mtDnaService!.getHaploGroups()
    //   .subscribe(
    //     data => {
    //       this.loadingData = false;
    //       this.haploGroups = data;
    //     },
    //     error => {
    //       this.loadingData = false;
    //       console.error(error);
    //     })
  }
}
