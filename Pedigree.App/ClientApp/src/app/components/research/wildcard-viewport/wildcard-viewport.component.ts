import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-research-wildcard-viewport',
  templateUrl: './wildcard-viewport.component.html',
  styleUrls: ['./wildcard-viewport.component.css']
})
export class ResearchWildcardViewportComponent implements OnInit {
  activeTab: number;
  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'wildcard_search':
        this.activeTab = 0;
        break;
      case 'query_position':
        this.activeTab = 1;
        break;
      case 'inbreeding':
        this.activeTab = 2;
        break;
      case 'common_ancestors':
        this.activeTab = 3;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.activeTab = 0;
        this.router.navigate(['research/wildcard/wildcard_search']);
        break;
      case 1:
        this.activeTab = 1;
        this.router.navigate(['research/wildcard/query_position']);
        break;
      case 2:
        this.activeTab = 2;
        this.router.navigate(['research/wildcard/inbreeding']);
        break;
      case 3:
        this.activeTab = 3;
        this.router.navigate(['research/wildcard/common_ancestors']);
        break;
    }
  }
}
