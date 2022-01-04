import _ from 'lodash';
import { Component, OnInit, ViewChild, ElementRef, Input, OnChanges, AfterViewInit } from '@angular/core';
import * as d3 from 'd3';
import flareJons from './flare.json'
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';
import { HaploGroup } from '../../../_models/haplo-group.model';
import { element } from 'protractor';
import { ChartService } from 'src/app/chart.service';
@Component({
  selector: 'app-lineage-graph',
  templateUrl: './lineage-graph.component.html',
  styleUrls: ['./lineage-graph.component.css']
})
export class LineageGraphComponent implements AfterViewInit {

  @ViewChild('pedigreeChartContainer', { static: true }) private pedigreeChartContainer: ElementRef;
  @ViewChild('horseHaploGroupChartContainer', { static: true }) private horseHaploGroupChartContainer: ElementRef;
  @ViewChild('populationHaploGroupChartContainer', { static: true }) private populationHaploGroupChartContainer: ElementRef;
  @Input() inputChartData: HorseHeirarchy;
  @Input() inputPopulationHaploGroups: HaploGroup[] = [];
  @Input() inputHorseHaploGroups: HaploGroup[] = [];

  public chartData: any;
  public populationHaploGroups: HaploGroup[] = [];
  public horseHaploGroups: HaploGroup[] = [];
  private margin = ({ top: 5, right: 200, bottom: 10, left: 120 });
  private duration = 700;
  private i = 0;
  private root: any;
  private tree: any;
  private pedigreeChartG: any;
  private pedigreeChartWidth: number;
  private tooltipDiv: any;
  private horseHaploGroupTooltip: any;
  private populationHaploGroupTooltip: any;

  constructor(
    private window: Window,
    private chartService: ChartService
  ) { }

  ngAfterViewInit() {

    this.horseHaploGroupTooltip = d3.select(this.horseHaploGroupChartContainer.nativeElement)
      .append("div")
      .attr("class", "d3tooltip")
      .style("display", "none");

    this.populationHaploGroupTooltip = d3.select(this.populationHaploGroupChartContainer.nativeElement)
      .append("div")
      .attr("class", "d3tooltip")
      .style("display", "none");
  }

  ngOnChanges() {
    if (!_.isEqual(this.chartData, this.inputChartData)) {
      this.chartData = this.inputChartData;
      this.generateChart();
    }

    if (!_.isEqual(this.populationHaploGroups, this.inputPopulationHaploGroups)) {
      this.populationHaploGroups = this.inputPopulationHaploGroups;
      this.drawHaploGroupChart({
        svgElId: 'population-haplo-group-chart',
        data: this.populationHaploGroups,
        tooltip: this.populationHaploGroupTooltip,
        title: 'Population HaploGroup Distribution'
      });
    }

    if (!_.isEqual(this.horseHaploGroups, this.inputHorseHaploGroups)) {
      this.horseHaploGroups = this.inputHorseHaploGroups;
      this.drawHaploGroupChart({
        svgElId: 'horse-haplo-group-chart',
        data: this.horseHaploGroups,
        tooltip: this.horseHaploGroupTooltip,
        title: this.root.data.name + ' MtDNA HaploGroup Distribution'
      });
    }
  }

  generateChart() {
    const element = this.pedigreeChartContainer.nativeElement;
    const pedigreeChartNode = document.getElementById('pedigree-chart');

    const width = pedigreeChartNode.clientWidth;
    const height = pedigreeChartNode.clientHeight;

    const svg = d3.select('#pedigree-chart');

    svg.selectAll("*").remove();

    this.pedigreeChartG = svg//.call((d) => this.responsivefy(d))
      .append('g')
      .attr('transform', `translate(${this.margin.left}, ${this.margin.top})`);

    this.tree = d3.tree().size([height, width]);
    this.root = d3.hierarchy(this.chartData, (d) => { return d.children; });
    this.root.x0 = width / 2;
    this.root.y0 = 0;

    // Commented for now as we want the data to be auto expanded
    //this.root.children.forEach((c) => this.collapse(c));

    this.pedigreeChartWidth = width;
    this.update(this.root);
  }

  // Collapse the node and all it's children
  collapse(d) {
    if (d.children) {
      d._children = d.children;
      d._children.forEach((c) => this.collapse(c));
      d.children = null;
    }
  }

