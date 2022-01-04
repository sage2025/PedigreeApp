import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { empty } from 'rxjs';
import { HorseList } from '../_models/horse-list.model';
import { Horse } from '../_models/horse.model';
import { HorseWithParentChildren } from '../_models/horse-with-parent-children.model';
import { TreeData } from '../_models/tree-data.model';
import { HorseHeirarchy } from '../_models/horse-heirarchy.model';
import { LinebreedingItem } from '../_models/linebreeding-item.model';
import { Par3Item } from '../_models/par3-item.model';
import { Race } from '../_models/race.model';
import { AncestriesData } from '../_models/ancestries-data.model';
import { PlotItem } from '../_models/plot-item.model';
import { SireCrossData } from '../_models/sire-cross-data.model';

@Injectable({
  providedIn: 'root'
})
export class HorseService {


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

  updateHorse(horse: Horse) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horse.id}`;
    return this.http.put(requestUrl, horse, this.httpOptions);
  }

  addHorse(horse: Horse) {
    const requestUrl =
      `${this._baseUrl}api/horses/`;
    return this.http.post(requestUrl, horse, this.httpOptions);
  }


  deleteHorse(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horseId}`;
    return this.http.delete(requestUrl, this.httpOptions);
  }

  checkHorseLinkage(horseOId: string) {
    const requestUrl =
      `${this._baseUrl}api/horses/linkage/${horseOId}`;
    return this.http.get(requestUrl, this.httpOptions);
  }

  getHorse(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horseId}/single`;
    return this.http.get<Horse>(requestUrl);
  }

  getHorseWithPC(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horseId}`;
    return this.http.get<HorseWithParentChildren>(requestUrl);
  }

  getHorsHeirarchy(horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/heirarchy/${horseId}`;
    return this.http.get<HorseHeirarchy>(requestUrl);
  }

  getHypotheticalHorsHeirarchy(maleHorseId: number, femaleHorseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/heirarchy/hypothetical?maleHorseId=${maleHorseId}&femaleHorseId=${femaleHorseId}`;
    return this.http.get<HorseHeirarchy>(requestUrl);
  }

  searchHorseStartsWith(name: string) {
    if (typeof name !== 'string' || name == '') return empty();
    const requestUrl =
      `${this._baseUrl}api/horses/search/${name}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  searchHorseEx(query: string, sex: string) {
    if (typeof query !== 'string' || query == '') return empty();

    let requestUrl =
      `${this._baseUrl}api/horses/search_ex/${query}`;

    let queryString = '';
    if (sex != null) queryString += `sex=${sex}`;

    if (queryString.length > 0) requestUrl += `?${queryString}`;

    return this.http.get<Horse[]>(requestUrl);
  }

  getHorses(query: string, sort: string, order: string, page: number) {
    const requestUrl =
      `${this._baseUrl}api/horses?q=${query}&sort=${sort}&order=${order}&page=${page + 1}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getHorsesForHypotheticalMating(maleq: string, femaleq: string, sort: string, order: string, page: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/hypothetical/search?maleq=${maleq}&femaleq=${femaleq}&sort=${sort}&order=${order}&page=${page + 1}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getLinebreedings(horseId: number) {
    const requestUrl = `${this._baseUrl}api/horses/linebreedings/${horseId}`;
    return this.http.get<LinebreedingItem[]>(requestUrl);
  }

  getLinebreedingsForHypothetical(maleHorseId: number, femaleHorseId: number) {
    const requestUrl = `${this._baseUrl}api/horses/linebreedings_hypothetical?maleHorseId=${maleHorseId}&femaleHorseId=${femaleHorseId}`;
    return this.http.get<LinebreedingItem[]>(requestUrl);
  }

  getEquivalents(horseId: number) {
    const requestUrl = `${this._baseUrl}api/horses/equivalents/${horseId}`;
    return this.http.get<Par3Item[]>(requestUrl);
  }

  getEquivalentsForHypothetical(maleHorseId: number, femaleHorseId: number) {
    const requestUrl = `${this._baseUrl}api/horses/equivalents_hypothetical?maleHorseId=${maleHorseId}&femaleHorseId=${femaleHorseId}`;
    return this.http.get<Par3Item[]>(requestUrl);
  }

  getCommonAncestors(horseId1: number, horseId2: number) {
    const requestUrl = `${this._baseUrl}api/horses/common_ancestors?horseId1=${horseId1}&horseId2=${horseId2}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getIncompletedPedigreeHorses(year: number, sort: string, order: string, page: number) {
    const requestUrl = `${this._baseUrl}api/horses/incompleted_pedigree_horses?year=${year}&sort=${sort}&order=${order}&page=${page + 1}`;
    return this.http.get<HorseList>(requestUrl);
  }

  getTwinDams() {
    const requestUrl = `${this._baseUrl}api/horses/twins`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getFounders(/*q: string, sort: string, order: string, page: number*/) {
    //const requestUrl = `${this._baseUrl}api/horses/founders?q=${q}&sort=${sort}&order=${order}&page=${page + 1}`;
    const requestUrl = `${this._baseUrl}api/horses/founders`;
    return this.http.get<HorseList>(requestUrl);
  }

  setAsFounder(horseId: number, isFounder: boolean) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horseId}/set_founder`;
    return this.http.put(requestUrl, { isFounder }, this.httpOptions);
  }

  setMtDNA(horseId: number, mtDNA: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/${horseId}/set_mtdna`;
    return this.http.put(requestUrl, { mtDNA }, this.httpOptions);
  }

  getRacesForHorse(horseId: number, sort: string, order: string) {
    const requestUrl = `${this._baseUrl}api/horses/races/${horseId}?sort=${sort}&order=${order}`;
    return this.http.get<Race[]>(requestUrl);
  }

  getAncestorData() {
    const requestUrl = `${this._baseUrl}api/horses/ancestor_data`;
    return this.http.get<AncestriesData>(requestUrl);
  }

  getPopulationData() {
    const requestUrl = `${this._baseUrl}api/horses/population_data`;
    return this.http.get<PlotItem[]>(requestUrl);
  }

  getZCurrentPlotData() {
    const requestUrl = `${this._baseUrl}api/horses/zcurrent_plot_data`;
    return this.http.get<PlotItem[]>(requestUrl);
  }

  getZHistoricalPlotData() {
    const requestUrl = `${this._baseUrl}api/horses/zhistorical_plot_data`;
    return this.http.get<PlotItem[]>(requestUrl);
  }

  getHorsesForSireSearch(maleHorseId: number, femaleHorseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/sire/search?maleId=${maleHorseId}&femaleId=${femaleHorseId}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHorsesForSireBroodmareSireSearch(type: string, horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/sire_broodmare_sire/search?type=${type}&maleId=${horseId}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHorsesForSirelineSearch(type: string, horseId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/sire_line/search?type=${type}&maleId=${horseId}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getSiresCrossData(maleId1: number, maleId2: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/sires_cross_data?maleId1=${maleId1}&maleId2=${maleId2}`;
    return this.http.get<SireCrossData>(requestUrl);
  }

  getHorsesForWildcard1Search(horse1Id: number, horse2Id: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/wildcard1/search?horse1Id=${horse1Id}&horse2Id=${horse2Id}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHorsesForWildcard2Search(horse1Id: number, horse2Id: number, horse3Id: number, horse4Id: number) {
    let requestUrl =
      `${this._baseUrl}api/horses/wildcard2/search?horse1Id=${horse1Id}&horse3Id=${horse3Id}`;
    if (horse2Id != null) requestUrl += '&horse2Id=' + horse2Id;
    if (horse4Id != null) requestUrl += '&horse4Id=' + horse4Id;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHorsesForWildcardQueryByPosition(searches: object) {
    let requestUrl =
      `${this._baseUrl}api/horses/wildcard/query_position?searches=${JSON.stringify(searches)}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHorsesForFamilyStakeSearch(femaleId: number, gen: number) {
    let requestUrl =
      `${this._baseUrl}api/horses/family_stake/search?femaleId=${femaleId}&gen=${gen}`;
    return this.http.get<Horse[]>(requestUrl);
  }

  getHierarchyForFemaleLineSearch(femaleId: number) {
    const requestUrl =
      `${this._baseUrl}api/horses/family_line/search?femaleId=${femaleId}`;
    return this.http.get<HorseHeirarchy>(requestUrl);
  }

  getHorsesForMtDNALookup(haploGroupId: number) {
    let requestUrl =
      `${this._baseUrl}api/horses/mtdna_lookup/${haploGroupId}`;
    return this.http.get<Horse[]>(requestUrl);
  }
}
