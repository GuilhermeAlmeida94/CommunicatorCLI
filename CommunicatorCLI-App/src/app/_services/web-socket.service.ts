import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MessageType } from '../_enums/message-type.enum';
import { Registry } from '../_models/registry';
import { Message } from '../_models/message';

@Injectable()
export class WebSocketService {
    private socket: WebSocket;
    private address = 'localhost';
    private port = '5000';
    registries$: BehaviorSubject<Registry> = new BehaviorSubject<Registry>(null);
    returnText$: BehaviorSubject<string> = new BehaviorSubject<string>('');

    constructor() { }

    startSocket(): void  {
      this.socket = new WebSocket(`ws://${this.address}:${this.port}/ws`);
      this.socket.addEventListener('open', (ev => {
        console.log('opened');
      }));
      this.socket.addEventListener('message', (ev => {
        const message = JSON.parse(ev.data);
        console.log('message object', message);
        switch (message.MessageType) {
          case MessageType.LogOn:
            message.Registry.On = true;
            message.Registry.Checked = false;
            this.registries$.next(message.Registry);
            break;
          case MessageType.LogOff:
            message.Registry.On = false;
            message.Registry.Checked = false;
            this.registries$.next(message.Registry);
            break;
          case MessageType.CommandResult:
            this.returnText$.next(message.CommandResult);
            break;
          default:
            break;
        }
      }));
      this.socket.onopen = () => this.sendMachine();
    }

    sendCommand(req: Message): void  {
      const requestAsJson = JSON.stringify(req);
      this.socket.send(requestAsJson);
    }

    sendMachine(): void {
      const req = new Message();
      req.MessageType = MessageType.Leader;

      const requestAsJson = JSON.stringify(req);
      this.socket.send(requestAsJson);
    }
}
