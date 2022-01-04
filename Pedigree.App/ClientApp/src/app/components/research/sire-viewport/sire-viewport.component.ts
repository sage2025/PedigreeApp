import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-research-sire-viewport',
  templateUrl: './sire-viewport.component.html',
  styleUrls: ['./sire-viewport.component.css']
})
export class ResearchSireViewportComponent implements OnInit {
  activeTab: number;
  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'sire_search':
        this.activeTab = 0;
        break;
      case 'sire_broodmare_search':
        this.activeTab = 1;
        break;
      case 'sireline_search':
        this.activeTab = 2;
        break;
      case 'sire_broodmare_crosses':
        this.activeTab = 3;
        break;
      case 'mtdna_lookup':
        this.activeTab = 4;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.activeTab = 0;
        this.router.navigate(['research/sire/sire_search']);
        break;
      case 1:
        this.activeTab = 1;
        this.router.navigate(['research/sire/sire_broodmare_search']);
        break;
      case 2:
        this.activeTab = 2;
        this.router.navigate(['research/sire/sireline_search']);
        break;
      case 3:
        this.activeTab = 3;
        this.router.navigate(['research/sire/sire_broodmare_crosses']);
        break;
      case 4:
        this.activeTab = 4;
        this.router.navigate(['research/sire/mtdna_lookup']);
        break;
    }
  }
}
