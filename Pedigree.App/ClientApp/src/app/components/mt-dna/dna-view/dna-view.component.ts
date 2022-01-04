import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatTabChangeEvent } from '@angular/material';
import { MtDnaService } from '../../../_services/mtdna.service';
import { HaploGroup } from 'src/app/_models/haplo-group.model';

@Component({
  selector: 'app-dna-view',
  templateUrl: './dna-view.component.html',
  styleUrls: ['./dna-view.component.css']
})
export class DNAViewComponent implements OnInit {
  activeTab: number;
  loadingData: boolean = false;
  haploGroups: HaploGroup[] = [];
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private mtDnaService: MtDnaService,) {

  }

  ngOnInit() {
    var tab = this.route.snapshot.paramMap.get("tab");
    switch (tab) {
      case 'founders':
        this.activeTab = 0;
        break;
      case 'flags':
        this.activeTab = 1;
        break;
      case 'admin':
        this.activeTab = 2;
        break;
      case 'population':
        this.activeTab = 3;
        break;
      case 'stallion':
        this.activeTab = 4;
        break;
      case 'distance':
        this.activeTab = 5;
        break;
    }

    setTimeout(() => {
      this.loadData();
    })
  }

  onTabChanged(event: MatTabChangeEvent) {
    switch (event.index) {
      case 0:
        this.router.navigate(['mtdna/founders']);
        break;
        break;
      case 1:
        this.router.navigate(['mtdna/flags']);
        break;
      case 2:
        this.router.navigate(['mtdna/admin']);
        break;
      case 3:
        this.router.navigate(['mtdna/population']);
        break;
      case 4:
        this.router.navigate(['mtdna/stallion']);
        break;
      case 5:
        this.router.navigate(['mtdna/distance']);
        break;
    }
  }

  loadData() {
    this.loadingData = true;
    this.mtDnaService!.getHaploGroups()
      .subscribe(
        data => {
          this.loadingData = false;
          this.haploGroups = data;
        },
        error => {
          this.loadingData = false;
          console.error(error);
        })
  }
}