  // Creates a curved (diagonal) path from parent to the child nodes
  diagonal(s, d) {

    var path = `M ${s.y} ${s.x}
            C ${(s.y + d.y) / 2} ${s.x},
              ${(s.y + d.y) / 2} ${d.x},
              ${d.y} ${d.x}`

    return path
  }

  // Toggle children on click.
  click(d) {
    if (d.children) {
      d._children = d.children;
      d.children = null;
    } else {
      d.children = d._children;
      d._children = null;
    }
    this.update(d);
  }

  mouseover(d) {
    this.tooltipDiv = d3.select(this.pedigreeChartContainer.nativeElement).append("div")
      .attr("class", "d3tooltip")
      .style("opacity", 1)
      .style("left", (d.y + 140) + "px")
      .style("top", (d.x - 20) + "px")
      .html(
        "<table style='font-size: 11px;' >" +
        "<tr><td>Sex: </td><td>" + d.data.sex + "</td></tr>" +
        "<tr><td>Age: </td><td>" + d.data.age + "</td></tr>" +
        "<tr><td>From: </td><td>" + d.data.country + "</td></tr>" +
        "<tr><td>COI: </td><td>" + (d.data.coi * 100).toFixed(4) + "</td></tr>" +
        "<tr><td>Pedigcomp: </td><td>" + d.data.pedigcomp + "%</td></tr>" +
        "<tr><td>GI: </td><td>" + d.data.gi + "</td></tr>" +
        (d.data.bal ? ("<tr><td>Bal: </td><td>" + d.data.bal.toFixed(4) + "</td></tr>") : '') +
        (d.data.ahc ? ("<tr><td>AHC: </td><td>" + d.data.ahc.toFixed(4) + "</td></tr>") : '') +
        (d.data.kal ? ("<tr><td>Kal: </td><td>" + d.data.kal.toFixed(4) + "</td></tr>") : '') +
        (d.data.zHistoricalBPR ? ("<tr><td>Historical Z-Score: </td><td>" + d.data.zHistoricalBPR.toFixed(4) + "</td></tr>") : '') +
        (d.data.zCurrentBPR ? ("<tr><td>Current Z-Score: </td><td>" + d.data.zCurrentBPR.toFixed(4) + "</td></tr>") : '') +
        "<tr><td>MtDNA: </td><td>" + (d.data.mtDNATitle) + "</td></tr>" +
        "</table>"
      );
    //if (d.parent) this.mouseover(d.parent);
  }

