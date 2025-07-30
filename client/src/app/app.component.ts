import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{  
  baseUrl = 'https://localhost:4999/api/';
  private http = inject(HttpClient);
  protected readonly title = signal('Skinet');
  products : Product[] = [];

  ngOnInit(): void {
    this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({
      next: (response) => {        
        this.products = response.data;
      },
      error: (error) => {
        console.error('Error fetching products:', error);
      },
      complete:()=> console.log('Product fetch complete')
    });
  }
}
