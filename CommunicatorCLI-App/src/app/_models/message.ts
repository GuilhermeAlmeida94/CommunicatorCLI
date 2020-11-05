import { MessageType } from '../_enums/message-type.enum';
import { Registry } from './registry';
import { CommandInput } from './command-input';

export class Message {
    MessageType: MessageType;
    CommandInput: CommandInput;
    CommandOutput: string;
    Registry: Registry;
}
