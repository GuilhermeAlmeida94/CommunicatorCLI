import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegistryComponent } from './registry/registry.component';
import { RegistriesSideBarComponent } from './registries-nav-bar/registries-nav-bar.component';
import { TerminalComponent } from './terminal/terminal.component';
import { HistoricComponent } from './historic/historic.component';
import { ContentComponent } from './content/content.component';
import { WebSocketService } from './_services/web-socket.service';
import { CollapseModule } from 'ngx-bootstrap/collapse';

@NgModule({
  declarations: [
    AppComponent,
      RegistryComponent,
      RegistriesSideBarComponent,
      TerminalComponent,
      HistoricComponent,
      ContentComponent
   ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    FlexLayoutModule,
    MatSidenavModule,
    MatListModule,
    MatButtonModule,
    BrowserAnimationsModule,
    CollapseModule.forRoot()
  ],
  providers: [
    WebSocketService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
