import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-horse',
  templateUrl: './edit-horse.component.html',
  styleUrls: ['./edit-horse.component.css']
})
export class EditHorseComponent implements OnInit {

  horseId: number;
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    var qParam = this.route.snapshot.paramMap.get("horseId");
    if (qParam != undefined) {
      this.horseId = Number(qParam);
    }
  }

  updateHorse(data) {
    this.horseId = data.horseId;
  }

}
