import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit {

  @Output() newItemEvent = new EventEmitter<string>();

  constructor() { }

  sendCommand(value: string): void {
    this.newItemEvent.emit(value);
  }

  ngOnInit(): void {
  }

}
