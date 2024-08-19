import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

constructor(private router: Router) {}
redirectToRegisterPage() {
    this.router.navigate(['/registro']);

}

redirectToLoginPage() {
  this.router.navigate(['/login']);
}
}
