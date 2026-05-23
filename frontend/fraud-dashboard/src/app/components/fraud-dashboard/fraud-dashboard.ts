import { Component, OnInit, signal } from '@angular/core';
import { FraudSignalRService } from  '../../services/fraud-signal-r.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-fraud-dashboard',
  imports: [CommonModule],
  templateUrl: './fraud-dashboard.html',
  styleUrl: './fraud-dashboard.css',
})
export class FraudDashboardComponent implements OnInit {

  events = signal<any[]>([]);

  constructor(private signalR: FraudSignalRService) {}

  ngOnInit(): void {
    this.signalR.startConnection();

    this.signalR.fraudEvents$.subscribe(event => {
      this.events.update(events => [event, ...events]);
    });
  }
}