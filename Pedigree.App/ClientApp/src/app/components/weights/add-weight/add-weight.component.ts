import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-weight',
  templateUrl: './add-weight.component.html',
  styleUrls: ['./add-weight.component.scss']
})
export class AddWeightComponent implements OnInit {
  weightId: number;


  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.weightId = -1;
  }
}
