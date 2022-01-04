import { Component, OnInit, ViewChild, ElementRef, Input, OnChanges } from '@angular/core';
import * as d3 from 'd3';
import flareJons from './data.json'
import { HorseHeirarchy } from '../../../_models/horse-heirarchy.model';

@Component({
  selector: 'app-radial-lineage-graph',
  templateUrl: './radial-lineage-graph.component.html',
  styleUrls: ['./radial-lineage-graph.component.css']
})
export class RadialLineageGraphComponent implements OnInit {
  @ViewChild('radialchart', { static: true })
  private chartContainer: ElementRef;

  @Input()
  chartData: HorseHeirarchy;

  public data: any;
  private margin = ({ top: 20, right: 90, bottom: 30, left: 90 });
  private width = 960;
  private height = 800;
  private radius: number;
  private root: any;
  private tree: any;
  private svg: any;
  private g: any;
  private i = 0;
  private duration = 750;
  
  constructor() { }


  ngOnInit() {

    this.data = flareJons;
    this.generateChart();
    //if (this.chartData) {
    //  this.data = this.chartData;
    //  this.generateChart();
    //}
  }

  ngOnChanges() {

    //if (this.chartData) {
    //  this.data = this.chartData;
    //  this.generateChart();
    //}
  }



  generateChart() {
    
    const element = this.chartContainer.nativeElement;
    this.svg = d3.select(element).append("svg")
      .attr("width", this.width)
      .attr("height", this.height);
    this.g = this.svg.append("g").attr("transform", "translate(" + (this.width / 2 ) + "," + (this.height / 2) + ")");
    this.tree = d3.tree()
      .size([360, 250]);

    this.root = d3.hierarchy(this.data, (d) => {
      return d.children;
    });
    var ii = 0; 
    this.root.each( (d) => {
      d.name = d.data.name; //transferring name to a name variable
      d.id = ii;
      ii += ii;
    });
    this.root.x0 = this.height / 2;
    this.root.y0 = 0;

    this.update(this.root);
    //this.svg.attr("viewbox", "0, 0, 500 500");
  }
  collapse(d) {
    if (d.children) {
      d._children = d.children;
      d._children.forEach(this.collapse);
      d.children = null;
    }
  }
  project(x, y) {
    var angle = (x - 90) / 180 * Math.PI, radius = y;
    return [radius * Math.cos(angle), radius * Math.sin(angle)];
  }
  connector(d) {
    return "M" + this.project(d.x, d.y)
      + "C" + this.project(d.x, (d.y + d.parent.y) / 2)
      + " " + this.project(d.parent.x, (d.y + d.parent.y) / 2)
      + " " + this.project(d.parent.x, d.parent.y)
  }
  flatten(root) {
    // hierarchical data to flat data for force layout
    var nodes = [];
    function recurse(node) {
      if (node.children) node.children.forEach(recurse);
      if (!node.id) node.id = ++this.i;
      else ++this.i;
      nodes.push(node);
    }
    recurse(root);
    return nodes;
  }
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
  color(d) {
    return d._children ? "#3182bd" // collapsed package
      : d.children ? "#c6dbef" // expanded package
        : "#fd8d3c"; // leaf node
  }
  update(source) {


    var treeData = this.tree(this.root);

    var nodes = treeData.descendants();
    var links = nodes.slice(1);

    var nodeUpdate;
    var nodeExit;

    // Normalize for fixed-depth.
    nodes.forEach( (d) => { d.y = d.depth * 180; });

    var nodeSvg = this.g.selectAll(".node")
     .data(nodes,  (d) => { return d.id || (d.id = ++this.i); });


    //nodeSvg.exit().remove();

    var nodeEnter = nodeSvg.enter()
      .append("g")
      .attr("class", "node")
      .attr("transform",  (d) =>{ return "translate(" + this.project(d.x, d.y) + ")"; })
      .on('click', (d) => this.click(d));



    nodeEnter.append("circle")
      .attr("r", 5)
      .style("fill", (d) => this.color(d));



    nodeEnter.append("text")
      .attr("dy", "0.31em")
      .attr("x", d => d.x < Math.PI === !d.children ? 10 : -10)
      .attr("text-anchor", d => d.x < Math.PI === !d.children ? "start" : "end")
      .text((d) => { return d.data.name; })
      .clone(true).lower()
      .attr("stroke", "white");;
    

    
    var nodeUpdate = nodeSvg.merge(nodeEnter).transition()
      .duration(this.duration)
      .attr("transform",  (d) => { return "translate(" + this.project(d.x, d.y) + ")"; });


    nodeSvg.select("circle")
      .style("fill", (d) => this.color(d));


    nodeUpdate.select("text")
      .style("fill-opacity", 1);

    // Transition exiting nodes to the parent's new position.
    var nodeExit = nodeSvg.exit().transition()
      .duration(this.duration)
      .attr("transform",  (d) => { return "translate(" + source.y + "," + source.x + ")"; }) //for the animation to either go off there itself or come to centre
      .remove();

    nodeExit.select("circle")
      .attr("r", 1e-6);

    nodeExit.select("text")
      .style("fill-opacity", 1e-6);

    nodes.forEach( (d) => {
      d.x0 = d.x;
      d.y0 = d.y;
    });


    var linkSvg = this.g.selectAll(".link")
      .data(links,  (link) => { var id = link.id + '->' + link.parent.id; return id; });



    // Transition links to their new position.
    linkSvg.transition()
      .duration(this.duration)
     .attr('d', (d) => this.connector(d));

    // Enter any new links at the parent's previous position.
    var linkEnter = linkSvg.enter().insert('path', 'g')
      .attr("class", "link")
      .attr("d",  (d) => {
        return "M" + this.project(d.x, d.y)
          + "C" + this.project(d.x, (d.y + d.parent.y) / 2)
          + " " + this.project(d.parent.x, (d.y + d.parent.y) / 2)
          + " " + this.project(d.parent.x, d.parent.y);
      });
    /*
     (d) => {
var o = {x: source.x0, y: source.y0, parent: {x: source.x0, y: source.y0}};
return connector(o);
});*/



    // Transition links to their new position.
    linkSvg.merge(linkEnter).transition()
      .duration(this.duration)
      .attr("d", (d) => this.connector(d));


    // Transition exiting nodes to the parent's new position.
    linkSvg.exit().transition()
      .duration(this.duration)
      .attr("d", /* (d) => {
                        var o = {x: source.x, y: source.y, parent: {x: source.x, y: source.y}};
                        return connector(o);
                    })*/ (d) => {
                      return "M" + this.project(d.x, d.y)
                        + "C" + this.project(d.x, (d.y + d.parent.y) / 2)
                        + " " + this.project(d.parent.x, (d.y + d.parent.y) / 2)
                        + " " + this.project(d.parent.x, d.parent.y);
        })
      .remove();

                    // Stash the old positions for transition.

  }
}
