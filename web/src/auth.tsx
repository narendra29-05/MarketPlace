// Authentication removed — the app runs as a fixed local identity that matches
// the backend's LocalUser (seeded admin, Users.Id = 1).

export interface AuthUser {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
}

export const LOCAL_USER: AuthUser = {
  userId: 1,
  email: "admin@findly.local",
  firstName: "Findly",
  lastName: "Admin",
};

export function useAuth(): { user: AuthUser } {
  return { user: LOCAL_USER };
}
