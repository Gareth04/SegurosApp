import { Component } from '@angular/core';
import { Insurance } from '../../Interfaces/Insurance.interface';
import { ClientI } from '../../Interfaces/Client.interface';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { InsuranceService } from '../../services/insurance.service';
import { ResponseArray, Response } from '../../Interfaces/Response.interface';
import {MessageService } from 'primeng/api';

@Component({
  selector: 'app-client',
  templateUrl: './client.component.html',
  styleUrls: ['./client.component.css']
})
export class ClientComponent {

  formulario: FormGroup;
  //Arreglos
  client: ClientI[] = [];

  //Datos de la bd
  name: string = "";
  cedula: string = ""; 
  phone: string = "";
  insurance_name: string = "";
  id: number = 0;
  id_insurance: number = 0;
  age: number = 0;

   //Objetos
   obj_client!: ClientI;

  //
  fileToUpload: File | null = null;

  constructor( private formBuilder: FormBuilder, private isuranceService : InsuranceService , private messageService : MessageService)
  {
    this.formulario = this.formBuilder.group({
      name: ['', [Validators.required, Validators.pattern('[A-Za-z ]')]],
      cedula: ['', [Validators.required, Validators.pattern('[0-9]{0,10}$')]],
      phone: ['', [Validators.required, Validators.pattern('[0-9]{0,10}$')]],
      age: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      insurance_name: ['', [Validators.required, Validators.pattern('[A-Za-z ]*')]]
    });
  }
  ngOnInit(): void {
    //Obtener el arreglo de asegurados y guardarlo en un arreglo de tipo asegurado
    this.listClient();
  }
  //Get
  listClient()
  {
    this.isuranceService.getListClient().subscribe(
      (response: ResponseArray) =>{
        if(response.code=== "00"){
          this.client = response.data;
          this.messageService.add({severity: 'success', detail: response.message });
        }else{
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });
  }

  //Post
  sendClient()
  {
    const insuranceName = this.formulario.get('insurance_name')!.value
    this.isuranceService.getObjInsuranceName(insuranceName).subscribe(
      (response: ResponseArray) =>{
        this.obj_client = {
          id_client: 0,
          id_Insurance: 0,
          cedula: this.formulario.get('cedula')!.value,
          name: this.formulario.get('name')!.value,
          phone: this.formulario.get('phone')!.value,
          age: this.formulario.get('age')!.value
        }
        console.log(this.obj_client)
        this.isuranceService.postClient(this.obj_client).subscribe(
          (response) => {
            if(response.code=== "00"){
              console.log("entre",response)
              this.messageService.add({severity: 'success', detail: response.message });
              this.listClient();
              this.clearForm();
              document.querySelector('.popup')?.classList.remove('open-popup')
            }else{
            this.messageService.add({severity: 'error', detail: response.message });
            }
          });
      });
  }
  handleFileInput(event: any) {
    this.fileToUpload = event.target.files[0];
  }
  uploadFile() {
    if (this.fileToUpload) {
      this.isuranceService.uploadExcelAsg(this.fileToUpload).subscribe(
        (response) => {
          if(response.code=== "00"){
            this.messageService.add({severity: 'success' , detail: response.message });
            this.listClient();
            this.closePopup();
          }else{
            console.log(response)
            this.messageService.add({severity: 'error', detail: response.message });
          }
        });
    } 
  }


  //put
  editatAsegurado(){
        this.obj_client = {
          id_client: this.id,
          id_Insurance: this.id_insurance,
          cedula: this.formulario.get('cedula')!.value,
          name: this.formulario.get('name')!.value,
          phone: this.formulario.get('phone')!.value,
          age: this.formulario.get('age')!.value,
        }
        this.isuranceService.putClient(this.obj_client).subscribe(
          (response) => {
            if(response.code === "00"){
              this.messageService.add({severity: 'success', detail: response.message });
              this.listClient();
              this.clearForm();
              console.log(response)
              document.querySelector('.editpopup')?.classList.remove('editopen-popup')
            }else{
              this.messageService.add({severity: 'error', detail: response.message });
              console.log(response)
            }
          });

  }
  //put
  agregarAsegurado(){
    let cedula = this.formulario.get('cedula')!.value
    let name = this.formulario.get('insurance_name')!.value
    this.isuranceService.PostClientSegu(cedula, name).subscribe(
      (response) => {
        if(response.code=== "00"){
          console.log(response)
          this.listClient();
          this.messageService.add({severity: 'success', detail: response.message });
        }else{
          console.log(response)
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });
  }

  //Delete
  removeClient(id:number){
    this.isuranceService.deleteClient(id).subscribe(
      (response) => {
        if(response.code=== "00"){
          this.listClient();
          this.messageService.add({severity: 'success', detail: response.message });
        }else{
          console.log(response)
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });   
  }
  removeInsurance(){
    let cedula = this.formulario.get('cedula')!.value
    let name = this.formulario.get('insurance_name')!.value
    this.isuranceService.deleteClientSegu(cedula, name).subscribe(
      (response) => {
        if(response.code=== "00"){
          console.log(response)
          this.listClient();
          this.messageService.add({severity: 'success', detail: response.message });
        }else{
          console.log(response)
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });  
  }

   //reset
   clearForm(){
     this.formulario.reset();
   }

  //Popups
  //Create
  openPopup()
  {
    document.querySelector('.popup')?.classList.add('open-popup')
    // this.popup?.classList.add("open-popup");
  }
  closePopup()
  {
    document.querySelector('.popup')?.classList.remove('open-popup')
  }
  //Edit
  openEditPopup(client: ClientI){
    this.id = client.id_client
    this.id_insurance = client.id_Insurance
    this.formulario.patchValue({
      cedula : client.cedula,
      name : client.name,
      phone : client.phone,
      age : client.age
    });
      document.querySelector('.editpopup')?.classList.add('editopen-popup')
  }
  closeEditPopup()
  {
    document.querySelector('.editpopup')?.classList.remove('editopen-popup')
  }

  openAgregarPopup(client: ClientI){
    this.id = client.id_client
    this.id_insurance = client.id_Insurance
    this.formulario.patchValue({
      cedula : client.cedula
    });
      document.querySelector('.agregarpopup')?.classList.add('agregaropen-popup')
  }
  closeAgregarPopup()
  {
    document.querySelector('.agregarpopup')?.classList.remove('agregaropen-popup')
  }
  openRemoveInsurance(client: ClientI){
    this.id = client.id_client
    this.id_insurance = client.id_Insurance
    this.formulario.patchValue({
      cedula : client.cedula
    });
      document.querySelector('.eliminarpopup')?.classList.add('eliminaropen-popup')
  }
  closeRemoveInsurance()
  {
    document.querySelector('.eliminarpopup')?.classList.remove('eliminaropen-popup')
  }
}
