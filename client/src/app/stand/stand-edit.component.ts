import {Component, OnInit} from '@angular/core';
import {StandService} from '../../services/stand.service';
import {ActivatedRoute} from '@angular/router';
import {switchMap} from 'rxjs/operators';
import {Stand} from '../model/stand';

@Component({
  templateUrl: './stand-edit.component.html'
})
export class StandEditComponent implements OnInit {
  private stand: Stand;
  constructor(
    private standService: StandService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params
      .pipe(
        switchMap(params => {
          return this.standService.getById(params.standId);
        })
      )
      .subscribe(stand => {
        this.stand = stand;
      });
  }
}
