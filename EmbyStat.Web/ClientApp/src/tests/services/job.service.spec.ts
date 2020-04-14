import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { getTestBed, TestBed } from '@angular/core/testing';

import { JobService } from '../../app/shared/services/job.service';
import { Job } from '../../app/shared/models/jobs/job';

describe('JobService', () => {
  let injector: TestBed;
  let service: JobService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [JobService]
    });
    injector = getTestBed();
    service = injector.inject(JobService);
    httpMock = injector.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fire the correct job', () => {
    service.fireJob('0').subscribe();

    const req = httpMock.expectOne('/api/job/fire/0');
    expect(req.request.method).toBe('POST');
  });

  it('should return list of jobs', () => {
    const jobListMock = [
      new Job(),
      new Job()
    ];

    service.getAll().subscribe((jobs: Job[]) => {
      expect(jobs).toEqual(jobListMock);
    });

    const req = httpMock.expectOne('/api/job');
    expect(req.request.method).toBe('GET');
    req.flush(jobListMock);
  });

  it('should return correct job', () => {
    const jobMock = new Job();
    jobMock.id = '1';

    service.getById('1').subscribe((job: Job) => {
      expect(job).toEqual(jobMock);
    });

    const req = httpMock.expectOne('/api/job/1');
    expect(req.request.method).toBe('GET');
    req.flush(jobMock);
  });

  it('should return the media sync job', () => {
    const jobMock = new Job();
    jobMock.id = '1';

    service.getMediaSyncJob().subscribe((job: Job) => {
      expect(job).toEqual(jobMock);
    });

    const req = httpMock.expectOne('/api/job/mediasync');
    expect(req.request.method).toBe('GET');
    req.flush(jobMock);
  });

  it('should update the job trigger', () => {
    const jobMock = new Job();
    jobMock.id = '1';

    service.updateTrigger('1', '0 0 0 0 0').subscribe();

    console.log(httpMock);
    const req = httpMock.expectOne('/api/job/1?cron=0%200%200%200%200');
    expect(req.request.method).toBe('PATCH');
    req.flush(jobMock);
  });

  afterEach(() => {
    httpMock.verify();
  });
});
