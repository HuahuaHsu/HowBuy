<template>
  <div style="padding: 50px; text-align: center;">
    <AppHeader />
    <router-view />
  </div>
</template>

<script setup lang="ts">
import AppHeader from './components/AppHeader.vue'
</script>