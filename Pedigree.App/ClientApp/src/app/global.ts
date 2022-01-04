import { Injectable } from '@angular/core';
import { HaploGroup } from './_models/haplo-group.model';

Injectable()
export class Globals{
    private _haploGroups: HaploGroup[];

    setHaploGroups(haploGroups: HaploGroup[]) {
        this._haploGroups = haploGroups;
    }

    getHaploGroups() {
        return this._haploGroups;
    }
}