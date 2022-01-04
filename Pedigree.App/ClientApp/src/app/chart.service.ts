import { Injectable } from '@angular/core';
import * as d3 from 'd3';
import { PlotItem } from 'src/app/_models/plot-item.model';
import { PlotChartOption } from './_models/plot-chart-option.model';


@Injectable()
export class ChartService {

  /**
   * Get text width and height
   * @param text 
   * @param fontsize 
   * @returns 
   */
  textSize(text, fontsize) {
    if (!d3) return;
    var container = d3.select('body').append('svg');
    container.append('text').attr('x', -99999).attr('y', -99999).text(text).style('font-size', `${fontsize}px`);
    var size = container.node().getBBox();
    container.remove();
    return { width: size.width, height: size.height };
  }


  drawPlotChart(options: PlotChartOption) {
    // set the dimensions and margins of the graph       
    const { chartAreaElId, chartElId, data, xAxisLabel, yAxisLabel, boxWidth, xAxisExtraInfoShow, chartHeight } = options;
    let chartEl: any = document.getElementById(chartElId);

    if (chartEl == null) {
      chartEl = document.createElementNS("http://www.w3.org/2000/svg", "svg");
      chartEl.setAttribute('id', chartElId);
      chartEl.setAttribute('width', '100%');
      chartEl.setAttribute('height', chartHeight);
      document.getElementById(chartAreaElId).appendChild(chartEl);
    }

    const clientWidth = chartEl.clientWidth;
    const clientHeight = chartEl.clientHeight;

    const margin = { top: 50, right: 30, bottom: 100, left: 60 },
      width = clientWidth - margin.left - margin.right,
      height = clientHeight - margin.top - margin.bottom;

    // append the svg object to the body of the page
    const svg = d3.select(`#${chartElId}`)
      .append("svg")
      .attr("width", width + margin.left + margin.right)
      .attr("height", height + margin.top + margin.bottom)
      .append("g")
      .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

    // Compute quartiles, median, inter quantile range min and max --> these info are then used to draw the box.
    // Show the X scale
    const xScale = d3.scaleBand()
      .range([0, width])
      .domain(data.map(d => d.title))
      .paddingInner(1)
      .paddingOuter(0.5);

    const xAxis = svg.append("g")
      .attr("transform", "translate(0," + height + ")")
      .call(d3.axisBottom(xScale))
      .selectAll("text")
      .style('font-size', '14px');

    if (xAxisExtraInfoShow) {
      svg.selectAll("x-extra-info-labels")
        .data(data)
        .enter()
        .append("text")
        .attr("x", d => xScale(d.title) - this.textSize(`(${d.info})`, 12).width / 2)
        .attr("y", height + 35)
        .text((d) => `(${d.info})`)
        .style('font-size', '12px')
        .attr("text-anchor", "center")
        .style("alignment-baseline", "middle");
    }

    if (xAxisLabel) {
      svg.append("text")
        .attr("class", "title")
        .attr("x", width / 2 - this.textSize(xAxisLabel, 14).width / 2)
        .attr("y", height + 70)
        .text(xAxisLabel);
    }

    // Show the Y scale
    const yScale = d3.scaleLinear()
      .domain([d3.min(data.map(d => d.min)) - 0.1, d3.max(data.map(d => d.max)) + 0.1])
      .range([height, 0]);
    const yAxis = svg.append("g").call(d3.axisLeft(yScale));

    svg.append("text")
      .attr("transform", "rotate(-90)")
      .attr("y", 0 - margin.left)
      .attr("x", 0 - (height / 2))
      .attr("dy", "1em")
      .style("text-anchor", "middle")
      .text(yAxisLabel);

    // Show the main vertical line
    svg
      .selectAll("vertLines")
      .data(data)
      .enter()
      .append("line")
      .attr("x1", (d) => xScale(d.title))
      .attr("x2", (d) => xScale(d.title))
      .attr("y1", (d) => yScale(d.min))
      .attr("y2", (d) => yScale(d.max))
      .attr("stroke", "black")
      .style("width", 40)

    // rectangle for the main box
    const infoText = svg
      .selectAll("info")
      .data(data)
      .enter()
      .append("text")
      .attr("x", (d) => xScale(d.title) - 32)
      .attr("y", (d) => yScale(d.max) - 45)
      .style('font-size', '12px');

    // Add a <tspan class="title"> for every data element.
    infoText.append("tspan").text(d => `Max: ${d.max.toFixed(4)}`).attr("x", (d) => xScale(d.title) - 32).attr('dy', '.6em');
    infoText.append("tspan").text(d => `Avg: ${d.median.toFixed(4)}`).attr("x", (d) => xScale(d.title) - 32).attr('dy', '1.2em');
    infoText.append("tspan").text(d => `Min: ${d.min.toFixed(4)}`).attr("x", (d) => xScale(d.title) - 32).attr('dy', '1.2em');

    svg
      .selectAll("boxes")
      .data(data)
      .enter()
      .append("rect")
      .attr("x", (d) => xScale(d.title) - boxWidth / 2)
      .attr("y", (d) => yScale(d.q3))
      .attr("height", (d) => yScale(d.q1) - yScale(d.q3))
      .attr("width", boxWidth)
      .attr("stroke", "black")
      .style("fill", "#9767eb");

    // Show the median
    svg
      .selectAll("medianLines")
      .data(data)
      .enter()
      .append("line")
      .attr("x1", (d) => xScale(d.title) - boxWidth / 2)
      .attr("x2", (d) => xScale(d.title) + boxWidth / 2)
      .attr("y1", (d) => yScale(d.median))
      .attr("y2", (d) => yScale(d.median))
      .attr("stroke", "black")
      .style("width", 80);
  }
}
