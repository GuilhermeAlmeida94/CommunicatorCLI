import { Component, Input, OnInit } from '@angular/core';
import { Registry } from '../_models/registry';

@Component({
  selector: 'app-registry',
  templateUrl: './registry.component.html',
  styleUrls: ['./registry.component.css']
})
export class RegistryComponent implements OnInit {
  @Input() registry: Registry;

  constructor() { }

  ngOnInit(): void {
  }

}
