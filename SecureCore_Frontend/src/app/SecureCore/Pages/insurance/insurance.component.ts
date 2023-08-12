import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InsuranceService } from '../../services/insurance.service';
import { Insurance } from '../../Interfaces/Insurance.interface';
import { ResponseArray } from '../../Interfaces/Response.interface';
import {MessageService } from 'primeng/api';


@Component({
  selector: 'app-insurance',
  templateUrl: './insurance.component.html',
  styleUrls: ['./insurance.component.css']
})
export class InsuranceComponent implements OnInit{
  formulario: FormGroup;
  name : string ="";
  id: number = 0;
  sum_Insured: number = 0;
  Premium: number = 0;
  insurance: Insurance[] = [];
  obj_isurance!: Insurance;

  selectedFile: File | undefined;
  fileToUpload: File | null = null;

  constructor(private formBuilder: FormBuilder, private isuranceService : InsuranceService, private messageService : MessageService)
  {
    this.formulario = this.formBuilder.group({
      name: ['', [Validators.required, Validators.pattern('[A-Za-z ]')]],
      sum_Insured: ['', [Validators.required, Validators.pattern('[0-9]')]],
      Premium: ['', [Validators.required, Validators.pattern('[0-9]*')]]
    });
  }

  ngOnInit(): void {
    this.listInsurance();
  }

  //Get
  listInsurance(){
    this.isuranceService.getListInsurance().subscribe(
      (response: ResponseArray) =>{
        if(response.code=== "00"){
          this.insurance = response.data;
          this.messageService.add({severity: 'success' , detail: response.message });
        }else{
          this.messageService.add({severity: 'error', summary: response.code , detail: response.message });
        }
      });
  }

  //Post
  sendIsurance(){
    this.obj_isurance = {
      id: 0,
      name: this.formulario.get('name')!.value,
      sum_Insured: this.formulario.get('sum_Insured')!.value,
      premium: this.formulario.get('Premium')!.value,
    }
    this.isuranceService.postInsurance(this.obj_isurance).subscribe(
      (Response) => {
        if(Response.code === "00"){
          this.messageService.add({severity: 'success' , detail: Response.message });
          this.listInsurance();
          this.clearForm();
          document.querySelector('.popup')?.classList.remove('open-popup')
        }else{
          this.messageService.add({severity: 'error', detail: Response.message });
        }
      });
  }
  
  handleFileInput(event: any) {
    this.fileToUpload = event.target.files[0];
  }
  uploadFile() {
    if (this.fileToUpload) {
      this.isuranceService.postInsuranceFile(this.fileToUpload).subscribe(
        (response) => {
          if(response.code=== "00"){
            this.messageService.add({severity: 'success' , detail: response.message });
            this.listInsurance();
            this.closePopup();
          }else{
            this.messageService.add({severity: 'error' , detail: response.message });
          }
        });
    }
  }

  //Put
  updateIsurance(){
    this.obj_isurance = {
      id: this.id,
      name: this.formulario.get('name')!.value,
      sum_Insured: this.formulario.get('sum_Insured')!.value,
      premium: this.formulario.get('Premium')!.value,
    }
    this.isuranceService.putInsurance(this.obj_isurance).subscribe(
      (response) => {
        if(response.code === "00"){
          this.messageService.add({severity: 'success', detail: response.message });
          this.listInsurance();
          this.clearForm();
          document.querySelector('.editpopup')?.classList.remove('editopen-popup')
        }else{
          this.messageService.add({severity: 'error', detail: response.message });
        }
      });
  }

  //Delete
  eliminarSeguro(id:number){
    this.isuranceService.deleteInsurance(id).subscribe(
      (response) => {
        if(response.code=== "00"){
          this.listInsurance();
          this.messageService.add({severity: 'success' , detail: response.message });
        }else{
          this.messageService.add({severity: 'error' , detail: response.message });
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
  openEditPopup(insurance:Insurance){
    this.id = insurance.id;
    this.formulario.patchValue({
      name: insurance.name,
      sum_Insured: insurance.sum_Insured,
      Premium: insurance.premium
    });
    document.querySelector('.editpopup')?.classList.add('editopen-popup')
  }
  closeEditPopup()
  {
    document.querySelector('.editpopup')?.classList.remove('editopen-popup')
  }
}
