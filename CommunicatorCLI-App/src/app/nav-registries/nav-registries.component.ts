import { Component, Input, OnInit } from '@angular/core';
import { Registry } from '../_models/registry';

@Component({
  selector: 'app-nav-registries',
  templateUrl: './nav-registries.component.html',
  styleUrls: ['./nav-registries.component.css']
})
export class NavRegistriesComponent implements OnInit {
  @Input() registries: Registry[];

  constructor() { }

  ngOnInit(): void {
  }

}
