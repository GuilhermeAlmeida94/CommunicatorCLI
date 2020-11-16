import { Component, Input, OnInit } from '@angular/core';
import { Registry } from '../_models/registry';

@Component({
  selector: 'app-registries-nav-bar',
  templateUrl: './registries-nav-bar.component.html',
  styleUrls: ['./registries-nav-bar.component.css']
})
export class RegistriesSideBarComponent implements OnInit {
  @Input() registries: Registry[];

  constructor() { }

  ngOnInit(): void {
  }

}
