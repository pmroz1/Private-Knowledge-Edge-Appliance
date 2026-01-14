import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileUploadModule } from 'primeng/fileupload';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../services/api.service';

interface ChatMessage {
    text: string;
    sender: 'user' | 'Ollama3.2';
}

@Component({
    selector: 'app-chat',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        FileUploadModule,
        ButtonModule,
        InputTextModule,
        CardModule,
        MessageModule,
        ToastModule
    ],
    providers: [MessageService],
    template: `
    <div class="flex flex-col h-screen p-4 gap-4 max-w-4xl mx-auto">
      <p-toast />
      
      <p-card header="Knowledge Base" subheader="Upload documents to chat with them">
        <div class="flex gap-4 items-center">
            <p-fileUpload 
                mode="basic" 
                chooseLabel="Select PDF" 
                chooseIcon="pi pi-upload"
                name="file" 
                url="http://localhost:5000/upload" 
                accept="application/pdf" 
                maxFileSize="10000000" 
                (onUpload)="onUpload($event)"
                [auto]="true"
                customUpload="true" 
                (uploadHandler)="customUpload($event)" />
        </div>
      </p-card>

      <p-card styleClass="flex-1 flex flex-col overflow-hidden" [style]="{height: '100%'}">
        <div class="flex flex-col h-full gap-4">
            <div class="flex-1 overflow-y-auto flex flex-col gap-2 p-2 border rounded border-surface-200" #chatContainer>
                @for (msg of messages(); track $index) {
                    <div [class]="'p-3 rounded-lg max-w-[80%] ' + (msg.sender === 'user' ? 'self-end bg-primary-100 text-right' : 'self-start bg-surface-100')">
                        <strong>{{ msg.sender === 'user' ? 'You' : 'Bot' }}:</strong>
                        <div class="whitespace-pre-wrap">{{ msg.text }}</div>
                    </div>
                }
                @if (messages().length === 0) {
                    <div class="text-center text-gray-500 mt-10">Start by asking a question...</div>
                }
            </div>

            <div class="flex gap-2">
                <input pInputText class="flex-1" [(ngModel)]="currentQuestion" (keydown.enter)="sendMessage()" placeholder="Ask a question..." [disabled]="isLoading()" />
                <p-button label="Send" icon="pi pi-send" (onClick)="sendMessage()" [loading]="isLoading()" [disabled]="!currentQuestion" />
            </div>
        </div>
      </p-card>
    </div>
  `,
    styles: [`
    :host { display: block; height: 100vh; background-color: var(--surface-ground); }
  `]
})
export class ChatComponent {
    private api = inject(ApiService);
    private messageService = inject(MessageService);

    messages = signal<ChatMessage[]>([]);
    currentQuestion = '';
    isLoading = signal(false);

    customUpload(event: any) {
        const file = event.files[0];
        this.api.upload(file).subscribe({
            next: (res) => {
                this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Document uploaded and processed.' });
            },
            error: (err) => {
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Upload failed.' });
                console.error(err);
            }
        });
    }

    onUpload(event: any) {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File uploaded' });
    }

    sendMessage() {
        if (!this.currentQuestion.trim() || this.isLoading()) return;

        const question = this.currentQuestion;
        this.messages.update(msgs => [...msgs, { text: question, sender: 'user' }]);
        this.currentQuestion = '';
        this.isLoading.set(true);

        this.api.ask(question).subscribe({
            next: (res) => {
                this.messages.update(msgs => [...msgs, { text: res.answer, sender: 'Ollama3.2' }]);
                this.isLoading.set(false);
            },
            error: (err) => {
                this.messages.update(msgs => [...msgs, { text: 'Error getting response.', sender: 'Ollama3.2' }]);
                this.isLoading.set(false);
                console.error(err);
            }
        });
    }
}
