import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-research-female-viewport',
  templateUrl: './female-viewport.component.html',
  styleUrls: ['./female-viewport.component.css']
})
export class ResearchFemaleViewportComponent implements OnInit {
  activeTab: number;
  constructor(
    private route: ActivatedRoute,
    private router: Router
    ) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'female_line_search':
        this.activeTab = 0;
        break;
      case 'family_stakes_search':
        this.activeTab = 1;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.activeTab = 0;
        this.router.navigate(['research/female/female_line_search']);
        break;
      case 1:
        this.activeTab = 1;
        this.router.navigate(['research/female/family_stakes_search']);
        break;
    }
  }
}