  update(source) {
    // Assigns the x and y position for the nodes
    var treeData = this.tree(this.root);

    // Compute the new tree layout.
    var nodes = treeData.descendants(),
      links = treeData.descendants().slice(1);

    // Normalize for fixed-depth.
    nodes.forEach((d) => { d.y = d.depth * (this.pedigreeChartWidth - this.margin.left - this.margin.right) / 5 });

    // ****************** Nodes section ***************************

    // Update the nodes...
    var node = this.pedigreeChartG.selectAll('g.node')
      .data(nodes, (d) => { return d.id || (d.id = ++this.i); });

    // Enter any new modes at the parent's previous position.
    var nodeEnter = node.enter().append('g')
      .attr('class', 'node')
      .attr("transform", (d) => {
        return "translate(" + source.y0 + "," + source.x0 + ")";
      })
      .on('click', (d) => this.click(d));

    // Add Circle for the nodes
    nodeEnter.append('circle')
      .attr('class', 'node')
      .attr('r', 1e-6)
      .style("fill", (d) => {
        return d._children ? "lightsteelblue" : "#fff";
      })
      .on("mouseover", (d) => this.mouseover(d))
      .on("mouseout", (d) => {
        this.tooltipDiv.remove();
      });

    // Add labels for the nodes
    nodeEnter.append('text')
      .attr("dy", (d) => d.children || d._children ? (d.data.sex == 'Female' ? "2em" : "-1.5em") : ".35em")
      .attr("x", (d) => {
        return d.children || d._children ? 0 : 13;
      })
      .attr("text-anchor", (d) => {
        return d.children || d._children ? "end" : "start";
      }).text((d) => { return d.data.name });

    // UPDATE
    var nodeUpdate = nodeEnter.merge(node);

    // Transition to the proper position for the node
    nodeUpdate.transition()
      .duration(this.duration)
      .attr("transform", (d) => {
        return "translate(" + d.y + "," + d.x + ")";
      });

    // Update the node attributes and style
    nodeUpdate.select('circle.node')
      .attr('r', 10)
      .style("fill", (d) => {
        return d.data.bgColor ? d.data.bgColor : "#fff";
      })
      .attr('cursor', 'pointer');


    // Remove any exiting nodes
    var nodeExit = node.exit().transition()
      .duration(this.duration)
      .attr("transform", (d) => {
        return "translate(" + source.y + "," + source.x + ")";
      })
      .remove();

    // On exit reduce the node circles size to 0
    nodeExit.select('circle')
      .attr('r', 1e-6);

    // On exit reduce the opacity of text labels
    nodeExit.select('text')
      .style('fill-opacity', 1e-6);

    // ****************** links section ***************************

    // Update the links...
    var link = this.pedigreeChartG.selectAll('path.link')
      .data(links, (d) => { return d.id; });

    // Enter any new links at the parent's previous position.
    var linkEnter = link.enter().insert('path', "g")
      .attr("class", "link")
      .attr('d', (d) => {
        var o = { x: source.x0, y: source.y0 }
        return this.diagonal(o, o)
      })
      .attr('style', (d) => {
        let style = '';
        if (d.data.mtDNAColor && d.data.sex == 'Female') {
          style += `stroke:${d.data.mtDNAColor};`;
          style += `stroke-width:2px;`;
          style += `stroke-opacity:1;`;
        }
        return style;
      });

    // UPDATE
    var linkUpdate = linkEnter.merge(link);

    // Transition back to the parent element position
    linkUpdate.transition()
      .duration(this.duration)
      .attr('d', (d) => { return this.diagonal(d, d.parent) });

    // Remove any exiting links
    var linkExit = link.exit().transition()
      .duration(this.duration)
      .attr('d', (d) => {
        var o = { x: source.x, y: source.y }
        return this.diagonal(o, o)
      })
      .remove();

    // Store the old positions for transition.
    nodes.forEach((d) => {
      d.x0 = d.x;
      d.y0 = d.y;
    });
  }



  responsivefy(svg) {
    // get container + svg aspect ratio
    var container = d3.select(svg.node().parentNode),
      width = parseInt(svg.style("width")),
      height = parseInt(svg.style("height"));
    var aspect = width / height;
    // add viewBox and preserveAspectRatio properties,
    // and call resize so that svg resizes on inital page load
    svg.attr("viewBox", "0 0 " + width + " " + height)
      .attr("preserveAspectRatio", "xMinYMid")
      .call(() => this.resize(container, svg, aspect));


    // to register multiple listeners for same event type,
    // you need to add namespace, i.e., 'click.foo'
    // necessary if you call invoke this function for multiple svgs
    // api docs: https://github.com/mbostock/d3/wiki/Selections#on
    d3.select(this.window).on("resize." + container.attr("id"), () => this.resize(container, svg, aspect));

  }

  resize(container, svg, aspect) {
    var targetWidth = parseInt(container.style("width"));
    svg.attr("width", targetWidth);
    svg.attr("height", Math.round(targetWidth / aspect));
  }
  // get width of container and resize svg to fit it


