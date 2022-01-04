import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
    selector: 'app-sales-reports',
    templateUrl: './sales-reports.component.html',
    styleUrls:['./sales-reports.component.css']
})
export class SalesReportsComponent implements OnInit {
activeTab: number;
  constructor(private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'active_sales':
        this.activeTab = 0;
        break;
      case 'sales_summary_reports':
        this.activeTab = 1;
        break;
      case 'best_sales_reports':
        this.activeTab = 2;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.router.navigate(['sales_reports/active_sales']);
        break;
      case 1:
        this.router.navigate(['sales_reports/sales_summary_reports']);
        break;
      case 2:
        this.router.navigate(['sales_reports/best_sales_reports']);
        break;
    }
  }
}