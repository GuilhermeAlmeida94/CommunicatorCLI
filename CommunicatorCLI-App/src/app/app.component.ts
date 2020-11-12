import { Component, OnInit } from '@angular/core';
import { Registry } from './_models/registry';
import { Message } from './_models/message';
import { CommandInput } from './_models/command-input';
import { WebSocketService } from './_services/web-socket.service';
import { MessageType } from './_enums/message-type.enum';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  showFiller = false;
  registries: Registry[] = [];
  returnText = '';

  constructor(private socketService: WebSocketService) {
    socketService.registries$.subscribe(registry => {
      if (registry) {
        const registryAdded = this.registries
          .filter(regItem => regItem.MachineName === registry.MachineName);

        if (registryAdded.length === 0) {
          this.registries.unshift(registry);
        }
        else {
          registryAdded[0].On = registry.On;
          registryAdded[0].Checked = registry.Checked;
        }
      }
    });
    socketService.returnText$.subscribe(rt => {
      this.returnText += rt;
    });
  }

  ngOnInit(): void {
    this.socketService.startSocket();
  }

  sendCommand(command: string): void {
    if (command === 'clear') {
      this.returnText = '';
    }
    else {
      const MachineNameToSend = this.registries
        .filter(reg => reg.Checked && reg.On)
        .map(reg => reg.MachineName);

      const message = new Message();
      message.MessageType = MessageType.CommandSend;
      message.CommandInput = new CommandInput();
      message.CommandInput.MachineNames = MachineNameToSend;
      message.CommandInput.Command = command;

      this.returnText += '>> ' + command + '\n';

      this.socketService.sendCommand(message);
    }
  }
}
