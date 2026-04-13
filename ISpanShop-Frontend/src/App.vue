<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useCartStore } from '@/stores/cart';
import { ShoppingCart, House } from '@element-plus/icons-vue';

const router = useRouter();
const cartStore = useCartStore();

const goToHome = () => router.push('/');
const goToCart = () => router.push('/cart');
</script>

<template>
  <div class="app-container">
    <!-- 全域導航列 -->
    <el-header class="nav-header">
      <div class="header-content container">
        <div class="logo" @click="goToHome">
          <img src="@/assets/logo.svg" alt="Logo" width="30" height="30" />
          <span class="brand-name">ISpanShop</span>
        </div>

        <div class="nav-links">
          <el-button :icon="House" link @click="goToHome">首頁</el-button>
          
          <el-badge :value="cartStore.totalQuantity" :hidden="cartStore.totalQuantity === 0" class="cart-badge">
            <el-button :icon="ShoppingCart" type="primary" circle @click="goToCart" />
          </el-badge>
        </div>
      </div>
    </el-header>

    <!-- 路由內容渲染區 -->
    <el-main class="main-content">
      <router-view />
    </el-main>

    <!-- 頁腳 (選配) -->
    <el-footer class="footer">
      <div class="container">
        © 2024 ISpanShop - 前後端分離練習專案
      </div>
    </el-footer>
  </div>
</template>

<style scoped>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #f5f7fa;
}

.nav-header {
  background-color: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  position: sticky;
  top: 0;
  z-index: 1000;
  height: 64px !important;
  display: flex;
  align-items: center;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.logo {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
}

.brand-name {
  font-size: 20px;
  font-weight: bold;
  color: #409eff;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 20px;
}

.main-content {
  flex: 1;
  padding: 0;
}

.footer {
  text-align: center;
  color: #909399;
  padding: 24px 0;
  background: #fff;
  border-top: 1px solid #e4e7ed;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.cart-badge :deep(.el-badge__content) {
  top: 5px;
  right: 5px;
}
</style>

<style>
/* 全域樣式修正 */
body {
  margin: 0;
  font-family: 'Helvetica Neue', Helvetica, 'PingFang SC', 'Hiragino Sans GB', 'Microsoft YaHei', '微软雅黑', Arial, sans-serif;
}
</style>
