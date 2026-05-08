<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { getMyCoupons } from '@/api/coupon';
import { ElMessage } from 'element-plus';
import { ArrowLeft, Ticket } from '@element-plus/icons-vue';

const router = useRouter();
const coupons = ref<any[]>([]);
const loading = ref(true);

async function loadCoupons() {
  loading.value = true;
  try {
    const res = await getMyCoupons();
    // /api/coupon/mine 回傳的是直接的陣列，axios 回傳在 res.data
    coupons.value = res.data;
  } catch (err) {
    ElMessage.error('載入優惠券失敗');
  } finally {
    loading.value = false;
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleDateString('zh-TW');
}

function getStatusText(status: number, endTime: string) {
  if (new Date(endTime) < new Date()) return '已過期';
  switch (status) {
    case 0: return '未使用';
    case 1: return '已使用';
    case 3: return '鎖定中';
    default: return '未知';
  }
}

function getStatusType(status: number, endTime: string) {
  if (new Date(endTime) < new Date()) return 'info';
  switch (status) {
    case 0: return 'success';
    case 1: return 'info';
    case 3: return 'warning';
    default: return 'info';
  }
}

onMounted(() => {
  loadCoupons();
});
</script>

<template>
  <div class="page-container">
    <div class="header">
      <el-button @click="router.back()" circle :icon="ArrowLeft" />
      <h2><el-icon><Ticket /></el-icon> 我的優惠券</h2>
    </div>

    <div class="coupon-list-container" v-loading="loading">
      <div v-if="coupons.length === 0" class="empty">
        <el-empty description="目前沒有持有的優惠券" />
      </div>
      <div v-else class="grid">
        <div v-for="c in coupons" :key="c.couponId" class="coupon-item-card" :class="{ used: c.usageStatus !== 0 }">
          <div class="coupon-main">
            <div class="coupon-left-panel">
              <div class="discount-value">
                <template v-if="c.couponType === 1">
                  <span class="symbol">$</span>
                  <span class="val">{{ Math.round(c.discountValue) }}</span>
                </template>
                <template v-else>
                  <span class="val">{{ (100 - c.discountValue) / 10 }}</span>
                  <span class="symbol">折</span>
                </template>
              </div>
              <div class="coupon-type-tag">{{ c.couponType === 1 ? '現金折抵' : '折扣券' }}</div>
            </div>
            <div class="coupon-right-panel">
              <div class="coupon-title">{{ c.title }}</div>
              <div class="coupon-code">代碼：{{ c.couponCode }}</div>
              <div class="coupon-expiry">有效期至 {{ formatDate(c.endTime) }}</div>
              <div class="coupon-status">
                <el-tag :type="getStatusType(c.usageStatus, c.endTime)" size="small">
                  {{ getStatusText(c.usageStatus, c.endTime) }}
                </el-tag>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-container {
  min-height: 100vh;
  background: #f8fafc;
  padding: 30px 20px;
}
.header {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-bottom: 30px;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
}
.header h2 {
  margin: 0;
  font-size: 24px;
  color: #1e293b;
  display: flex;
  align-items: center;
  gap: 8px;
}
.coupon-list-container {
  max-width: 1200px;
  margin: 0 auto;
}
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 20px;
}
.coupon-item-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  border: 1px solid #e2e8f0;
  transition: transform 0.2s;
}
.coupon-item-card:hover {
  transform: translateY(-2px);
}
.coupon-item-card.used {
  opacity: 0.7;
  filter: grayscale(0.5);
}
.coupon-main {
  display: flex;
  height: 120px;
}
.coupon-left-panel {
  width: 120px;
  background: linear-gradient(135deg, #ee4d2d 0%, #ff7337 100%);
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 10px;
}
.used .coupon-left-panel {
  background: #94a3b8;
}
.discount-value {
  margin-bottom: 4px;
}
.discount-value .val {
  font-size: 32px;
  font-weight: bold;
}
.discount-value .symbol {
  font-size: 16px;
  margin: 0 2px;
}
.coupon-type-tag {
  font-size: 12px;
  background: rgba(255, 255, 255, 0.2);
  padding: 2px 8px;
  border-radius: 4px;
}
.coupon-right-panel {
  flex: 1;
  padding: 15px;
  display: flex;
  flex-direction: column;
  position: relative;
}
.coupon-title {
  font-weight: bold;
  font-size: 16px;
  color: #1e293b;
  margin-bottom: 8px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.coupon-code {
  font-size: 13px;
  color: #64748b;
  margin-bottom: 4px;
}
.coupon-expiry {
  font-size: 12px;
  color: #94a3b8;
}
.coupon-status {
  position: absolute;
  bottom: 15px;
  right: 15px;
}

@media (max-width: 480px) {
  .grid {
    grid-template-columns: 1fr;
  }
}
</style>
