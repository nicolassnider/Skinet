import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'address'
})
export class AddressPipe implements PipeTransform {
  transform(value: any, type: 'shipping' | 'billing' = 'shipping'): string {
    console.log('AddressPipe input:', value, type);

    let addressObj: any;
    let name: string | undefined;

    if (!value) return 'Unknown address';

    if (type === 'shipping' && value.shipping) {
      addressObj = value.shipping.address;
      name = value.shipping.name;
    } else if (type === 'billing' && value.payment_method_preview?.billing_details) {
      addressObj = value.payment_method_preview.billing_details.address;
      name = value.payment_method_preview.billing_details.name;
    } else if (value.address && value.name) {
      addressObj = value.address;
      name = value.name;
    }

    if (addressObj && name) {
      const { line1, line2, city, state, country, postal_code } = addressObj;
      return `${name}, ${line1}${line2 ? ', ' + line2 : ''}, ${city}, ${state}, ${postal_code}, ${country}`;
    } else {
      return 'Unknown address';
    }
  }
}
