import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Race } from '../_models/race.model';
import { RaceList } from '../_models/race-list.model';

@Injectable({
    providedIn: 'root'
})
export class RaceService {
    public httpOptions: any;
    _baseUrl: string;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this._baseUrl = baseUrl;
        this.httpOptions = {
            headers: new HttpHeaders(
                {
                    'Content-Type': 'application/json; charset=utf-8'
                    //'BrowserToken': 'auth_Token'
                }
            )
        }
    }


    updateRace(race: Race) {
        const requestUrl = `${this._baseUrl}api/races/${race.id}`;
        return this.http.put(requestUrl, race, this.httpOptions);
    }

    addRace(race: Race) {
        const requestUrl = `${this._baseUrl}api/races/`;
        return this.http.post(requestUrl, race, this.httpOptions);
    }


    deleteRace(raceId: number) {
        const requestUrl = `${this._baseUrl}api/races/${raceId}`;
        return this.http.delete(requestUrl, this.httpOptions);
    }

    getRace(raceId: number) {
        const requestUrl = `${this._baseUrl}api/races/${raceId}`;
        return this.http.get<Race>(requestUrl);
    }

    getRaces(filters: object, sort: string, order: string, page: number) {
        const query: string = JSON.stringify(filters);
        const requestUrl = `${this._baseUrl}api/races?q=${query}&sort=${sort}&order=${order}&page=${page + 1}`;
        return this.http.get<RaceList>(requestUrl);
    }

    checkHasResult(raceId: number) {
        const requestUrl = `${this._baseUrl}api/races/hasresult/${raceId}`;
        return this.http.get(requestUrl, this.httpOptions);
    }
}