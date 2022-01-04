import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Weight } from '../_models/weight.model';
import { WeightList } from '../_models/weight-list.model';

@Injectable({
    providedIn: 'root'
})
export class WeightService {
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


    updateWeight(weight: Weight) {
        const requestUrl = `${this._baseUrl}api/weights/${weight.id}`;
        return this.http.put(requestUrl, weight, this.httpOptions);
    }

    addWeight(weight: Weight) {
        const requestUrl = `${this._baseUrl}api/weights/`;
        return this.http.post(requestUrl, weight, this.httpOptions);
    }


    deleteWeight(weightId: number) {
        const requestUrl = `${this._baseUrl}api/weights/${weightId}`;
        return this.http.delete(requestUrl, this.httpOptions);
    }

    getWeight(weightId: number) {
        const requestUrl = `${this._baseUrl}api/weights/${weightId}`;
        return this.http.get<Weight>(requestUrl);
    }

    getWeights(filters: object, sort: string, order: string, page: number) {
        const query: string = JSON.stringify(filters);
        const requestUrl = `${this._baseUrl}api/weights?q=${query}&sort=${sort}&order=${order}&page=${page + 1}`;
        return this.http.get<WeightList>(requestUrl);
    }

}