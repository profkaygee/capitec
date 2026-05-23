import { Routes } from '@angular/router';
import { FraudDashboardComponent } from './components/fraud-dashboard/fraud-dashboard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: FraudDashboardComponent
      }
];
