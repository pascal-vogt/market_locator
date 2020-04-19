import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { MarketMapComponent } from './market-map/market-map.component';
import { StandListComponent } from './stand/stand-list.component';
import { StandEditComponent } from './stand/stand-edit.component';
import {FormsModule} from '@angular/forms';

const appRoutes: Routes = [
  {
    path: 'market-map',
    component: MarketMapComponent
  },
  {
    path: 'stands',
    component: StandListComponent
  },
  {
    path: 'stands/:standId',
    component: StandEditComponent
  },
  {
    path: '**',
    redirectTo: 'market-map'
  }
];


@NgModule({
  declarations: [
    AppComponent,
    MarketMapComponent,
    StandListComponent,
    StandEditComponent
  ],
  imports: [
    HttpClientModule,
    BrowserModule,
    RouterModule.forRoot(appRoutes, {useHash: true}),
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
