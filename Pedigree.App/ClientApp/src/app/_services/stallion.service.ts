import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { StallionRatingList } from '../_models/stallion-rating-list.model';

@Injectable({
  providedIn: 'root'
})
export class StallionService {


  public httpOptions: any;
  _baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._baseUrl = baseUrl;
    this.httpOptions = {
      headers: new HttpHeaders(
        {
          'Content-Type': 'application/json; charset=utf-8'
          //'BrowserToken': 'auth_Token'
        })
    }
  }

  getStallionRatings(type: string, query: string, sort: string, order: string, page: number) {
    const requestUrl =
      `${this._baseUrl}api/stallion/stallion_ratings?t=${type}&q=${query}&sort=${sort}&order=${order}&page=${page + 1}`;
    return this.http.get<StallionRatingList>(requestUrl);
  } 
}
