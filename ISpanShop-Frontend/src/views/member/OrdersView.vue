<template>
  <div class="order-history-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span class="title">我的訂單</span>
        </div>
      </template>

      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="全部" name="all"></el-tab-pane>
        <el-tab-pane label="待付款" name="0"></el-tab-pane>
        <el-tab-pane label="待出貨" name="1"></el-tab-pane>
        <el-tab-pane label="運送中" name="2"></el-tab-pane>
        <el-tab-pane label="已完成" name="3"></el-tab-pane>
        <el-tab-pane label="已取消" name="4"></el-tab-pane>
      </el-tabs>

      <div v-loading="loading" class="order-list">
        <el-empty v-if="filteredOrders.length === 0" description="暫無訂單資料" />
        
        <div v-for="order in filteredOrders" :key="order.id" class="order-item">
          <div class="order-item-header">
            <span class="store-name">
              <el-icon><Shop /></el-icon>
              {{ order.storeName }}
            </span>
            <span class="order-status" :class="getStatusClass(order.status)">
              {{ order.statusName }}
            </span>
          </div>

          <div class="order-item-content" @click="goToDetail(order.id)">
            <div class="product-info">
              <el-image 
                :src="order.firstProductImage || '/placeholder.png'" 
                class="product-image"
                fit="cover"
              />
              <div class="product-detail">
                <div class="product-name">{{ order.firstProductName }}</div>
                <div class="item-count">共 {{ order.totalItemCount }} 件商品</div>
              </div>
            </div>
            <div class="order-price">
              <span class="label">訂單金額：</span>
              <span class="amount">${{ formatPrice(order.finalAmount) }}</span>
            </div>
          </div>

          <div class="order-item-footer">
            <span class="order-time">下單時間：{{ formatDate(order.createdAt) }}</span>
            <div class="actions">
              <el-button size="small" @click="goToDetail(order.id)">查看詳情</el-button>
            </div>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { Shop } from '@element-plus/icons-vue';
import { getMyOrdersApi } from '@/api/order';
import type { OrderListItem } from '@/types/order';
import { ElMessage } from 'element-plus';

const router = useRouter();
const loading = ref(false);
const orders = ref<OrderListItem[]>([]);
const activeTab = ref('all');

const fetchOrders = async () => {
  loading.value = true;
  try {
    const res = await getMyOrdersApi();
    orders.value = res.data;
  } catch (error) {
    console.error('獲取訂單失敗', error);
    ElMessage.error('獲取訂單失敗，請稍後再試');
  } finally {
    loading.value = false;
  }
};

const filteredOrders = computed(() => {
  if (activeTab.value === 'all') {
    return orders.value;
  }
  return orders.value.filter(o => o.status.toString() === activeTab.value);
});

const handleTabChange = (name: string) => {
  // 可以在這裡實作分頁或重新抓取，目前僅過濾本地資料
};

const goToDetail = (id: number) => {
  router.push(`/member/orders/${id}`);
};

const formatPrice = (price: number) => {
  return new Intl.NumberFormat('zh-TW').format(price);
};

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-';
  const date = new Date(dateStr);
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const getStatusClass = (status: number) => {
  switch (status) {
    case 0: return 'status-pending';
    case 1: return 'status-processing';
    case 2: return 'status-shipped';
    case 3: return 'status-completed';
    case 4: return 'status-cancelled';
    default: return '';
  }
};

onMounted(() => {
  fetchOrders();
});
</script>

<style scoped lang="scss">
.order-history-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 20px;
}

.card-header {
  .title {
    font-size: 1.2rem;
    font-weight: bold;
  }
}

.order-list {
  margin-top: 20px;
}

.order-item {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  margin-bottom: 20px;
  background-color: #fff;

  &:hover {
    box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
  }

  .order-item-header {
    padding: 10px 20px;
    border-bottom: 1px solid #ebeef5;
    display: flex;
    justify-content: space-between;
    align-items: center;
    background-color: #fafafa;

    .store-name {
      font-weight: bold;
      display: flex;
      align-items: center;
      gap: 5px;
    }

    .order-status {
      font-size: 0.9rem;
      &.status-pending { color: #e6a23c; }
      &.status-processing { color: #409eff; }
      &.status-shipped { color: #67c23a; }
      &.status-completed { color: #909399; }
      &.status-cancelled { color: #f56c6c; }
    }
  }

  .order-item-content {
    padding: 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    cursor: pointer;

    .product-info {
      display: flex;
      gap: 15px;

      .product-image {
        width: 80px;
        height: 80px;
        border-radius: 4px;
        flex-shrink: 0;
      }

      .product-detail {
        display: flex;
        flex-direction: column;
        justify-content: center;

        .product-name {
          font-weight: 500;
          margin-bottom: 5px;
          display: -webkit-box;
          -webkit-line-clamp: 2;
          -webkit-box-orient: vertical;
          overflow: hidden;
        }

        .item-count {
          color: #909399;
          font-size: 0.85rem;
        }
      }
    }

    .order-price {
      text-align: right;
      .amount {
        font-size: 1.2rem;
        font-weight: bold;
        color: #f56c6c;
      }
    }
  }

  .order-item-footer {
    padding: 10px 20px;
    border-top: 1px dotted #ebeef5;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 0.85rem;
    color: #909399;
  }
}
</style>
