<script setup lang="ts">
import { computed, nextTick, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '@/stores/cart';
import { ElMessageBox, ElMessage } from 'element-plus';
import { Delete, ShoppingCart } from '@element-plus/icons-vue';
import type { TableInstance } from 'element-plus';

const router = useRouter();
const cartStore = useCartStore();
const tableRef = ref<TableInstance>();

// 初始化時，根據 Store 的 selected 狀態設定表格勾選
onMounted(async () => {
  await nextTick();
  if (tableRef.value) {
    cartStore.items.forEach((item) => {
      if (item.selected) {
        tableRef.value!.toggleRowSelection(item, true);
      }
    });
  }
});

/**
 * 處理表格勾選變動
 * 同步更新 Store 中項目的 selected 狀態
 */
const handleSelectionChange = (selectedRows: any[]) => {
  // 先將所有項目的 selected 設為 false
  cartStore.items.forEach((item) => {
    item.selected = false;
  });
  // 將被選中的項目 selected 設為 true
  selectedRows.forEach((row) => {
    const item = cartStore.items.find((i) => i.variantId === row.variantId);
    if (item) item.selected = true;
  });
};

/**
 * 處理刪除商品
 */
const handleDelete = (variantId: number) => {
  ElMessageBox.confirm('確定要從購物車移除此商品嗎？', '確認移除', {
    confirmButtonText: '確定',
    cancelButtonText: '取消',
    type: 'warning',
  })
    .then(() => {
      cartStore.removeFromCart(variantId);
      ElMessage.success('已移除商品');
    })
    .catch(() => {});
};

/**
 * 前往結帳
 */
const goToCheckout = () => {
  if (cartStore.selectedCount === 0) return;
  router.push('/checkout');
};

/**
 * 回到商店
 */
const goBackToStore = () => {
  router.push('/product');
};
</script>

<template>
  <div class="cart-view container">
    <div class="cart-header">
      <h1><el-icon><ShoppingCart /></el-icon> 購物車</h1>
    </div>

    <!-- 購物車清單 -->
    <template v-if="cartStore.items.length > 0">
      <el-table
        ref="tableRef"
        :data="cartStore.items"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <!-- 勾選框 -->
        <el-table-column type="selection" width="55" />

        <!-- 商品圖片 -->
        <el-table-column label="商品圖片" width="120">
          <template #default="{ row }">
            <el-image
              :src="row.imageUrl"
              fit="cover"
              class="product-image"
              :preview-src-list="[row.imageUrl]"
              preview-teleported
            >
              <template #error>
                <div class="image-slot">
                  <el-icon><Picture /></el-icon>
                </div>
              </template>
            </el-image>
          </template>
        </el-table-column>

        <!-- 商品資訊 -->
        <el-table-column label="商品資訊" min-width="200">
          <template #default="{ row }">
            <div class="product-info">
              <div class="product-name">{{ row.productName }}</div>
              <el-tag size="small" type="info">{{ row.variantName }}</el-tag>
            </div>
          </template>
        </el-table-column>

        <!-- 單價 -->
        <el-table-column label="單價" width="120">
          <template #default="{ row }">
            <span class="price">NT$ {{ row.price.toLocaleString() }}</span>
          </template>
        </el-table-column>

        <!-- 數量 -->
        <el-table-column label="數量" width="180">
          <template #default="{ row }">
            <el-input-number
              v-model="row.quantity"
              :min="1"
              :max="row.stock"
              size="small"
              @change="(val: number) => cartStore.updateQuantity(row.variantId, val)"
            />
            <div class="stock-hint" v-if="row.stock <= 5">僅剩 {{ row.stock }} 件</div>
          </template>
        </el-table-column>

        <!-- 小計 -->
        <el-table-column label="小計" width="120">
          <template #default="{ row }">
            <span class="subtotal">NT$ {{ (row.price * row.quantity).toLocaleString() }}</span>
          </template>
        </el-table-column>

        <!-- 操作 -->
        <el-table-column label="操作" width="100">
          <template #default="{ row }">
            <el-button
              type="danger"
              :icon="Delete"
              circle
              plain
              @click="handleDelete(row.variantId)"
            />
          </template>
        </el-table-column>
      </el-table>

      <!-- 底部固定結帳列 -->
      <div class="checkout-bar">
        <div class="bar-left">
          <span>已選取 {{ cartStore.selectedCount }} 項商品</span>
        </div>
        <div class="bar-right">
          <div class="total-section">
            <span class="label">總金額 ({{ cartStore.selectedCount }} 個項目):</span>
            <span class="total-price">NT$ {{ cartStore.totalPrice.toLocaleString() }}</span>
          </div>
          <el-button
            type="primary"
            size="large"
            :disabled="cartStore.selectedCount === 0"
            @click="goToCheckout"
          >
            前往結帳
          </el-button>
        </div>
      </div>
    </template>

    <!-- 空狀態 -->
    <el-empty v-else description="您的購物車是空的" :image-size="200">
      <el-button type="primary" @click="goBackToStore">回到商店選購</el-button>
    </el-empty>
  </div>
</template>

<style scoped lang="scss">
.cart-view {
  padding: 40px 20px 100px;
}

.cart-header {
  margin-bottom: 24px;
  h1 {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 24px;
    margin: 0;
  }
}

.product-image {
  width: 80px;
  height: 80px;
  border-radius: 4px;
  border: 1px solid #eee;
}

.product-info {
  .product-name {
    font-weight: 600;
    margin-bottom: 4px;
    color: #333;
  }
}

.price, .subtotal {
  font-family: 'PingFang TC', sans-serif;
}

.subtotal {
  color: #f56c6c;
  font-weight: 600;
}

.stock-hint {
  font-size: 12px;
  color: #e6a23c;
  margin-top: 4px;
}

.checkout-bar {
  position: sticky;
  bottom: 20px;
  margin-top: 30px;
  background: #fff;
  padding: 20px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 -4px 12px rgba(0, 0, 0, 0.1);
  border-radius: 8px;
  z-index: 10;

  .bar-left {
    color: #606266;
  }

  .bar-right {
    display: flex;
    align-items: center;
    gap: 24px;

    .total-section {
      .label {
        font-size: 14px;
        color: #606266;
        margin-right: 8px;
      }
      .total-price {
        font-size: 24px;
        font-weight: bold;
        color: #f56c6c;
      }
    }
  }
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.image-slot {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  background: #f5f7fa;
  color: #909399;
  font-size: 24px;
}
</style>
