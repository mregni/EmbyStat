export class ListToQueryParam {
  public static convert(name: string, list: string[]): string {
    let params = '?';
    list.forEach(item => params += `${name}=${item}&`);
    return params.slice(0, -1);
  }
}
