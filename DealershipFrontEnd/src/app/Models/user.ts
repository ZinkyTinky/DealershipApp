export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  token?: string; // JWT token if returned on login
}
