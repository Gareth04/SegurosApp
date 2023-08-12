import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { MainPageComponent } from "./SecureCore/Pages/main-page/main-page.component";
import { ClientComponent } from "./SecureCore/Pages/client/client.component";
import { InsuranceComponent } from "./SecureCore/Pages/insurance/insurance.component";

const routes: Routes = [
    {
      path:'',
      component: MainPageComponent,
      pathMatch: 'full',
    },
    {
      path:'client',
      component: ClientComponent,
      pathMatch: 'full',
    },
    {
      path:'insurance',
      component: InsuranceComponent,
      pathMatch: 'full',
    },
    {
      path:'main',
      component: MainPageComponent,
      pathMatch: 'full',
    },
    {//necesito procesar si la persona ingresa a una pagina de forma incorrecta, para eso es esta ruta
      path:'**',
      redirectTo:'main'
  }
  ];

@NgModule({
    imports:[RouterModule.forRoot(routes)],
    exports:[RouterModule]
})
export class AppRoutingModule{}