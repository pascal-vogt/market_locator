import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Stand} from '../app/model/stand';

@Injectable({
  providedIn: 'root'
})
export class StandService {
  constructor(
    private httpClient: HttpClient
  ) {}

  getAll(): Observable<Stand[]> {
    return this.httpClient.get<Stand[]>('api/stands');
  }

  getById(standId: number): Observable<Stand> {
    return this.httpClient.get<Stand>(`api/stands/${standId}`);
  }
}
