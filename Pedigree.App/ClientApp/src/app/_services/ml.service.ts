import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MLModel } from '../_models/ml-model.model';

@Injectable({
  providedIn: 'root'
})
export class MLService {


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

  retrainModel(modelId: number) {
    const requestUrl =
      `${this._baseUrl}api/ml/retrain_model/${modelId}`;
    return this.http.post<MLModel>(requestUrl, {}, this.httpOptions);
  }

  trainModel(columns: object) {
    const requestUrl =
      `${this._baseUrl}api/ml/train_model`;
    return this.http.post<MLModel>(requestUrl, {columns}, this.httpOptions);
  }

  deployModel(modelId: number) {
    const requestUrl =
      `${this._baseUrl}api/ml/deploy_model/${modelId}`;
    return this.http.post(requestUrl, {}, this.httpOptions);
  }

  evaluateModel(modelId: number, data: Object) {
    const requestUrl =
      `${this._baseUrl}api/ml/evaluate_model`;
    return this.http.post<number>(requestUrl, {modelId, data}, this.httpOptions);
  }

  getLastModel() {
    const requestUrl =
      `${this._baseUrl}api/ml/last_model`;
    return this.http.get<MLModel>(requestUrl);
  }

  getHypotheticalMLScore(maleHorseId: number, femaleHorseId: number, features: string[], modelId: number) {    
    let requestUrl =
      `${this._baseUrl}api/ml/hypothetical_ml_score?maleHorseId=${maleHorseId}&femaleHorseId=${femaleHorseId}&features=${features}&modelId=${modelId}`;
    return this.http.get<any>(requestUrl);
  }
}
