import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CargoRequestCreateComponent } from './cargo-request-create.component';

describe('CargoRequestCreateComponent', () => {
  let component: CargoRequestCreateComponent;
  let fixture: ComponentFixture<CargoRequestCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CargoRequestCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CargoRequestCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
