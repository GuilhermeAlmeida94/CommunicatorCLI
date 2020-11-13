import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-historic',
  templateUrl: './historic.component.html',
  styleUrls: ['./historic.component.css']
})
export class HistoricComponent implements OnInit {

  @Input() historicReturnText: string;

  constructor() { }

  ngOnInit(): void {
  }

}
