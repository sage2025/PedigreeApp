import {HaploType} from './haplo-type.model';

export class HaploGroup {
    id: number;
    title: string;
    color: string;
    types: HaploType[];

    refPopCount: number;
    refPopCountPercent: number;
    ratedHorses: number;
    ratedHorsesPercent: number;
    threePlusStarts: number;
    threePlusStartsPercent: number;
    elite: number;
    elitePercent: number;
    nonElite: number;
    nonElitePercent: number;
}