<!--<template>-->
<!--  <div>-->
<!--    <h2>Network Graph</h2>-->
<!--    <h3>Nodes:</h3>-->
<!--    <ul>-->
<!--      <li v-for="node in transformedData.nodes" :key="node.id">-->
<!--        ID: {{ node.id }}, Label: {{ node.label }}, Type: {{ node.type }}-->
<!--      </li>-->
<!--    </ul>-->
<!--    <h3>Links:</h3>-->
<!--    <ul>-->
<!--      <li v-for="link in transformedData.links" :key="link.id">-->
<!--        Link ID: {{ link.id }}, Source: {{ link.source }}, Target: {{ link.target }}-->
<!--      </li>-->
<!--    </ul>-->
<!--  </div>-->
<!--</template>-->


<!--<script setup>-->
<!--import { computed, watch } from 'vue';-->
<!--import { useResourcesStore } from '../stores/resourcesStore';-->
<!--import { useNamespaceStore } from '../stores/namespaceStore';-->
<!--import visualizationDataService from "../services/visualizationDataService.js";-->
<!--import GraphBuilder from "../utils/graphBuilder.js";-->

<!--const resourcesStore = useResourcesStore();-->
<!--const namespaceStore = useNamespaceStore();-->

<!--watch(() => namespaceStore.selectedNamespace, (newNamespace) => {-->
<!--  if (newNamespace) {-->
<!--    resourcesStore.fetchResources();-->
<!--    resourcesStore.initializeUpdateListener();-->
<!--  }-->
<!--}, {immediate: true});-->

<!--const transformedData = computed(() => {-->
<!--  const data = visualizationDataService.getTransformedData();-->
<!--  return {-->
<!--    nodes: data.nodes || [],-->
<!--    links: data.links || []-->
<!--  };-->
<!--});-->

<!--</script>-->



<template>
  <div>
    <h2>Network Graph</h2>
    <!-- Контейнер для графу D3 -->
    <div ref="graphContainer" class="graph-container"></div>
  </div>
</template>

<script setup>
import { onMounted, ref, watchEffect, watch } from 'vue';
import { useResourcesStore } from '../stores/resourcesStore';
import { useNamespaceStore } from '../stores/namespaceStore';
import visualizationDataService from "../services/visualizationDataService.js";
import GraphBuilder from "../utils/graphBuilder.js";

// Підключаємося до Pinia сторів
const resourcesStore = useResourcesStore();
const namespaceStore = useNamespaceStore();
const graphContainer = ref(null);

watch(() => namespaceStore.selectedNamespace, (newNamespace) => {
  if (newNamespace) {
    resourcesStore.fetchResources();
    resourcesStore.initializeUpdateListener();
  }
}, {immediate: true});

// Створюємо інстанс GraphBuilder тільки після того, як дані будуть завантажені
let graphBuilder = null;
watchEffect(() => {
  // Отримуємо трансформовані дані
  const data = visualizationDataService.getTransformedData();

  console.log(graphContainer.value);
  if (data.nodes.length) {
    console.log("inside in graph building")
      // Інакше, створюємо новий граф
      graphBuilder = new GraphBuilder(graphContainer.value, data);
      graphBuilder.build();
  }
});

// Отримуємо та відображаємо ресурси при монтажі компонента
onMounted(() => {
  if (namespaceStore.selectedNamespace) {
    resourcesStore.fetchResources();
    resourcesStore.initializeUpdateListener();
  }
});
</script>

<style>
.graph-container {
  width: 800px;
  height: 600px;
  margin: auto;
}
</style>





