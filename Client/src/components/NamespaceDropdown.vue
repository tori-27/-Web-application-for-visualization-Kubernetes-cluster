<template>
  <div>
    <select v-model="selectedNamespace" @change="changeNamespace">
      <option v-for="namespace in namespaces" :key="namespace" :value="namespace">
        {{ namespace }}
      </option>
    </select>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted } from 'vue';
import { useNamespaceStore } from "../stores/namespaceStore.js";
import signalRService from "../services/signalRService.js";

const store = useNamespaceStore();
const namespaces = computed(() => store.namespaces);
const selectedNamespace = computed({
  get: () => store.selectedNamespace,
  set: (value) => store.changeNamespace(value)
});

onMounted(async () => {
  await store.fetchNamespaces();

  signalRService.connection.on("ReceiveNamespacesUpdate", store.handleNamespaceUpdate);
});

onUnmounted(() => {
  if (signalRService.connection) {
    signalRService.connection.off("ReceiveNamespacesUpdate", store.handleNamespaceUpdate);
  }
});

function changeNamespace(event) {
  store.changeNamespace(event.target.value);
}
</script>



