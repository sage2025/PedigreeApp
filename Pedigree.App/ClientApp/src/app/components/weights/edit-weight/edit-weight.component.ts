import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-weight',
  templateUrl: './edit-weight.component.html',
  styleUrls: ['./edit-weight.component.scss']
})
export class EditWeightComponent implements OnInit {

  weightId: number;
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    var qParam = this.route.snapshot.paramMap.get("weightId");
    if (qParam != undefined) {
      this.weightId = Number(qParam);
    }
  }

}
