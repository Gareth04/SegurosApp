import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { SecureCoreModule } from './SecureCore/SecureCore.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from './shared/shared.module';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PrimeNgModule } from './PrimeNg/prime-ng.module';
import { MessageService } from 'primeng/api';

@NgModule({
  declarations: [
    AppComponent

  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SecureCoreModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule,
    SharedModule,
    PrimeNgModule
  ],
  providers: [MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }
