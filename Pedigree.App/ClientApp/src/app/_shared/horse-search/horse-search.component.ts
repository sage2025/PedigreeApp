import _ from 'lodash';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime, tap, switchMap, finalize } from 'rxjs/operators';
import { HorseService } from '../../_services/horse.service';
import { Horse } from '../../_models/horse.model';
@Component({
  selector: 'app-horse-search',
  templateUrl: './horse-search.component.html',
  styleUrls: ['./horse-search.component.css']
})
export class HorseSearchComponent implements OnInit {

  searchHorsesCtrl = new FormControl();
  filteredHorses: Horse[];
  isLoading = false;
  errorMsg: string = null;

  @Input() label: string;
  @Input() sex: string;
  @Input() disabled: boolean;

  @Input() value!: Horse;
  @Output() valueChange = new EventEmitter<Horse>();

  constructor(private horseService: HorseService) { }

  ngOnInit() {
    this.searchHorsesCtrl.valueChanges
      .pipe(
        debounceTime(500),
        tap(() => {
          this.isLoading = true;
          this.filteredHorses = [];
        }),
        switchMap(value => this.horseService.searchHorseEx(value, this.sex)
          .pipe(
            finalize(() => {
              this.isLoading = false;
              if (!_.isEqual(this.value, this.searchHorsesCtrl.value)) {
                this.value = this.searchHorsesCtrl.value;
                this.valueChange.emit(this.value);
              }
            })
          )
        )
      ).subscribe(data => {
        if (data == undefined || data == null || data.length == 0) {
          this.errorMsg = "No matches";
          this.filteredHorses = [];
        }
        else {
          this.errorMsg = null;
          this.filteredHorses = this.sex != null ? data.filter(x => x.sex == this.sex) : data;
        }
      },
        error => {
          this.errorMsg = error.error;
        }
      );
  }

  ngOnChanges() {
    if (!this.value) this.searchHorsesCtrl.setValue(null);
    else if (this.value.id) this.searchHorsesCtrl.setValue(this.value);
  }
  displayHorse(horse: Horse): string {
    return horse && horse.name ? horse.name : '';
  }
}
