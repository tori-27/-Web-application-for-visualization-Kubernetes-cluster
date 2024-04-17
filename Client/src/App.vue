
<template>
  <div class="app">
    <Sidebar />
    <router-view />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import Sidebar from './components/Sidebar.vue'
import signalRService from "./services/signalRService.js";

onMounted(() => {
  signalRService.startConnection();
});


</script>
<style Lang="scss">
:root {
  --primary: #A4FFF4;
  --grey: #64748b;
  --dark: #1e293b;
  --dark-alt: #334155;
  --light: #2C3E50;
  --sidebar-width: 300px;
}

* {
  margin: 0;
  padding: 1;
  box-sizing: border-box;
  font-family: 'Fira sans', sans-serif;
}

body {
  background: var(--light)
}

button {
  cursor: pointer;
  appearance: none;
  border: none;
  outline: none;
  background: none;
}
.app {
  display: flex;

  main {
    flex: 1 1 0;
    padding: 2rem;

    @media (max-width: 768px){
      padding-left: 6rem;
    }
  }
}
</style>
