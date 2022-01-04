import { Component, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Horse } from 'src/app/_models/horse.model';
import { HorseService } from 'src/app/_services/horse.service';

@Component({
    selector: 'app-research-family-stakes-search',
    templateUrl: './family-stakes-search.component.html',
    styleUrls: ['./family-stakes-search.component.css']
})

export class FamilyStakesSearchComponent implements AfterViewInit {
    displayedColumns: string[] = ['pedigree', 'name', 'sex', 'age', 'fatherName', 'motherName', 'bestRaceClass', 'action'];
    horseData = new MatTableDataSource<Horse>();

    horse: Horse = null;
    private subscription: Subscription;
    loading = false;
    showSearchClear: boolean = false;

    generation: number = 4;
    generationArray = [];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private horseService: HorseService
    ) {
        this.setGenerations();

        if (this.route.snapshot.params.tab == 'family_stakes_search') {
            const params = this.route.snapshot.queryParams;

            this.generation = params['gen'] ? parseInt(params['gen']) : 4;
            if (params['femaleId']) this.loadHorse(params['femaleId']);
        }
    }

    ngAfterViewInit() {

    }

    setGenerations() {
        var tempArray = [];
        for (var i = 1; i <= 8; i++) {
            tempArray.push(i);
        }
        this.generationArray = tempArray;
    }

    enableSearch() {
        return this.horse && this.horse.id;
    }

    search() {
        if (!this.enableSearch()) return;

        this.router.navigate(['research/female/family_stakes_search'], { queryParams: { femaleId: this.horse.id, gen: this.generation } });
        this.loadData();
    }

    clearSearchBox() {
        this.horse = null;
        this.horseData.data = [];
        this.showSearchClear = false;
    }

    loadHorse(horseId) {
        this.horseService.getHorse(horseId).pipe(first())
            .subscribe(result => {
                this.horse = result;
                this.loadData();
            });
    }

    loadData() {
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;

        this.subscription = this.horseService!.getHorsesForFamilyStakeSearch(this.horse.id, this.generation)
            .subscribe(data => {
                this.loading = false;
                this.horseData.data = data;
                this.showSearchClear = true;
            },
                err => {
                    this.loading = false;
                    console.error(err);
                    this.showSearchClear = true;
                });
    }

    toggleRaceBubble(tooltip, horse: Horse) {
        if (tooltip.isOpen()) {
            tooltip.close();
        } else {
            this.horseService!.getRacesForHorse(horse.id, 'Date', 'DESC')
                .subscribe(data => {
                    tooltip.open({
                        name: horse.name,
                        races: data
                    });
                })
        }
    }
}