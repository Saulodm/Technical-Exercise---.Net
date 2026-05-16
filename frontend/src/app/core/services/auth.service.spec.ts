import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, LoginResponse } from '../../models/auth.models';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const mockLoginResponse: LoginResponse = {
    idUser: 1,
    token: 'test-token-123',
    username: 'testuser',
    email: 'test@example.com',
  };

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AuthService,
      ],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should send a POST request and store session', () => {
      const loginRequest: LoginRequest = {
        email: 'test@example.com',
        password: 'password123',
      };

      service.login(loginRequest).subscribe((response) => {
        expect(response).toEqual(mockLoginResponse);
        expect(localStorage.getItem('auth_token')).toBe('test-token-123');
        expect(service.isAuthenticated()).toBeTrue();
        expect(service.currentUser()?.username).toBe('testuser');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Auth/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(loginRequest);
      req.flush(mockLoginResponse);
    });
  });

  describe('register', () => {
    it('should send a POST request and store session', () => {
      const registerRequest: RegisterRequest = {
        username: 'testuser',
        email: 'test@example.com',
        password: 'password123',
      };

      service.register(registerRequest).subscribe((response) => {
        expect(response).toEqual(mockLoginResponse);
        expect(localStorage.getItem('auth_token')).toBe('test-token-123');
        expect(service.isAuthenticated()).toBeTrue();
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/Auth/register`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(registerRequest);
      req.flush(mockLoginResponse);
    });
  });

  describe('logout', () => {
    it('should clear session and set auth state to null', () => {
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('auth_user', JSON.stringify(mockLoginResponse));

      service.logout();

      expect(localStorage.getItem('auth_token')).toBeNull();
      expect(localStorage.getItem('auth_user')).toBeNull();
      expect(service.isAuthenticated()).toBeFalse();
      expect(service.currentUser()).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    it('should return false when no token exists', () => {
      expect(service.isAuthenticated()).toBeFalse();
    });

    it('should return true when token exists', () => {
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('auth_user', JSON.stringify(mockLoginResponse));
      expect(service.isAuthenticated()).toBeTrue();
    });
  });

  describe('getToken', () => {
    it('should return the stored token', () => {
      localStorage.setItem('auth_token', 'test-token');
      expect(service.getToken()).toBe('test-token');
    });

    it('should return null when no token is stored', () => {
      expect(service.getToken()).toBeNull();
    });
  });
});
