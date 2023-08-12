import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ToastModule} from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';



@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  exports:[
    ToastModule,
    MessagesModule,
    MessageModule,
    BrowserAnimationsModule
  ]
})
export class PrimeNgModule { }
