import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';

import { map, of } from 'rxjs';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  deliveryMethods: DeliveryMethod[] = [];
  private http = inject(HttpClient);
  
  getDeliveryMethods(){
    if(this.deliveryMethods.length>0) return of(this.deliveryMethods);
    return this.http.get<DeliveryMethod[]>(this.baseUrl+'payments/delivery-methods').pipe(
      map(methods => {
        this.deliveryMethods = methods;
        return methods;
      })
    );
  }
}
