import { Race } from './race.model';

export class Position {
  id: number;
  raceId: number;
  place: number;
  horseOId: string;
  horseName: string;
  horseAge: string;
  horseSex: string;
  horseCountry: string;
  horseFamily: string;
  horseFather: string;
  horseMother: string;
  horseId: number;
  race: Race;
}
