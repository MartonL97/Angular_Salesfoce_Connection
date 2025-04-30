import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CallbackComponent } from './callback/callback.component';  // Import the CallbackComponent
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  { path: 'loging', component: LoginComponent, title: "Log In" },
  { path: 'callback', component: CallbackComponent },
  { path: 'home', component: HomeComponent, title:"Home" },
  { path: '', redirectTo: '/loging', pathMatch: 'full' }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
