<!-- src/layouts/MemberLayout.vue -->
<template>
  <div class="member-layout-wrapper">
    <div class="member-body">
      <!-- 側邊欄 -->
      <aside class="sidebar">
        <div class="user-card">
          <el-avatar :size="60" icon="UserFilled" />
          <div class="user-info">
            <div class="user-name">{{ authStore.memberInfo?.account || '會員名稱' }}</div>
            <div class="edit-profile" @click="router.push('/member/settings')">
              <el-icon><Edit /></el-icon> 編輯個人資料
            </div>
          </div>
        </div>
        
        <el-menu :default-active="activeMenu" router class="member-menu">
          <el-menu-item index="/member">
            <el-icon><User /></el-icon>我的帳戶
          </el-menu-item>
          <el-menu-item index="/member/orders">
            <el-icon><Document /></el-icon>購買清單
          </el-menu-item>
          <el-menu-item index="/member/wallet">
            <el-icon><Wallet /></el-icon>我的錢包
          </el-menu-item>
          <el-menu-item index="/member/settings">
            <el-icon><Setting /></el-icon>設定
          </el-menu-item>
        </el-menu>
      </aside>

      <!-- 內容區 -->
      <main class="member-content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { User, Document, Wallet, Setting, Edit, UserFilled } from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const activeMenu = computed(() => {
  // 處理詳情頁面時的選單啟動狀態
  if (route.path.startsWith('/member/orders')) return '/member/orders';
  return route.path;
})
</script>

<style scoped lang="scss">
.member-layout-wrapper {
  padding: 20px 0;
}

.member-body {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  gap: 20px;
  padding: 0 10px;
}

.sidebar {
  width: 180px;
  flex-shrink: 0;
}

.user-card {
  padding: 15px 0;
  display: flex;
  align-items: center;
  gap: 12px;
  border-bottom: 1px solid #efefef;
  margin-bottom: 15px;

  .user-info {
    .user-name {
      font-weight: bold;
      font-size: 14px;
      color: #333;
      margin-bottom: 4px;
    }
    .edit-profile {
      font-size: 12px;
      color: #888;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 3px;
      &:hover {
        color: #ee4d2d;
      }
    }
  }
}

.member-menu {
  border: none;
  background: transparent;

  :deep(.el-menu-item) {
    height: 40px;
    line-height: 40px;
    font-size: 14px;
    padding-left: 10px !important;
    
    &:hover {
      color: #ee4d2d;
      background-color: transparent;
    }
    
    &.is-active {
      color: #ee4d2d;
      background-color: transparent;
      font-weight: bold;
    }

    .el-icon {
      font-size: 18px;
    }
  }
}

.member-content {
  flex: 1;
  min-width: 0; // 避免 flex 項目溢出
}

@media (max-width: 768px) {
  .member-body {
    flex-direction: column;
  }
  .sidebar {
    width: 100%;
  }
}
</style>
