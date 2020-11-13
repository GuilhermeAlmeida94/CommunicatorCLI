import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-terminal',
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit {

  @Output() terminalEvent = new EventEmitter<string>();

  constructor() { }

  terminalAction(value: string): void {
    this.terminalEvent.emit(value);
  }

  ngOnInit(): void {
  }

}
