import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistroComponent } from './registro/registro.component';
import { RegistroAdminComponent } from './registroAdmin/registroAdmin.component';
import { LoginComponent } from './login/login.component';
import { OutliersComponent } from './estadisticas/outliers/outliers.component';
import { SpaghettiComponent } from './estadisticas/spaghetti/spaghetti.component';
import { GraphComponent } from './estadisticas/graph/graph.component';
import { MapComponent } from './estadisticas/map/map.component';
import { ClustersComponent } from './estadisticas/clusters/clusters.component';
import { VolumetryComponent } from './estadisticas/volumetry/volumetry.component';
import { TableComponent } from './estadisticas/table/table.component';
import { OutliersDetailComponent } from './estadisticas/outliers/outliers-detail/outliers-detail.component';
import { EstadisticasComponent } from './estadisticas/estadisticas.component';
import { ClientesComponent } from './estadisticas/clientes/clientes.component';
import { DivisasComponent } from './estadisticas/divisas/divisas.component';
import { AuthGuard } from './auth.guard';
import { UsersInfoComponent } from './menu/usuarios/users-info/users-info.component';
import { EstadisticaComponent } from './estadistica/estadistica.component';
import { HomeComponent } from './home/home.component';
import { DashboardComponent } from './monitor/dashboard.component';
import { PerfilComponent } from '../perfil/perfil.component';
import { DashboardClientesComponent } from './client-access-monitor/client-access-monitor.component';

const routes: Routes = [
  { path: "login", component: LoginComponent},
  { path: "users", component: UsersInfoComponent},
  { path: 'home', component: HomeComponent },
  {path: 'perfil', component: PerfilComponent},
  { path: "registro", component: RegistroComponent },
  { path: "registroadmin", component: RegistroAdminComponent },
  { path: 'users-info', component: UsersInfoComponent, canActivate: [AuthGuard], },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard], },
  { path: 'dashboardclientes', component: DashboardClientesComponent},
  { path: "estadistica", component: EstadisticaComponent},
  { path: "estadisticas", component: EstadisticasComponent,
    children: [
      { path: 'outliers', component: OutliersComponent },
      { path: 'outliers/:id', component: OutliersDetailComponent },
      { path: 'volumetry', component: VolumetryComponent },
      { path: 'clusters', component: ClustersComponent },
      { path: 'map', component: MapComponent },
      { path: 'graph', component: GraphComponent },
      { path: 'spaghetti', component: SpaghettiComponent },
      { path: 'table', component: TableComponent },
      { path: 'clientes', component: ClientesComponent },
      { path: 'clientes/:id', component: ClientesComponent },
      { path: 'divisas', component: DivisasComponent },
    ],
  },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', redirectTo: '/home', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
