
import { Image } from './image';
import { StockAccessory } from './stock-accessory';

export interface StockItem {
  id: number;
  regNo: string;
  make: string;
  model: string;
  modelYear: number;
  kms: number;
  colour: string;
  vin: string;
  retailPrice: number;
  costPrice: number;
  accessories: StockAccessory[];
  images: Image[];
  dtCreated: string; // ISO date string
  dtUpdated: string; // ISO date string
}
