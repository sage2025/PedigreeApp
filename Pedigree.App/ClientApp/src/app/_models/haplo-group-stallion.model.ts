import { HaploGroup } from './haplo-group.model';
import { HaploType } from './haplo-type.model';

export class HaploGroupStallion {   
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
     
    g1Wnr: number;
    g1WnrPercent: number;  
    g2Wnr: number;
    g2WnrPercent: number;  
    g3Wnr: number;
    g3WnrPercent: number;
    sWnr: number;
    sWnrPercent: number;
}