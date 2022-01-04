import { Race } from "./race.model";

export class Horse {
  id: number;
  oId: string;
  name: string;
  age: number;
  sex: string;
  country: string;
  family: string;
  mtDNA: number;
  mtDNAFlag: boolean;
  mtDNATitle: string;
  mtDNAColor: string;
  isFounder: boolean;
  fatherId: number;
  fatherOId: string;
  fatherName: string;
  motherId: number;
  motherOId: string;
  motherName: string;
  bmFatherId: number; // Broodmare sire
  bmFatherOId: string;
  bmFatherName: string;
  bestRaceClass: string;
  races: Race[];
  hasRelationships: boolean;
  hideRow: boolean = false;
  showHeader: boolean = false;
  showHeaderText: boolean = false;
  inbreeding: string;
  twinYear: number;
  twins: number;
  g1Wnrs: number;
  g2Wnrs: number;
  g3Wnrs: number;
  lrWnrs: number;
  totalWnrs: number;
}
