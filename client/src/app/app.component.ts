import { Component } from '@angular/core';
import { SessionService } from '../services/session.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'market-locator';

  constructor(
    private sessionService: SessionService
  ) {
    this.sessionService.logIn('admin', '1234');
  }

}
