import { ShowStatistics } from 'src/app/shared/models/show/show-statistics';

import { HttpResponse } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { Library } from '../../../shared/models/library';
import { ListContainer } from '../../../shared/models/list-container';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';
import { ShowService } from './show.service';

describe('ShowService', () => {
  let injector: TestBed;
  let service: ShowService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ShowService]
    });
    injector = getTestBed();
    service = injector.inject(ShowService);
    httpMock = injector.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch show libraries', () => {
    const libraryList = [
      new Library(),
      new Library()
    ];

    service.getLibraries().subscribe((list: Library[]) => {
      expect(list.length).toBe(2);
      expect(list).toEqual(libraryList);
    });

    const req = httpMock.expectOne('/api/show/libraries');
    expect(req.request.method).toBe('GET');
    req.flush(libraryList);
  });

  it('should fetch show statistics', () => {
    const statisticMock = new ShowStatistics();

    service.getStatistics(['1', '2']).subscribe((statistic: ShowStatistics) => {
      expect(statistic).toEqual(statisticMock);
    });

    const req = httpMock.expectOne('/api/show/statistics?libraryIds=1&libraryIds=2');
    expect(req.request.method).toBe('GET');
    req.flush(statisticMock);
  });

  it('should fetch collected rows list', () => {
    const rowsMock = [
      new ShowCollectionRow(),
      new ShowCollectionRow()
    ];

    service.getCollectedList(['1', '2'], 0).subscribe((rows: ListContainer<ShowCollectionRow>) => {
      expect(rows.data).toEqual(rowsMock);
      expect(rows.totalCount).toEqual(2);
    });

    const req = httpMock.expectOne('/api/show/collectedlist?page=0&libraryIds=1&libraryIds=2');
    expect(req.request.method).toBe('GET');

    const returnData = new ListContainer<ShowCollectionRow>();
    returnData.data = rowsMock;
    returnData.totalCount = rowsMock.length;
    req.flush(returnData);
  });

  afterEach(() => {
    httpMock.verify();
  });
});
