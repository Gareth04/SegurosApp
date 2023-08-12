import { HttpClient, HttpParams} from '@angular/common/http'
import { Injectable } from "@angular/core";
import { environment } from 'src/environments/environment.development';
import { ClientI } from '../Interfaces/Client.interface';
import { Observable } from 'rxjs';
import { Response, ResponseArray } from '../Interfaces/Response.interface';
import { Insurance } from '../Interfaces/Insurance.interface';

@Injectable({ 
    providedIn:'root'
})
export class InsuranceService{
    constructor(private http: HttpClient){}
    getListInsuranceByPerson(cedula: string) {
        const params = new HttpParams()
        .set('cedula',cedula)
        return this.http.get<Response>(`${environment.apiUrl}/ClientInsurance/GetClientInsuranceDetails?`,{params:params})
    }
    getListaByIdInsurance(id: number) {
        const params = new HttpParams()
        .set('insuranceCode',id)
        return this.http.get<ResponseArray>(`${environment.apiUrl}/ClientInsurance/GetClientDetailsByInsuranceCode?`,{params:params})   
    }

    //Seguros
    getListInsurance() {
        return this.http.get<ResponseArray>(`${environment.apiUrl}/Insurance`)
    }
    getObjInsuranceName(name: string){
        const params = new HttpParams()
        .set('name',name)
        console.log(name)
        return this.http.get<ResponseArray>(`${environment.apiUrl}/Insurance/nombre?`,{params:params})
    }
    getObjInsuranceById(id: number)  {
        const params = new HttpParams()
        .set('id',id)
        return this.http.get<ResponseArray>(`${environment.apiUrl}/Insurance/id?`,{params:params})
      }
    postInsurance(insurance: Insurance){
        return this.http.post<Response>(`${environment.apiUrl}/Insurance`,insurance)
    }
    deleteInsurance(id : number){
        const params = new HttpParams()
        .set('id',id)
        return this.http.delete<Response>(`${environment.apiUrl}/Insurance/${id}`);
    }
    putInsurance(insurance: Insurance){
        return this.http.put<Response>(`${environment.apiUrl}/Insurance/${insurance.id}`,insurance);
    }
    postInsuranceFile(file: File) {
        const formData : FormData = new FormData();
        formData.append('file', file);
        return this.http.post<Response>('https://localhost:7090/api/Insurance/api/file', formData);
    }

    //Client
    getListClient() {
        return this.http.get<ResponseArray>(`${environment.apiUrl}/clientes`)
    }
    postClient(client: ClientI){
        return this.http.post<Response>(`${environment.apiUrl}/clientes`, client)
    }
    putClient(client: ClientI){
        return this.http.put<Response>(`${environment.apiUrl}/clientes/${client.id_client}`,client);
      }
    deleteClient(id_client : number){
        const params = new HttpParams()
        .set('id',id_client)
        return this.http.delete<Response>(`${environment.apiUrl}/clientes?`,{params:params});
    }
    PostClientSegu(cedula: string, nombre_seg: string): Observable<any> {
        const url = `${environment.apiUrl}/ClientInsurance/addInsurance?cedula=${cedula}&insuranceName=${nombre_seg}`;
        return this.http.post(url, null);
      }
    deleteClientSegu(cedula: string, nombre_seg: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/ClientInsurance/removeInsurance?cedula=${cedula}&insuranceName=${nombre_seg}`);
    }
    uploadExcelAsg(file: File) {
        const formData : FormData = new FormData();
        formData.append('file', file);
        console.log(formData);
        return this.http.post<any>('https://localhost:7090/api/clientes/api/file', formData);
    }



}