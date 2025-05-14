import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { SuccessPageComponent } from './success-page/success-page.component';


const routes: Routes = [

  { path: 'login', component: LoginComponent },
  { path: 'success', component: SuccessPageComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' } // Redirect to login by default

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
