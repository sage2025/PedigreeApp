import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root',
})
export class FilesService {
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

    public upload(formData: FormData) {
        return this.http.post(`${this._baseUrl}api/files/upload`, formData, {
            reportProgress: true,
            observe: 'events',
        });
    }

    public uploadSvg(data: string) {
        // return this.http.post(`${this._baseUrl}api/files/uploadsvg`,data,  {
            
        // });
    }

    public download(id:number): Observable<any> {
        
        
        return this.http.get(`${this._baseUrl}api/files/download/${id}`, {
            });
        
    }

    

}