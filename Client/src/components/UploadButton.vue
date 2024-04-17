<template>
  <div class="file-upload-container">
    <input type="file" id="kubeconfig" class="file-upload-input" @change="uploadFile($event)" />
    <label for="kubeconfig" class="file-upload-button">
      Upload .kubeconfig file
    </label>
  </div>
</template>

<script>
import signalRService from "../services/signalRService.js";
export default {
  name: "UploadButton",
  methods: {
    async uploadFile(event) {
      const file = event.target.files[0];
      if (!file) {
        this.$emit('uploadError', 'No file provided');
        return;
      }

      const maxSize = 512000;
      if(file.size > maxSize){
        this.$emit('uploadError', 'File size exceeds the limit of 512Kb');
        return;
      }
      const reader = new FileReader();
      reader.onload = async (e) => {
        const base64 = e.target.result;
        try {
          await signalRService.connection.invoke("UploadKubeConfig", base64);
          this.$emit('uploadSuccess', 'Client have initialized');
        } catch (error) {
          this.$emit('uploadError', error.message);
        }
      };
      reader.readAsDataURL(file);
    }
  }
}
</script>

<style lang="scss" scoped>
.file-upload-button{
  font-size: 2rem;
  padding: 2rem 3rem;
  color: var(--primary);
  background-color: var(--dark-alt);
  border: none;
  border-radius: 8px;
  transition: background-color 0.3s, filter 0.3s;
  &:hover {
   filter: brightness(90%);
  }
  &:focus {
   outline: 2px solid var(--primary);
   outline-offset: 2px;
  }
}

.file-upload-input {
  width: 0.1px;
  height: 0.1px;
  opacity: 0;
  overflow: hidden;
  position: absolute;
  z-index: -1;
}

.file-upload-container {
  position: relative;
}
</style>