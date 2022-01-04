import { Horse } from "./horse.model";

export class HorseWithParentChildren {
  mainHorse: Horse;
  parents: Horse[];
  children: Horse[];
}
