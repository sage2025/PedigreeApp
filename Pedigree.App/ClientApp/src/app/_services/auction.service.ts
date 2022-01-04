import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { empty } from 'rxjs';
import { Auction } from '../_models/auction.model';
import { AuctionDetail } from '../_models/auction-detail.model';

@Injectable({
    providedIn: 'root'
})

export class AuctionService {
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

    getCurrentSales() {
        let requestUrl = 
            `${this._baseUrl}api/auction/get_current_sales`;
        return this.http.get(requestUrl);
    }

    addAuction(date: string, name: string) {
        let requestUrl = 
            `${this._baseUrl}api/auction/add_auction?date=${date}&name=${name}`;
        return this.http.get<Auction>(requestUrl);
    }

    addHorse(auctionId: number, number: number, name: string, type: string, yob: number, sex: string, country: string, fatherName: string, motherName: string) {
        let requestUrl = 
            `${this._baseUrl}api/auction/add_horse?auctionId=${auctionId}&number=${number}&name=${name}&type=${type}&yob=${yob}&sex=${sex}&country=${country}&fatherName=${fatherName}&motherName=${motherName}`;
        return this.http.get(requestUrl);
    }

    addAuctionDetails(details: AuctionDetail[]) {
        let requestUrl = 
            `${this._baseUrl}api/auction/add_auction_details`;
        return this.http.post(requestUrl, details, this.httpOptions);
    }

    deleteAuction(auctionId: number) {
        let requestUrl = 
            `${this._baseUrl}api/auction/delete_auction?auctionId=${auctionId}`;
        return this.http.get(requestUrl);
    }

    deleteAuctionDetail(auctionDetailId: number) {
        let requestUrl = 
            `${this._baseUrl}api/auction/delete_auction_detail?auctionDetailId=${auctionDetailId}`;
        return this.http.get(requestUrl);
    }

    checkPedigComp(horseId: number) {
        let requestUrl = 
            `${this._baseUrl}api/auction/check_pedigcomp?horseId=${horseId}`;
        return this.http.get(requestUrl);
    }

    getAuctionDetail(auctionId: number) {
        let requestUrl = 
            `${this._baseUrl}api/auction/get_auction_detail?auctionId=${auctionId}`;
        return this.http.get(requestUrl);
    }

    getAuction(auctionId: number) {
        let requestUrl = 
            `${this._baseUrl}api/auction/get_auction?auctionId=${auctionId}`;
        return this.http.get(requestUrl);
    }
    getmtDNAHap(motherName: string) {
        let requestUrl = 
            `${this._baseUrl}api/auction/get_mtDNAHap?motherName=${motherName}`;
        return this.http.get(requestUrl);
    }
}