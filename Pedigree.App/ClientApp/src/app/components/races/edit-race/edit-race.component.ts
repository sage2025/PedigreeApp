import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-race',
  templateUrl: './edit-race.component.html',
  styleUrls: ['./edit-race.component.scss']
})
export class EditRaceComponent implements OnInit {

  raceId: number;
  constructor(private route: ActivatedRoute, 
    private router: Router,
  ) { }

  ngOnInit() {
    var qParam = this.route.snapshot.paramMap.get("raceId");
    if (qParam != undefined) {
      this.raceId = Number(qParam);
    }
  }

  onView(raceId) {
      this.router.navigate(['positions'], {queryParams: {raceId}});
  }

}
