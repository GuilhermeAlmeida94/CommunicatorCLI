import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit {

  @Input() returnText: string;
  @Output() newItemEvent = new EventEmitter<string>();

  constructor() { }

  sendCommand(value: string): void {
    this.newItemEvent.emit(value);
  }

  ngOnInit(): void {
  }

}
