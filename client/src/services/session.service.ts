import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  constructor(
    private httpClient: HttpClient
  ) {}

  logIn(email: string, password: string) {
    this.httpClient.post('api/session', {
      email,
      password
    }).subscribe();
  }
}
