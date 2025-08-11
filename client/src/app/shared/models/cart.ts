import { nanoid } from 'nanoid';

export type CartType = {
    id:string;
    items: CartItem[];
    deliveryMethodId?:number;
    paymentIntentId?:string;
    clientSecret?:string;
};

export type CartItem = {
    productId: number;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
};

export class Cart implements CartType {
    id = nanoid();
    items: CartItem[] = [];   
    deliveryMethodId?:number;
    paymentIntentId?:string;
    clientSecret?:string;

    addItem(item: CartItem): void {
        const existingItem = this.items.find(i => i.productId === item.productId);
        if (existingItem) {
            existingItem.quantity += item.quantity;
        } else {
            this.items.push(item);
        }
    }

    removeItem(productId: number): void {
        this.items = this.items.filter(item => item.productId !== productId);
    }

    get totalQuantity(): number {
        return this.items.reduce((sum, item) => sum + item.quantity, 0);
    }

    get totalPrice(): number {
        return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    }
}
