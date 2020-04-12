import { Component, OnInit } from '@angular/core';
import { StandService } from '../../services/stand.service';
import { Stand } from '../model/stand';

@Component({
  templateUrl: './stand-list.component.html'
})
export class StandListComponent implements OnInit {
  private stands: Stand[];
  constructor(
    private standService: StandService
  ) {}

  ngOnInit(): void {
    this.standService.getAll().subscribe(stands => {
      this.stands = stands;
    });
  }
}
