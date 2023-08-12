import { Component } from '@angular/core';
import { ClientI } from '../../Interfaces/Client.interface';
import { InsuranceService } from '../../services/insurance.service';
import { Response, ResponseArray } from '../../Interfaces/Response.interface';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { IClientInsurance, IinsuranceInfo } from '../../Interfaces/ClientInsurance.interface';
import {MessageService } from 'primeng/api';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent {

  formulario: FormGroup;

  id: number = 0;
  client: IClientInsurance[] = [];
  insurance: IinsuranceInfo[] = [];
  showClientTable: boolean = false;
  showInsuranceTable: boolean = true;
  constructor(private isuranceService : InsuranceService, private formBuilder: FormBuilder, private messageService : MessageService){
    this.formulario = this.formBuilder.group({
      cedula: ['', [Validators.required, Validators.pattern('[0-9]{0,10}$')]]
    });
  }

  getDataByCed(){
    const cedulaValue = this.formulario.get('cedula')!.value;
    this.isuranceService.getListInsuranceByPerson(cedulaValue).subscribe(
      (response: Response) =>{
        if(response.code === "00"){
          this.messageService.add({severity: 'success' , detail: response.message });
          this.insurance = response.data;
          this.showClientTable = false; // Oculta la tabla de clientes
        this.showInsuranceTable = true; // Muestra la tabla de seguros
          this.formulario.get('cedula')!.reset();
        }else{
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });
  }
  getDataByCod(){
    this.isuranceService.getListaByIdInsurance(this.id).subscribe(
      (response: ResponseArray) =>{
        if(response.code === "00"){
          this.messageService.add({severity: 'success', detail: response.message });
          this.client = response.data;
          this.showClientTable = true; // Muestra la tabla de clientes
          this.showInsuranceTable = false; // Oculta la tabla de seguros
          this.id = 0;
        }else{
          this.messageService.add({severity: 'error' , detail: response.message });
        }
      });
  }
}
