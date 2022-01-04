import { Component, ViewChild, HostListener, OnInit } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { NavItem } from './nav-item';
import { NavService } from './nav.service';

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.scss'],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';

  opened = true;
  
  navItems: NavItem[] = [{
    displayName: 'Horses',
    iconName: 'dashboard',
    route: '/'
  }, {
    displayName: 'Stakes',
    iconName: '360',
    children: [{
      displayName: 'Races',
      iconName: '360',
      route: 'races'
    }, {
      displayName: 'Weights',
      iconName: '360',
      route: 'weights'
    }, {
      displayName: 'Population',
      iconName: 'bar_chart',
      route: 'stakes/population/current'
    }]
  }, {
    displayName: 'Sales Reports',
    iconName: 'gavel',
    route: '/sales_reports/active_sales'
  }, {
    displayName: 'Stallion Ratings',
    iconName: 'poll',
    route: 'stallion-ratings/current-sr'
  }, {
    displayName: 'Research',
    iconName: 'library_books',
    children: [{
      displayName: 'Sire/Broodmare Sire',
      iconName: 'group',
      route: 'research/sire/sire_search'
    }, {
      displayName: 'Wildcard Queries',
      iconName: 'query_builder',
      route: 'research/wildcard/wildcard_search'
    }, {
      displayName: 'Female Line',
      iconName: 'show_chart',
      route: 'research/female/female_line_search'
    }]
  }, {
    displayName: 'mtDNA',
    iconName: 'device_hub',
    route: 'mtdna/founders'
  }, {
    displayName: 'ANCESTRY',
    iconName: 'people',
    route: 'ancestry/ancestor'
  }, {
    displayName: 'Machine Learning',
    iconName: 'memory',
    route: 'ml'
  }, {
    displayName: 'Admin',
    iconName: 'settings',
    route: 'research/admin/sales_admin'
  }];

  @ViewChild('sidenav', { static: true }) sidenav: MatSidenav;
  constructor(private navService: NavService) { }

  ngOnInit() {
    if (window.innerWidth < 768) {
      this.sidenav.fixedTopGap = 55;
      this.opened = false;
    } else {
      this.sidenav.fixedTopGap = 55;
      this.opened = true;
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    if (event.target.innerWidth < 768) {
      this.sidenav.fixedTopGap = 55;
      this.opened = false;
    } else {
      this.sidenav.fixedTopGap = 55
      this.opened = true;
    }
  }

  isBiggerScreen() {
    const width = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
    if (width < 768) {
      return true;
    } else {
      return false;
    }
  }

}
