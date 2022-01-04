import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-research-admin-viewport',
  templateUrl: './admin-viewport.component.html',
  styleUrls: ['./admin-viewport.component.css']
})
export class ResearchAdminViewportComponent implements OnInit {
  activeTab: number;
  constructor(private route: ActivatedRoute, private router: Router) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'sales_admin':
        this.activeTab = 0;
        break;
      case 'ten_generations':
        this.activeTab = 1;
        break;
      case 'twin':
        this.activeTab = 2;
        break;
      case 'grandparents':
        this.activeTab = 3;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.router.navigate(['research/admin/sales_admin']);
        break;
      case 1:
        this.router.navigate(['research/admin/ten_generations']);
        break;
      case 2:
        this.router.navigate(['research/admin/twin']);
        break;
      case 3:
        this.router.navigate(['research/admin/grandparents']);
        break;
    }
  }
}
