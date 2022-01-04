export class HorseHeirarchy {
  id: number;
  oId: string;
  name: string;
  age: number;
  sex: string;
  country: string;
  family: string;
  mtDNATitle: string;
  mtDNAColor: string;
  bestRaceClass: string;
  father: string;
  mother: string;
  sireOfDam: string;
  isLeaf: boolean;
  bgColor: string;
  coi: number;
  pedigcomp: number;
  gi: number;
  children: HorseHeirarchy[] = [];
}
