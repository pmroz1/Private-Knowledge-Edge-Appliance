import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    private http = inject(HttpClient);
    private baseUrl = 'http://localhost:5000';

    upload(file: File): Observable<any> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post(`${this.baseUrl}/upload`, formData);
    }

    ask(question: string): Observable<{ answer: string }> {
        return this.http.post<{ answer: string }>(`${this.baseUrl}/chat`, { question });
    }
}
