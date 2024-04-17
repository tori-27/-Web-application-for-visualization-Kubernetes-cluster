import * as d3 from 'd3';

export default class GraphBuilder {
    constructor(selector, data) {
        this.selector = selector;
        this.nodes = data.nodes;
        this.links = data.links;
        this.svg = d3.select(selector).append('svg')
            .attr('width', 800)
            .attr('height', 600);
        this.initializeSimulation();
    }

    initializeSimulation() {
        // Створюємо симуляцію з початковими вузлами і зв'язками
        this.simulation = d3.forceSimulation(this.nodes)
            .force('link', d3.forceLink(this.links).id(d => d.id))
            .force('charge', d3.forceManyBody())
            .force('center', d3.forceCenter(400, 300));
    }

    build() {
        // Фільтруємо зв'язки, які мають невідповідні вузли
        this.filterLinks();

        const link = this.svg.append('g')
            .attr('class', 'links')
            .selectAll('line')
            .data(this.links)
            .enter().append('line')
            .attr('stroke-width', 2)
            .attr('stroke', '#ddd');

        const node = this.svg.append('g')
            .attr('class', 'nodes')
            .selectAll('circle')
            .data(this.nodes)
            .enter().append('circle')
            .attr('r', 5)
            .attr('fill', '#007bff')
            .call(d3.drag()
                .on('start', (event, d) => {
                    if (!event.active) this.simulation.alphaTarget(0.3).restart();
                    d.fx = d.x;
                    d.fy = d.y;
                })
                .on('drag', (event, d) => {
                    d.fx = event.x;
                    d.fy = event.y;
                })
                .on('end', (event, d) => {
                    if (!event.active) this.simulation.alphaTarget(0);
                    d.fx = null;
                    d.fy = null;
                }));

        this.simulation.on('tick', () => {
            link
                .attr('x1', d => d.source.x)
                .attr('y1', d => d.source.y)
                .attr('x2', d => d.target.x)
                .attr('y2', d => d.target.y);
            node
                .attr('cx', d => d.x)
                .attr('cy', d => d.y);
        });
    }

    filterLinks() {
        this.links = this.links.filter(link =>
            this.nodes.some(node => node.id === link.source.id || node.id === link.target.id)
        );
        this.simulation.force('link').links(this.links);
    }

    update(newNodes, newLinks) {
        this.nodes = newNodes;
        this.links = newLinks;
        this.filterLinks();

        // Оновлення симуляції d3 з новими даними
        this.simulation.nodes(newNodes);
        this.simulation.force("link").links(this.links);
        this.simulation.alpha(1).restart();
    }
}
