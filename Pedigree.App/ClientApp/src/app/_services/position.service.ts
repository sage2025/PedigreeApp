import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Position } from '../_models/position.model';
import { PositionList } from '../_models/position-list.model';

@Injectable({
    providedIn: 'root'
})
export class PositionService {
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


    updatePosition(position: Position) {
        const requestUrl = `${this._baseUrl}api/positions/${position.id}`;
        return this.http.put(requestUrl, position, this.httpOptions);
    }

    addPosition(position: Position) {
        const requestUrl = `${this._baseUrl}api/positions/`;
        return this.http.post(requestUrl, position, this.httpOptions);
    }


    deletePosition(positionId: number) {
        const requestUrl = `${this._baseUrl}api/positions/${positionId}`;
        return this.http.delete(requestUrl, this.httpOptions);
    }

    getRace(raceId: number) {
        const requestUrl = `${this._baseUrl}api/races/${raceId}`;
        return this.http.get<Position>(requestUrl);
    }

    getPositions(raceId: number, q: string, page: number) {
        const requestUrl = `${this._baseUrl}api/positions?raceId=${raceId}&q=${q}&page=${page + 1}`;
        return this.http.get<PositionList>(requestUrl);
    }

}