import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';

@Component({
  selector: 'app-ancestry-viewport',
  templateUrl: './viewport.component.html',
  styleUrls: ['./viewport.component.css']
})
export class AncestryViewportComponent implements OnInit {
  activeTab: number;
  constructor(
    private route: ActivatedRoute,
    private router: Router
    ) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'ancestor':
        this.activeTab = 0;
        break;
      case 'population':
        this.activeTab = 1;
        break;
    }
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.activeTab = 0;
        this.router.navigate(['ancestry/ancestor']);
        break;
      case 1:
        this.activeTab = 1;
        this.router.navigate(['ancestry/population']);
        break;
    }
  }
}
