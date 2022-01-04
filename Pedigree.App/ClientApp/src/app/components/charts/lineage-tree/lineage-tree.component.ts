import { Component, OnInit, ViewChild, ElementRef, Input, OnChanges } from '@angular/core';
declare var d3;
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';

@Component({
  selector: 'app-lineage-tree',
  templateUrl: './lineage-tree.component.html',
  styleUrls: ['./lineage-tree.component.css']
})
export class LineageTreeComponent implements OnInit, OnChanges {

  @ViewChild('chart', { static: true })
  private chartContainer: ElementRef;

  @Input()
  chartData: HorseHeirarchy;
  private margin: any = { top: 20, right: 120, bottom: 20, left: 120 };
  private width: number;
  private height: number;
  private tree: any;
  private svg: any;
  private diagonal: any;
  diameter = 960;
  i = 0;
  duration = 750;
  root: any;
  constructor() { }

  ngOnInit() {

    if (this.chartData) {
      this.generateChart();
    }
  }

  ngOnChanges() {

    if (this.chartData) {
      this.generateChart();
    }
  }

  generateChart = () => {
    const element = this.chartContainer.nativeElement;
    
    //var div = d3.select(element)
    //  .append("div") // declare the tooltip div
    //  .attr("class", "tooltip")
    //  .style("opacity", 0);

    this.width = 960 - this.margin.right - this.margin.left;
    this.height = 800 - this.margin.top - this.margin.bottom;

    this.tree = d3.layout.tree()
      .size([this.height, this.width]);

    this.svg = d3.select(element).append("svg")
      .attr("width", this.width + this.margin.right + this.margin.left)
      .attr("height", this.height + this.margin.top + this.margin.bottom)
      .append("g")
      .attr("transform", "translate(" + this.margin.left + "," + this.margin.top + ")");

    this.diagonal = d3.svg.diagonal()
      .projection(function (d) { return [d.y, d.x] });

    this.root = this.chartData;
    console.log(this.root);
    //select2_data = extract_select2_data(values, [], 0)[1];//I know, not the prettiest...
    this.root.x0 = this.height / 2;
    this.root.y0 = 0;
    this.root.children.forEach(this.collapse);
    this.update(this.root);

  }
  //recursively collapse children
  public collapse = (d) => {
    if (d.children) {
      d._children = d.children;
      d._children.forEach(this.collapse);
      d.children = null;
    }
  }

  // Toggle children on click.
  public click = (d) => {
    if (d.children) {
      d._children = d.children;
      d.children = null;
    }
    else {
      d.children = d._children;
      d._children = null;
    }
    this.update(d);
  }

  public openPaths = (paths) => {
    for (var i = 0; i < paths.length; i++) {
      if (paths[i].id !== "1") {//i.e. not root
        paths[i].class = 'found';
        if (paths[i]._children) { //if children are hidden: open them, otherwise: don't do anything
          paths[i].children = paths[i]._children;
          paths[i]._children = null;
        }
        this.update(paths[i]);
      }
    }
  }

  public update = (source) => {
    // Compute the new tree layout.
    var nodes = this.tree.nodes(this.root).reverse();
     var links = this.tree.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach((d) => { d.y = d.depth * 180; });

    // Update the nodesâ€¦
    var node = this.svg.selectAll("g.node")
      .data(nodes, (d) =>{ return d.id || (d.id = ++this.i); });

    // Enter any new nodes at the parent's previous position.
    var nodeEnter = node.enter().append("g")
      .attr("class", "node")
      .attr("transform", (d) => { return "translate(" + source.y0 + "," + source.x0 + ")"; })
      .on("click", this.click);

    nodeEnter.append("circle")
      .attr("r", 1e-6)
      .style("fill", (d) => { return d._children ? "lightsteelblue" : "#fff"; });

    nodeEnter.append("text")
      .attr("x", (d) => { return d.children || d._children ? -10 : 10; })
      .attr("dy", ".35em")
      .attr("text-anchor", (d) => { return d.children || d._children ? "end" : "start"; })
      .text((d) => { return d.name; })
      .style("fill-opacity", 1e-6);

    // Transition nodes to their new position.
    var nodeUpdate = node.transition()
      .duration(this.duration)
      .attr("transform", (d) => { return "translate(" + d.y + "," + d.x + ")"; });

    nodeUpdate.select("circle")
      .attr("r", 4.5)
      .style("fill", (d) => {
        if (d.class === "found") {
          return "#ff4136"; //red
        }
        else if (d._children) {
          return "lightsteelblue";
        }
        else {
          return "#fff";
        }
      })
      .style("stroke", (d) => {
        if (d.class === "found") {
          return "#ff4136"; //red
        }
      });

    nodeUpdate.select("text")
      .style("fill-opacity", 1);

    // Transition exiting nodes to the parent's new position.
    var nodeExit = node.exit().transition()
      .duration(this.duration)
      .attr("transform",  (d) => { return "translate(" + source.y + "," + source.x + ")"; })
      .remove();

    nodeExit.select("circle")
      .attr("r", 1e-6);

    nodeExit.select("text")
      .style("fill-opacity", 1e-6);

    // Update the linksâ€¦
    var link = this.svg.selectAll("path.link")
      .data(links,  (d) => { return d.target.id; });

    // Enter any new links at the parent's previous position.
    link.enter().insert("path", "g")
      .attr("class", "link")
      .attr("d",  (d) => {
        var o = { x: source.x0, y: source.y0 };
        return this.diagonal({ source: o, target: o });
      });

    // Transition links to their new position.
    link.transition()
      .duration(this.duration)
      .attr("d", this.diagonal)
      .style("stroke",  (d) => {
        if (d.target.class === "found") {
          return "#ff4136";
        }
      });

    // Transition exiting nodes to the parent's new position.
    link.exit().transition()
      .duration(this.duration)
      .attr("d",  (d) => {
        var o = { x: source.x, y: source.y };
        return this.diagonal({ source: o, target: o });
      })
      .remove();

    // Stash the old positions for transition.
    nodes.forEach( (d) => {
      d.x0 = d.x;
      d.y0 = d.y;
    });
  }
}
