import { EventEmitter, Input, Output } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.css']
})
export class ContentComponent implements OnInit {

  @Input() contentReturnText: string;
  @Output() contentEvent = new EventEmitter<string>();

  constructor() { }

  contentAction(value: string): void {
    this.contentEvent.emit(value);
  }

  ngOnInit(): void {
  }

}
