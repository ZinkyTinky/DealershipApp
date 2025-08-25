export interface Image {
  id: number;
  name: string;
  imageBinary?: File | Blob;
  imageUrl?: string;
  previewUrl?: string;
}
