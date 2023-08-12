import { Insurance } from './Insurance.interface';
export interface ClientI{
    id_client: number,
    id_Insurance: number,
    cedula: string,
    name: string,
    phone: string,
    age: number
}