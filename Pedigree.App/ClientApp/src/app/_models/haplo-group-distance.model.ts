import { HaploType } from './haplo-type.model';

export class HaploGroupDistance {   
    id: number;
    title: string;
    color: string;
    types: HaploType[];
     
    firstPlaceCount: number;
    sprintPercent: number;  
    sprinterMilerPercent: number;
    intermediatePercent: number;  
    longPercent: number;
    extendedPercent: number;
}