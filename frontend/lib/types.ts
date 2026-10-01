// Shapes of the DTOs in backend/src/IMDB.Application/DTOs/Dtos.cs.

export interface Genre { id: number; title: string }

export interface Movie {
  id: number; imdbId: string; title: string; releaseDate: string; description: string;
  duration: number; genres: Genre[]; posterUrl: string; rate: number;
}

export interface Series {
  id: number; imdbId: string; title: string; releaseDate: string; description: string;
  genres: Genre[]; posterUrl: string; rate: number;
}

export interface Episode {
  id: number; imdbId: string; title: string; releaseDate: string; description: string;
  episodeNumber: number; durationMinutes: number | null; seasonId: number; posterUrl: string; rate: number;
}

export interface Season {
  id: number; imdbId: string; title: string; releaseDate: string; description: string;
  seasonNumber: number; episodesCount: number; seriesId: number; posterUrl: string; rate: number; episodes: Episode[];
}

export interface Person { id: number; imdbId: string; fullName: string; birthDate: string; bio: string | null; photoUrl: string }

export const CAST_ROLES = ["Actor", "Director", "Writer", "Producer", "Cinematographer", "Editor", "Composer", "SoundDepartment", "CostumeDesigner"] as const;
export type CastRole = (typeof CAST_ROLES)[number] | string;

export interface Cast { id: number; mediaId: number; personId: number; role: CastRole; person: Person | null }

export interface Comment { id: number; text: string; contentId: number; userId: string; userName: string | null; createdOn: string; lastUpdatedOn: string }

export const SCORES = ["Zero", "One", "Two", "Three", "Four", "Five"] as const;
export interface Rate { id: number; mediaId: number; userId: string; userName: string | null; score: (typeof SCORES)[number] | string }

export interface NewUser { userName: string; email: string; token: string }

export type Media = Movie | Series;
