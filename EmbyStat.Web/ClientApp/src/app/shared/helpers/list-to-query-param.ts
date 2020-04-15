export class ListToQueryParam {
  public static convert(name: string, list: string[], firstParam = true): string {
    let params = '';
    if (firstParam) {
      params = '?';
    } else {
      params = '&';
    }

    list.forEach(item => params += `${name}=${item}&`);
    return params.slice(0, -1);
  }
}