  drawHaploGroupChart(options: any) {
    const { svgElId, data, tooltip, title } = options;

    const svgNode = document.getElementById(svgElId);
    if (svgNode == null) return;

    const me = this;

    const svg: any = d3.select(`#${svgElId}`);

    svg.selectAll("*").remove();

    const margin = 60;

    const clientRect = svgNode.getBoundingClientRect();
    const width = clientRect.width - 2 * margin;
    const height = (clientRect.height - 50) - 2 * margin;

    const chart = svg.append('g')
      .attr('transform', `translate(${margin}, ${margin})`);

    const xScale = d3.scaleBand()
      .range([0, width])
      .domain(data.map((g) => g.title))
      .padding(0.5);

    const yScale = d3.scaleLinear()
      .range([height, 0])
      .domain([0, Math.max(...data.map(g => g.refPopCount))]);

    // chart.append('g')
    //   .call(d3.axisLeft(yScale));

    const xAxis = chart.append('g')
      .attr('transform', `translate(0, ${height})`)
      .call(d3.axisBottom(xScale))
      .selectAll("line")
      .style('display', 'none')
      .selectAll("text")
      .style('font-size', '18px');

    // Draw chart bar
    const bar = chart.selectAll('.bar')
      .data(data)
      .enter()
      .append('rect')
      .style("display", d => (d.refPopCount === 0 ? "none" : null))
      .attr('fill', (d) => d.color)
      .attr('x', (d) => xScale(d.title))
      .attr('width', xScale.bandwidth())
      .attr('y', (d) => yScale(0))
      .attr('height', (d) => height - yScale(0))
      .on('mouseenter', function (d) {
        d3.select(this)
          .transition()
          .duration(300)
          .attr('opacity', 0.6)
          .attr('x', xScale(d.title) - 5)
          .attr('width', xScale.bandwidth() + 10)
      })
      .on("mousemove", function (d) {
        tooltip
          .style('opacity', 1)
          .style('left', `${d3.event.pageX - clientRect.left}px`)
          .style("top", `${d3.event.pageY - 250}px`)
          .style("display", "inline-block")
          .html(
            "<table style='font-size: 11px;' >" +
            "<tr><td>Count: </td><td>" + d.refPopCount + "</td></tr>" +
            "<tr><td>Percent: </td><td>" + d.refPopCountPercent + "%</td></tr>" +
            "</table>"
          );
      })
      .on("mouseout", function (d) {
        tooltip.style("display", "none");

        d3.select(this)
          .transition()
          .duration(300)
          .attr('opacity', 1)
          .attr('x', xScale(d.title))
          .attr('width', xScale.bandwidth())
      });

    // Bar transition
    bar
      .transition()
      .ease(d3.easeLinear)
      .duration(750)
      .attr('y', d => yScale(d.refPopCount))
      .attr('height', d => height - yScale(d.refPopCount))
      .delay((d, i) => i * 150);


    // Draw bar value text
    chart.selectAll("text.bar")
      .data(data)
      .enter()
      .append("text")
      .style("display", d => { return d.refPopCount === 0 ? "none" : null; })
      .attr("class", "bar")
      .attr("text-anchor", "middle")
      .attr("x", (d) => xScale(d.title) + xScale.bandwidth() / 2)
      .attr("y", d => height)
      .attr("height", 0)
      .transition()
      .duration(750)
      .delay((d, i) => { return i * 150; })
      .attr("y", (d) => yScale(d.refPopCount) - 5)
      .text((d) => d.refPopCount);

    svg.append("text")
    .attr("text-anchor", "start")
    .attr("x", (clientRect.width - this.chartService.textSize(title, 17).width) / 2)
    .attr("y", 50)
    .attr('fill', 'black')
    .attr('font-size', 17)
    .attr('font-weight', 500)
    .text(title);

    svg.append("text")
      .attr("text-anchor", "start")
      .attr("x", (clientRect.width - this.chartService.textSize('HaploGroups', 14).width) / 2)
      .attr("y", clientRect.height - 50 - 10)
      .attr('font-size', 14)
      .attr('fill', '#673ab7')
      .text("HaploGroups");

    // Add one dot in the legend for each name.
    const size = 10;
    const legend = svg.append('g')
      .attr('transform', `translate(${(clientRect.width - data.length * (size + 50)) / 2}, 110)`);

    legend.selectAll("mydots")
      .data(data)
      .enter()
      .append("rect")
      .attr("x", (d, i) => (i * (size + 50)))
      .attr("y", 100)
      .attr("width", size)
      .attr("height", size)
      .style("fill", (d) => d.color);

    // Add one dot in the legend for each name.
    legend.selectAll("mylabels")
      .data(data)
      .enter()
      .append("text")
      .attr("x", (d, i) => (i * (size + 50) + size + 2))
      .attr("y", 100 + size / 2 + 2) // 100 is where the first dot appears. 25 is the distance between dots
      //.style("fill", (d) => d.color)
      .text((d) => d.title)
      .attr("text-anchor", "left")
      .style("alignment-baseline", "middle");

  }

}
