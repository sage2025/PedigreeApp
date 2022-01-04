import { Component, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import * as d3 from 'd3';
import { HorseService } from 'src/app/_services/horse.service';
import { HorseHeirarchy } from 'src/app/_models/horse-heirarchy.model';
import { Horse } from 'src/app/_models/horse.model';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs/operators';

@Component({
    selector: 'app-research-female-line-search',
    templateUrl: './female-line-search.component.html',
    styleUrls: ['./female-line-search.component.css']
})

export class FemaleLineSearchComponent implements AfterViewInit {
    horse: Horse = null;
    private subscription: Subscription;
    loading = false;
    showSearchClear: boolean = false;

    chartData: HorseHeirarchy;
    chartGroupEl: any;
    tree: any;
    root: any;
    i: number = 0;
    private duration = 700;

    private tooltip: any;

    @ViewChild('radialChartContainer', { static: true }) private radialChartContainer: ElementRef;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private horseService: HorseService
    ) {
        if (this.route.snapshot.params.tab == 'female_line_search') {
            const params = this.route.snapshot.queryParams;
            if (params['femaleId']) this.loadHorse(params['femaleId']);
        }
    }

    ngAfterViewInit() {

    }

    loadHorse(horseId) {
        this.horseService.getHorse(horseId).pipe(first())
            .subscribe(result => {
                this.horse = result;
                this.loadData();
            });
    }

    enableSearch() {
        return this.horse && this.horse.id;
    }

    search() {
        if (!this.enableSearch()) return;

        this.router.navigate(['research/female/female_line_search'], { queryParams: { femaleId: this.horse.id } });
        this.loadData();
    }

    clearSearchBox() {
        this.horse = null;
        this.chartData = null;
        this.showSearchClear = false;
        d3.select('#female-radial-chart').selectAll("*").remove();
    }

    loadData() {
        if (this.subscription) this.subscription.unsubscribe();

        this.loading = true;

        this.subscription = this.horseService!.getHierarchyForFemaleLineSearch(this.horse.id)
            .subscribe(data => {
                this.loading = false;
                this.chartData = data;
                this.showSearchClear = true;
                this.generateChart();
            },
                err => {
                    this.loading = false;
                    console.error(err);
                    this.showSearchClear = true;
                });
    }

    getNodeColor(d: any) {
        switch (d.data.bestRaceClass) {
            case 'G1Wnr': return 'red';
            case 'G2Wnr': return 'yellow';
            case 'G3Wnr': return 'green';
            case 'SWnr': return 'blue';
            case 'Gsp':
            case 'Lsp':
                return 'aqua';
        }

        return d._children || d.children ? "lightsteelblue" : "#fff";
    }
    generateChart() {
        const chartEl = document.querySelector('#female-radial-chart') as SVGGraphicsElement;

        const clientRect = chartEl.getBoundingClientRect();

        const width = clientRect.width;
        const height = clientRect.height;

        const radius = height / 2 - 80;

        this.tree = d3.tree()
            .size([2 * Math.PI, radius])
            .separation((a, b) => (a.parent == b.parent ? 1 : 2) / a.depth);

        this.root = d3.hierarchy(this.chartData)
            .sort((a, b) => d3.ascending(a.data.name, b.data.name));

        this.root.x0 = 0;
        this.root.y0 = 0;

        const svg = d3.select('#female-radial-chart');

        svg.selectAll("*").remove();

        this.chartGroupEl = svg.append("g").attr("transform", "translate(" + (width / 2) + "," + (height / 2) + ")");

        this.update(this.root);

        const bbox = chartEl.getBBox();
        //svg.attr('viewBox', `${bbox.x} ${bbox.y} ${bbox.width} ${bbox.height}`);
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

    mouseover(d) {
        const { x, y } = this.radialChartContainer.nativeElement.getBoundingClientRect()
        this.tooltip = d3.select(this.radialChartContainer.nativeElement)
            .append("div")
            .attr("class", "d3tooltip")
            .style("opacity", 1)
            .style("left", d3.event.pageX - x + "px")
            .style("top", d3.event.pageY - (window.pageYOffset + y) + "px")
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
    }

    update(source) {
        const tree = this.tree(this.root);

        const nodes = tree.descendants();

        const node = this.chartGroupEl.selectAll('g.node')
            .data(nodes, (d) => { return d.id || (d.id = ++this.i); });

        const nodeEnter = node.enter().append('g')
            .attr('class', 'node')
            .attr("transform", (d) => `
                rotate(${source.x0 * 180 / Math.PI - 90})
                translate(${source.y0},0)
                rotate(${source.x0 >= Math.PI ? 180 : 0})
            `)
            .on('click', (d) => this.click(d));

        nodeEnter.append('circle')
            .attr('class', 'node')
            .attr('r', 1e-6)
            .style("fill", (d: any) => this.getNodeColor(d))
            .on("mouseover", (d) => this.mouseover(d))
            .on("mouseout", (d) => {
                this.tooltip.remove();
            });


        nodeEnter.append('text')
            .attr("font-size", 10)
            .attr("dy", '0.31em')
            .attr("x", d => d.x < Math.PI === !d.children ? 6 : -6)
            .attr("text-anchor", d => d.x < Math.PI === !d.children ? "start" : "end")
            .attr("opacity", "0")
            .text((d: any) => (d.data.sex == 'Female' ? '(f)' : '') + d.data.name)
            .transition().duration(this.duration + 500).style("opacity", "1");

        const nodeUpdate = nodeEnter.merge(node);

        nodeUpdate.transition()
            .duration(this.duration)
            .attr("transform", (d) => `
                rotate(${d.x * 180 / Math.PI - 90})
                translate(${d.y},0)
                rotate(${d.x >= Math.PI ? 180 : 0})
            `);

        nodeUpdate.select('circle.node')
            .attr('r', d => d.data.id == this.horse.id ? 8 : 5)
            .attr('cursor', 'pointer');

        nodeUpdate.select('text')
            .clone(true).lower()
            .attr("stroke", "white");

        const nodeExit = node.exit().transition()
            .duration(this.duration)
            .attr("transform", (d) => `
                rotate(${source.x * 180 / Math.PI - 90})
                translate(${source.y},0)
                rotate(${source.x >= Math.PI ? 180 : 0})
            `)
            .remove();

        nodeExit.select('circle')
            .attr('r', 1e-6);

        nodeExit.select('text')
            .style('fill-opacity', 1e-6);

        this.chartGroupEl.selectAll('path.link').remove();
        const link = this.chartGroupEl.selectAll('path.link')
            .data(tree.links(), (d) => { return d.id; });

        const linkEnter = link.enter().insert('path', "g")
            .attr("class", "link")
            .attr("style", "opacity: 0")
            .attr('d', d3.linkRadial().angle((d: any) => d.x).radius((d: any) => d.y))

        const linkUpdate = linkEnter.merge(link);

        linkUpdate.transition()
            .duration(this.duration + 500)
            .attr("style", `opacity: 1; stroke:${this.horse.mtDNAColor};`);

        // Remove any exiting links
        // link.exit().transition()
        //     .duration(this.duration)
        //     .attr("style", "opacity: 0")
        //     .remove();

        nodes.forEach((d) => {
            d.x0 = d.x;
            d.y0 = d.y;
        });
    }
}