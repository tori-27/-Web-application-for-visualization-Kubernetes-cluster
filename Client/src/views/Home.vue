<template>
  <main class="home-page">
    <NetworkGraph></NetworkGraph>
    <NamespaceDropdown />
  </main>
</template>

<script setup>
import NamespaceDropdown from "../components/NamespaceDropdown.vue";
import NetworkGraph from "../components/KubernetesResourcesGraph.vue";
</script>

<style lang="scss">
.home-page {
  display: flex;
  height: 100vh;
}

.dropdown {
  width: 100%;
  max-width: 300px;
}

.namespace-selector {
  width: 100%;
  padding: 0.5rem 1rem;
  background-color: var(--dark);
  color: var(--grey);
  border: 1px solid var(--dark);
  border-radius: 8px;
  outline: none;
  appearance: none;
  cursor: pointer;

  &:focus {
    border-color: var(--primary);
  }

  option {
    padding: 0.5rem 1rem;
    background: var(--dark);
    color: var(--light);
  }
}

.namespace-selector {
  overflow-y: auto;
  max-height: 200px;
}
</style>
