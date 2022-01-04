import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Relationship } from '../_models/relationship.model';
import { HorseList } from '../_models/horse-list.model';
import { Horse } from '../_models/horse.model';

@Injectable({
  providedIn: 'root'
})
export class RelationshipService {

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

  addRelationship(model: Relationship) {
    const requestUrl =
      `${this._baseUrl}api/relationships`;
    return this.http.post(requestUrl, model, this.httpOptions);
  }

  deleteRelationship(horseOId: string, parentType: string) {
    const requestUrl =
      `${this._baseUrl}api/relationships`;
    var options = this.httpOptions;
    options["body"] = { "horseOId": horseOId, "parentType": parentType };
    return this.http.delete<Boolean>(requestUrl, options);
  }

  deleteMappging(horseOId: string, parentOId: string) {
    const requestUrl =
      `${this._baseUrl}api/relationships/mapping`;
    var options = this.httpOptions;
    options["body"] = { "horseOId": horseOId, "parentOId": parentOId };
    return this.http.delete<Boolean>(requestUrl, options);
  }

  getOffsprings(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/offsprings/${horseId}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getSiblings(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/siblings/${horseId}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getFemailTail(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/femaletail/${horseId}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getUniqueAncestors(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/unique_ancestors/${horseId}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getUniqueAncestorsForHypothetical(maleHorseId: number, femaleHorseId: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/unique_ancestors_hypothetical?maleHorseId=${maleHorseId}&femaleHorseId=${femaleHorseId}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getInbreedings(horseOId: string, sort: string, order: string, page: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/inbreedings/${horseOId}?sort=${sort}&order=${order}&page=${page+1}&size=100`;
    return this.http.get<HorseList>(requestUrl);
  }

  getGrandparents(sort: string, order: string, page: number) {
    const requestUrl =
      `${this._baseUrl}api/relationships/grandparents?sort=${sort}&order=${order}&page=${page+1}&size=100`;
    return this.http.get<HorseList>(requestUrl);
  }
}
