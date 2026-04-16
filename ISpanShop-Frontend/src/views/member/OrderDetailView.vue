<template>
  <div class="order-detail-container" v-loading="loading">
    <el-card v-if="order" shadow="never">
      <template #header>
        <div class="card-header">
          <el-button @click="router.back()" circle icon="ArrowLeft" />
          <span class="title">訂單詳情</span>
          <span class="order-status" :class="getStatusClass(order.status)">
            {{ order.statusName }}
          </span>
        </div>
      </template>

      <div class="info-section">
        <div class="info-item">
          <span class="label">訂單編號：</span>
          <span class="value">{{ order.orderNumber }}</span>
        </div>
        <div class="info-item">
          <span class="label">下單時間：</span>
          <span class="value">{{ formatDate(order.createdAt) }}</span>
        </div>
        <div v-if="order.paymentDate" class="info-item">
          <span class="label">付款時間：</span>
          <span class="value">{{ formatDate(order.paymentDate) }}</span>
        </div>
      </div>

      <el-divider />

      <div class="address-section">
        <h3 class="section-title">收件資訊</h3>
        <p><strong>收件人：</strong>{{ order.recipientName }}</p>
        <p><strong>電話：</strong>{{ order.recipientPhone }}</p>
        <p><strong>地址：</strong>{{ order.recipientAddress }}</p>
        <p v-if="order.note"><strong>備註：</strong>{{ order.note }}</p>
      </div>

      <el-divider />

      <div class="items-section">
        <h3 class="section-title">商品清單</h3>
        <div v-for="item in order.items" :key="item.id" class="product-item">
          <el-image :src="item.coverImage || '/placeholder.png'" class="product-image" fit="cover" />
          <div class="product-info">
            <div class="name">{{ item.productName }}</div>
            <div class="variant">{{ item.variantName }}</div>
            <div class="price-qty">
              <span class="price">${{ formatPrice(item.price) }}</span>
              <span class="qty">x {{ item.quantity }}</span>
            </div>
          </div>
          <div class="subtotal">
            ${{ formatPrice(item.price * item.quantity) }}
          </div>
        </div>
      </div>

      <el-divider />

      <div class="summary-section">
        <div class="summary-item">
          <span>商品總計</span>
          <span>${{ formatPrice(order.totalAmount) }}</span>
        </div>
        <div class="summary-item">
          <span>運費</span>
          <span>${{ formatPrice(order.shippingFee || 0) }}</span>
        </div>
        <div class="summary-item total">
          <span>實付金額</span>
          <span class="final-amount">${{ formatPrice(order.finalAmount) }}</span>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ArrowLeft } from '@element-plus/icons-vue';
import { getOrderDetailApi } from '@/api/order';
import type { OrderDetail } from '@/types/order';
import { ElMessage } from 'element-plus';

const route = useRoute();
const router = useRouter();
const loading = ref(false);
const order = ref<OrderDetail | null>(null);

const fetchOrderDetail = async () => {
  const id = Number(route.params.id);
  if (isNaN(id)) return;

  loading.value = true;
  try {
    const res = await getOrderDetailApi(id);
    order.value = res.data;
  } catch (error) {
    console.error('獲取訂單詳情失敗', error);
    ElMessage.error('獲取訂單詳情失敗');
  } finally {
    loading.value = false;
  }
};

const formatPrice = (price: number) => {
  return new Intl.NumberFormat('zh-TW').format(price);
};

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-';
  const date = new Date(dateStr);
  return date.toLocaleString('zh-TW');
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
  fetchOrderDetail();
});
</script>

<style scoped lang="scss">
.order-detail-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 15px;
  
  .title {
    font-size: 1.2rem;
    font-weight: bold;
    flex-grow: 1;
  }

  .order-status {
    font-weight: bold;
    &.status-pending { color: #e6a23c; }
    &.status-processing { color: #409eff; }
    &.status-shipped { color: #67c23a; }
    &.status-completed { color: #909399; }
    &.status-cancelled { color: #f56c6c; }
  }
}

.section-title {
  font-size: 1.1rem;
  margin-bottom: 15px;
  border-left: 4px solid #409eff;
  padding-left: 10px;
}

.info-section {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  .info-item {
    font-size: 0.95rem;
    .label { color: #909399; }
  }
}

.address-section {
  p {
    margin: 5px 0;
    line-height: 1.6;
  }
}

.product-item {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 10px 0;
  border-bottom: 1px solid #f0f0f0;

  &:last-child { border-bottom: none; }

  .product-image {
    width: 60px;
    height: 60px;
    border-radius: 4px;
  }

  .product-info {
    flex-grow: 1;
    .name { font-weight: 500; }
    .variant { font-size: 0.85rem; color: #909399; }
    .price-qty {
      margin-top: 5px;
      .price { color: #f56c6c; margin-right: 10px; }
      .qty { color: #909399; }
    }
  }

  .subtotal {
    font-weight: bold;
  }
}

.summary-section {
  .summary-item {
    display: flex;
    justify-content: flex-end;
    gap: 20px;
    margin-bottom: 8px;
    
    &.total {
      margin-top: 15px;
      font-size: 1.1rem;
      font-weight: bold;
    }

    .final-amount {
      color: #f56c6c;
      font-size: 1.4rem;
    }
  }
}
</style>
