<template>
  <div class="settings-container">
    <upload-button
        upload-url="https://localhost:7254/api/KubernetesCluster/upload"
    ></upload-button>
  </div>
</template>

<script>
import UploadButton from "../components/UploadButton.vue";
export default {
  components:{
  UploadButton
  },
}
</script>

<style lang="scss" scoped>
.settings-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10rem;
  margin-top: 10rem;
  margin-left: 5rem;
}
</style>
