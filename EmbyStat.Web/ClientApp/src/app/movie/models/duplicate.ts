export class Duplicate {
  public number: number;
  public itemOne: Item;
  public itemTwo: Item;
  public title: string;
  public reason: string;
}

export class Item {
  public dateCreated: Date;
  public quality: string;
  public id: string;
}
