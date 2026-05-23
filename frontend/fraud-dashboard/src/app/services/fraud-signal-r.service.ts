import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FraudSignalRService {
  private hubConnection!: signalR.HubConnection;

  public fraudEvents$ = new Subject<any>();

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5001/hubs/fraud')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Error', err));

    this.hubConnection.on('fraudEvent', (data) => {
      this.fraudEvents$.next(data);
    });
  }
}
