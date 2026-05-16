import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        provideNoopAnimations(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have an invalid form when empty', () => {
    expect(component.form.valid).toBeFalse();
  });

  it('should validate email field', () => {
    const emailControl = component.email;
    expect(emailControl.valid).toBeFalse();

    emailControl.setValue('invalid-email');
    expect(emailControl.hasError('email')).toBeTrue();

    emailControl.setValue('valid@email.com');
    expect(emailControl.valid).toBeTrue();
  });

  it('should validate password field', () => {
    const passwordControl = component.password;
    expect(passwordControl.valid).toBeFalse();

    passwordControl.setValue('somepassword');
    expect(passwordControl.valid).toBeTrue();
  });

  it('should disable submit button when form is invalid', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const submitButton = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(submitButton.disabled).toBeTrue();
  });

  it('should enable submit button when form is valid', () => {
    component.email.setValue('test@example.com');
    component.password.setValue('password123');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const submitButton = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(submitButton.disabled).toBeFalse();
  });

  it('should not call onSubmit when form is invalid', () => {
    spyOn(component, 'onSubmit');
    const compiled = fixture.nativeElement as HTMLElement;
    const form = compiled.querySelector('form')!;
    form.dispatchEvent(new Event('submit'));
    expect(component.onSubmit).not.toHaveBeenCalled();
  });
});
