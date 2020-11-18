import { MessageType } from '../_enums/message-type.enum';
import { Registry } from './registry';
import { Command } from './command';

export class Message {
    MessageType: MessageType;
    Command: Command;
    CommandResult: string;
    Registry: Registry;
}
