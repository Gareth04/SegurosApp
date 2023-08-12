import { NgModule } from "@angular/core";
import { InsuranceComponent } from "./Pages/insurance/insurance.component";
import { ClientComponent } from "./Pages/client/client.component";
import { MainPageComponent } from "./Pages/main-page/main-page.component";
import { CommonModule } from "@angular/common";
import { FormsModule , ReactiveFormsModule} from "@angular/forms";
import { RouterModule } from '@angular/router';
import { PrimeNgModule } from "../PrimeNg/prime-ng.module";

@NgModule({
    declarations:[
        InsuranceComponent,
        ClientComponent,
        MainPageComponent
    ],
    imports:[
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        PrimeNgModule
    ]
})
export class SecureCoreModule{}