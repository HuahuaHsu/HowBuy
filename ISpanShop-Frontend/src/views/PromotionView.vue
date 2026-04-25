<template>
  <div class="promotion-page">
    <div class="container">
      <el-breadcrumb separator="/" class="breadcrumb">
        <el-breadcrumb-item :to="{ path: '/' }">首頁</el-breadcrumb-item>
        <el-breadcrumb-item>活動專區</el-breadcrumb-item>
      </el-breadcrumb>

      <div class="promo-header">
        <h1 class="promo-title">活動專區 (ID: {{ promotionId }})</h1>
        <p class="promo-hint">更多精彩優惠即將呈現，敬請期待！</p>
      </div>

      <div class="promo-content">
        <el-empty description="此活動暫無專屬商品或內容" :image-size="200" />
      </div>

      <div class="actions">
        <el-button @click="router.push('/')">返回首頁</el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const promotionId = computed(() => route.params.id)
</script>

<style scoped>
.promotion-page {
  padding: 20px 0;
  min-height: 80vh;
  background: #f5f5f5;
}
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}
.breadcrumb {
  margin-bottom: 20px;
}
.promo-header {
  background: white;
  padding: 40px;
  border-radius: 8px;
  text-align: center;
  margin-bottom: 20px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}
.promo-title {
  font-size: 28px;
  color: #333;
  margin: 0 0 10px;
}
.promo-hint {
  color: #999;
  font-size: 16px;
}
.promo-content {
  background: white;
  padding: 60px;
  border-radius: 8px;
  margin-bottom: 20px;
}
.actions {
  text-align: center;
}
</style>
