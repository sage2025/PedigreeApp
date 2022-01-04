import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-horse',
  templateUrl: './add-horse.component.html',
  styleUrls: ['./add-horse.component.css']
})
export class AddHorseComponent implements OnInit {
  horseId: number;


  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.horseId = -1;
  }
}
