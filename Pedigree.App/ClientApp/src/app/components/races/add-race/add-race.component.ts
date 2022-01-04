import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-race',
  templateUrl: './add-race.component.html',
  styleUrls: ['./add-race.component.scss']
})
export class AddRaceComponent implements OnInit {
  raceId: number;


  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.raceId = -1;
  }
}
