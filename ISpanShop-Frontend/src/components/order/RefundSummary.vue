<!-- src/components/order/RefundSummary.vue -->
<script setup lang="ts">
import { computed, watch } from 'vue'
import { InfoFilled } from '@element-plus/icons-vue'

interface OrderItem {
  id: number
  price: number
  quantity: number
}

interface OrderData {
  totalAmount: number
  finalAmount: number
  levelDiscount?: number
  discountAmount?: number
  pointDiscount?: number
  items: OrderItem[]
}

interface Props {
  order: OrderData | null
  selectedItemIds: number[]
  returnQuantities: Record<number, number>
}

const props = defineProps<Props>()

// 除錯訊息：當 order 改變時打印
watch(() => props.order, (newOrder) => {
  if (newOrder) {
    console.log('RefundSummary: Order Data received', {
      totalAmount: newOrder.totalAmount,
      finalAmount: newOrder.finalAmount,
      levelDiscount: newOrder.levelDiscount,
      discountAmount: newOrder.discountAmount
    })
  }
}, { immediate: true })

// 1. 退貨商品原價小計
const itemsSubtotal = computed(() => {
  if (!props.order) return 0
  return props.selectedItemIds.reduce((sum, id) => {
    const item = props.order?.items.find(i => i.id === id)
    return sum + (item ? item.price * (props.returnQuantities[id] || 0) : 0)
  }, 0)
})

// 2. 判斷是否為「全額退貨」
const isFullReturn = computed(() => {
  if (!props.order) return false
  const isAllSelected = props.order.items.length === props.selectedItemIds.length
  const isAllQtyMatch = props.selectedItemIds.every(id => {
    const item = props.order?.items.find(i => i.id === id)
    return item && props.returnQuantities[id] === item.quantity
  })
  return isAllSelected && isAllQtyMatch
})

// 3. 計算各項分攤比例 (部分退款時使用)
const ratio = computed(() => {
  if (!props.order || props.order.totalAmount === 0) return 0
  return itemsSubtotal.value / props.order.totalAmount
})

// 4. 各項折抵分攤計算
const levelDiscountShare = computed(() => 
  isFullReturn.value ? (props.order?.levelDiscount || 0) : Math.round((props.order?.levelDiscount || 0) * ratio.value)
)

const couponDiscountShare = computed(() => 
  isFullReturn.value ? (props.order?.discountAmount || 0) : Math.round((props.order?.discountAmount || 0) * ratio.value)
)

const pointDiscountShare = computed(() => 
  isFullReturn.value ? (props.order?.pointDiscount || 0) : Math.round((props.order?.pointDiscount || 0) * ratio.value)
)

// 5. 最終預計退款總額
const finalRefundAmount = computed(() => {
  if (isFullReturn.value) return props.order?.finalAmount || 0
  const amount = itemsSubtotal.value - levelDiscountShare.value - couponDiscountShare.value - pointDiscountShare.value
  return amount > 0 ? amount : 0
})

const formatPrice = (val: number) => val?.toLocaleString('zh-TW') || '0'

// 向外暴露計算結果，讓父組件在 Footer 也能顯示
defineExpose({
  finalRefundAmount
})
</script>

<template>
  <div v-if="order" class="refund-summary-box">
    <div class="summary-section">
      <div class="summary-row">
        <span>退貨商品小計</span>
        <span>NT$ {{ formatPrice(itemsSubtotal) }}</span>
      </div>

      <!-- 會員等級折抵分攤 -->
      <!-- 修正：確保只要有數值就顯示，或者至少在 debug 時能看到 -->
      <div v-if="levelDiscountShare !== 0" class="summary-row">
        <span class="label-with-hint">
          會員等級折抵分攤
          <el-tooltip content="按商品金額比例分攤當初享有的會員折扣" placement="top">
            <el-icon class="info-icon"><InfoFilled /></el-icon>
          </el-tooltip>
        </span>
        <span class="discount">- NT$ {{ formatPrice(Math.abs(levelDiscountShare)) }}</span>
      </div>

      <!-- 優惠券折抵分攤 -->
      <div v-if="couponDiscountShare !== 0" class="summary-row">
        <span>優惠券折抵分攤</span>
        <span class="discount">- NT$ {{ formatPrice(Math.abs(couponDiscountShare)) }}</span>
      </div>

      <!-- 點數折抵分攤 -->
      <div v-if="pointDiscountShare !== 0" class="summary-row">
        <span>點數折抵分攤</span>
        <span class="discount">- NT$ {{ formatPrice(Math.abs(pointDiscountShare)) }}</span>
      </div>

      <div class="summary-row final">
        <span>預計退款金額</span>
        <span class="price">NT$ {{ formatPrice(finalRefundAmount) }}</span>
      </div>

      <div class="refund-hint">
        <small v-if="isFullReturn">* 全額退貨將退還訂單最終實付金額 (含運費)。</small>
        <small v-else>* 部分退貨將依比例扣除折抵金額，且不退還運費。</small>
      </div>
    </div>
  </div>
</template>

<style scoped>
.summary-section {
  background: #fafafa;
  padding: 20px;
  border-radius: 4px;
  border: 1px dashed #e4e4e4;
}
.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  font-size: 14px;
  color: #666;
}
.discount { color: #ee4d2d; }
.final {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #e4e4e4;
  font-size: 16px;
  font-weight: bold;
  color: #333;
}
.price { color: #ee4d2d; font-size: 22px; }
.label-with-hint { display: flex; align-items: center; gap: 4px; }
.info-icon { font-size: 14px; color: #909399; cursor: pointer; }
.refund-hint {
  margin-top: 12px;
  text-align: right;
  color: #999;
  font-size: 12px;
}
</style>
