import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-stakes-population-viewport',
  templateUrl: './population-viewport.component.html',
  styleUrls: ['./population-viewport.component.css']
})
export class StakesPopulationViewportComponent implements OnInit {
  activeTab: number;
  constructor(private route: ActivatedRoute, private router: Router) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'current':
        this.activeTab = 0;
        break;
      case 'historical':
        this.activeTab = 1;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.activeTab = 0;
        this.router.navigate(['stakes/population/current']);
        break;
      case 1:
        this.activeTab = 1;
        this.router.navigate(['stakes/population/historical']);
        break;
    }
  }
}
