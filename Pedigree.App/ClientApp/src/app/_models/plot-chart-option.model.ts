import { PlotItem } from "./plot-item.model";

export class PlotChartOption {
    xAxisLabel?: string; 
    yAxisLabel: string; 
    chartAreaElId: string;
    chartElId: string; 
    data: PlotItem[]; 
    boxWidth?: number;
    xAxisExtraInfoShow?: boolean = false;
    chartHeight?: string;
  }
  