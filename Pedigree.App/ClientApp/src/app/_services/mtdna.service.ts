import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HaploGroup } from '../_models/haplo-group.model';
import { HaploGroupStallion } from '../_models/haplo-group-stallion.model';
import { HaploGroupDistance } from '../_models/haplo-group-distance.model';
import { MtDNAFlag } from '../_models/mtdna-flag.model';

@Injectable({
  providedIn: 'root'
})
export class MtDnaService {


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

  getHaploGroups() {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplogroups`;
    return this.http.get<HaploGroup[]>(requestUrl);
  } 

  getSimpleHaploGroups() {
    const requestUrl =
      `${this._baseUrl}api/mtdna/simple_haplogroups`;
    return this.http.get<HaploGroup[]>(requestUrl);
  }  

  getHorseHaploGroups(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/horse_haplogroups/${horseId}`;
    return this.http.get<HaploGroup[]>(requestUrl);
  }  

  getHypotheticalHorseHaploGroups(maleId: number, femaleId: number) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/hypothetical_horse_haplogroups?maleId=${maleId}&femaleId=${femaleId}`;
    return this.http.get<HaploGroup[]>(requestUrl);
  }  

  addHaploType(groupId: number, typeName: string) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplotypes`;
    return this.http.post(requestUrl, {groupId, name: typeName}, this.httpOptions);
  }

  deleteHaploType(typeId: number) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplotypes/${typeId}`;
    return this.http.delete(requestUrl, this.httpOptions);
  }

  updateHaploGroupColor(groupId: number, color: string) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplogroups/${groupId}/color`;
    return this.http.put(requestUrl, {color}, this.httpOptions);
  }

  getHaploGroupsStallion(maleId: number) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplogroups_stallion?maleId=${maleId}`;
    return this.http.get<HaploGroupStallion[]>(requestUrl);
  } 

  getHaploGroupsDistance() {
    const requestUrl =
      `${this._baseUrl}api/mtdna/haplogroups_distance`;
    return this.http.get<HaploGroupDistance[]>(requestUrl);
  } 

  addMtDNAFlags(startHorseOId: string, endHorseOId: string) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/flags`;
    return this.http.post<number>(requestUrl, {startHorseOId, endHorseOId}, this.httpOptions);    
  }

  deleteMtDNAFlags(flagId: number) {
    const requestUrl =
      `${this._baseUrl}api/mtdna/flags/${flagId}`;
    return this.http.delete(requestUrl, this.httpOptions);    
  }

  getMtDNAFlags() {
    const requestUrl =
      `${this._baseUrl}api/mtdna/flags`;
    return this.http.get<MtDNAFlag[]>(requestUrl);
  } 
}
