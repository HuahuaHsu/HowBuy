<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElLoading } from 'element-plus'
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/auth'
import { useAddressStore } from '@/stores/address'
import { checkoutApi, type CheckoutRequest } from '@/api/checkout'
import { getMemberProfile } from '@/api/member'
import AddressCard from '@/components/member/AddressCard.vue'
import AddressFormDialog from '@/components/member/AddressFormDialog.vue'
import { Plus, ArrowRight, Location, Phone, User } from '@element-plus/icons-vue'

const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()
const addressStore = useAddressStore()

// 地址相關
const selectedAddressId = ref<number | null>(null)
const addressDialogVisible = ref(false)

// 表單資料 (手動填寫用)
const recipient = ref({
  name: '',
  phone: '',
  city: '',
  region: '',
  street: '',
  address: ''
})

const cities = [
  '台北市', '新北市', '桃園市', '台中市', '台南市', '高雄市',
  '基隆市', '新竹市', '嘉義市', '新竹縣', '苗栗縣', '彰化縣',
  '南投縣', '雲林縣', '嘉義縣', '屏東縣', '宜蘭縣', '花蓮縣',
  '台東縣', '澎湖縣', '金門縣', '連江縣'
]

// 當地址選中時，同步到表單
const handleSelectAddress = (addr: any) => {
  selectedAddressId.value = addr.id
  recipient.value.name = addr.recipientName
  recipient.value.phone = addr.recipientPhone
  recipient.value.city = addr.city
  recipient.value.region = addr.region
  recipient.value.street = addr.street
  recipient.value.address = `${addr.city}${addr.region}${addr.street}`
}

const handleAddAddressSuccess = async (form: any) => {
  const success = await addressStore.addAddress(form)
  if (success) {
    addressDialogVisible.value = false
    // 新增成功後，Store 會自動 fetchAddresses，我們選取最新的一筆
    if (addressStore.addresses.length > 0) {
      handleSelectAddress(addressStore.addresses[0])
    }
  }
}

const paymentMethod = ref('ECPay')
const availableCoupons = ref<any[]>([])
const selectedCouponId = ref<number | null>(null)
const usePoints = ref(false)
const walletBalance = ref(0)
const showCouponModal = ref(false)

const subtotal = computed(() => cartStore.totalPrice)
const shippingFee = ref(60)
const selectedCoupon = computed(() => availableCoupons.value.find(c => c.id === selectedCouponId.value))

const couponDiscount = computed(() => {
  if (!selectedCoupon.value) return 0
  const c = selectedCoupon.value
  if (c.couponType === 1) return c.discountValue
  if (c.couponType === 2) {
    let disc = Math.round(subtotal.value * (c.discountValue / 100), 0)
    if (c.maximumDiscount) disc = Math.min(disc, c.maximumDiscount)
    return disc
  }
  return 0
})

const pointDiscount = computed(() => {
  if (!usePoints.value) return 0
  const remaining = subtotal.value - couponDiscount.value
  return Math.min(walletBalance.value, remaining)
})

const finalAmount = computed(() => subtotal.value + shippingFee.value - couponDiscount.value - pointDiscount.value)

onMounted(async () => {
  if (cartStore.items.length === 0) {
    router.push('/cart')
    return
  }

  try {
    const memberId = authStore.memberInfo?.memberId || 0
    const [couponsRes, walletRes, profileRes] = await Promise.all([
      checkoutApi.getAvailableCoupons(cartStore.items[0].storeId, subtotal.value, cartStore.items.map(i => i.productId)),
      checkoutApi.getWalletBalance(),
      memberId ? getMemberProfile(memberId) : Promise.resolve({ data: null })
    ])

    availableCoupons.value = couponsRes.data
    walletBalance.value = walletRes.data.pointBalance ?? walletRes.data.balance ?? 0

    // 載入地址簿
    await addressStore.fetchAddresses()
    if (addressStore.defaultAddress) {
      handleSelectAddress(addressStore.defaultAddress)
    } else if (profileRes && profileRes.data) {
      recipient.value.name = profileRes.data.fullName || ''
      recipient.value.phone = profileRes.data.phoneNumber || ''
      recipient.value.city = profileRes.data.city || ''
      recipient.value.region = profileRes.data.region || ''
      recipient.value.street = profileRes.data.address || ''
      recipient.value.address = `${recipient.value.city}${recipient.value.region}${recipient.value.street}`
    }

    authStore.updatePoints(walletBalance.value)

    if (availableCoupons.value.length > 0) {
      let bestCouponId = null
      let maxDiscount = -1
      availableCoupons.value.forEach(c => {
        let disc = c.couponType === 1 ? c.discountValue : Math.round(subtotal.value * (c.discountValue / 100), 0)
        if (c.couponType === 2 && c.maximumDiscount) disc = Math.min(disc, c.maximumDiscount)
        disc = Math.min(disc, subtotal.value)
        if (disc > maxDiscount) { maxDiscount = disc; bestCouponId = c.id; }
      })
      if (bestCouponId !== null) selectedCouponId.value = bestCouponId
    }
  } catch (err) {
    console.error('Checkout init failed', err)
  }
})

function selectCoupon(id: number | null) {
  selectedCouponId.value = id
  showCouponModal.value = false
}

async function handleSubmit() {
  if (!selectedAddressId.value) {
    recipient.value.address = `${recipient.value.city}${recipient.value.region}${recipient.value.street}`
  }

  if (!recipient.value.name || !recipient.value.phone || !recipient.value.address) {
    ElMessage.warning('請填寫完整的收件資訊')
    return
  }

  const loading = ElLoading.service({ text: '正在建立訂單...' })
  try {
    const payload: CheckoutRequest = {
      userId: authStore.memberInfo?.memberId || 0,
      storeId: cartStore.items[0].storeId,
      usePoints: usePoints.value,
      couponId: selectedCouponId.value,
      items: cartStore.items.map(i => ({
        productId: i.productId,
        variantId: i.variantId || 0,
        unitPrice: i.price,
        quantity: i.quantity,
        productName: i.name,
        variantName: i.variantName || '預設規格'
      })),
      recipientName: recipient.value.name,
      recipientPhone: recipient.value.phone,
      recipientAddress: recipient.value.address,
      paymentMethod: paymentMethod.value
    }

    const res = await checkoutApi.createOrder(payload)
    loading.close()
    if (res.data.success) {
      ElMessage.success('訂單已建立')
      cartStore.clearCart()
      const backendBase = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7125'
      window.location.href = `${backendBase.replace(/\/$/, '')}${res.data.paymentUrl}`
    }
  } catch (err: any) {
    loading.close()
    ElMessage.error(err.response?.data?.message || '結帳失敗')
  }
}

function formatPrice(val: number) { return val.toLocaleString('zh-TW') }
</script>

<template>
  <div class="checkout-page">
    <div class="checkout-container">
      <h1 class="page-title">結帳</h1>

      <!-- 收件資訊 -->
      <el-card class="section-card">
        <template #header>
          <div class="card-header">
            <span>📍 收件資訊</span>
            <el-button type="primary" link :icon="Plus" @click="addressDialogVisible = true">
              新增收件地址
            </el-button>
          </div>
        </template>

        <!-- 已有地址清單 -->
        <div v-if="addressStore.addresses.length > 0" class="address-selection">
          <p class="selection-hint">請選擇收件地址：</p>
          <el-scrollbar max-height="320px">
            <el-row :gutter="10">
              <el-col v-for="addr in addressStore.addresses" :key="addr.id" :span="24">
                <AddressCard
                  :address="addr"
                  selectable
                  :selected="selectedAddressId === addr.id"
                  @select="handleSelectAddress"
                />
              </el-col>
              </el-row>
          </el-scrollbar>
        </div>

        <!-- 手動填寫表單 (無地址時顯示) -->
        <div v-else class="manual-form">
          <el-form label-width="100px" label-position="top">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="收件人姓名">
                  <el-input v-model="recipient.name" placeholder="請輸入姓名" :prefix-icon="User" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="手機號碼">
                  <el-input v-model="recipient.phone" placeholder="請輸入電話" :prefix-icon="Phone" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="縣市">
                  <el-select v-model="recipient.city" placeholder="選擇縣市" class="w-full">
                    <el-option v-for="c in cities" :key="c" :label="c" :value="c" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="行政區">
                  <el-input v-model="recipient.region" placeholder="如：大安區" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="詳細地址">
              <el-input v-model="recipient.street" type="textarea" :rows="2" placeholder="街道名稱、門牌號碼" />
            </el-form-item>
          </el-form>
        </div>
      </el-card>

      <!-- 支付方式 -->
      <el-card class="section-card">
        <template #header><div class="card-header">💳 支付方式</div></template>
        <el-radio-group v-model="paymentMethod">
          <el-radio label="ECPay" border size="large">綠界支付</el-radio>
          <el-radio label="NewebPay" border size="large">藍新支付</el-radio>
        </el-radio-group>
      </el-card>

      <!-- 訂單商品 -->
      <el-card class="section-card">
        <template #header><div class="card-header">🛒 訂單商品</div></template>
        <div v-for="item in cartStore.items" :key="item.productId" class="item-row">
          <el-image :src="item.image" class="item-img" fit="cover" />
          <div class="item-info">
            <div class="item-name">{{ item.name }}</div>
            <div class="item-spec" v-if="item.variantName">{{ item.variantName }}</div>
            <div class="item-price">NT$ {{ formatPrice(item.price) }} x {{ item.quantity }}</div>
          </div>
          <div class="item-total">NT$ {{ formatPrice(item.price * item.quantity) }}</div>
        </div>
      </el-card>

      <!-- 優惠折抵 -->
      <el-card class="section-card">
        <template #header><div class="card-header">🧧 優惠與折抵</div></template>
        <div class="discount-row" @click="showCouponModal = true">
          <div class="label">優惠券</div>
          <div class="value clickable">
            <span v-if="selectedCoupon" class="coupon-tag">{{ selectedCoupon.title }}</span>
            <span v-else>選擇優惠券</span>
            <el-icon><ArrowRight /></el-icon>
          </div>
        </div>
        <div class="discount-row">
          <div class="label">蝦幣折抵 <small>可用 {{ walletBalance }} 點</small></div>
          <div class="value"><el-switch v-model="usePoints" :disabled="walletBalance <= 0" /></div>
        </div>
      </el-card>

      <!-- 總計 -->
      <div class="summary-section">
        <div class="summary-row"><span>商品小計</span><span>NT$ {{ formatPrice(subtotal) }}</span></div>
        <div class="summary-row"><span>運費</span><span>NT$ {{ formatPrice(shippingFee) }}</span></div>
        <div v-if="couponDiscount > 0" class="summary-row discount"><span>優惠券折抵</span><span>- NT$ {{ formatPrice(couponDiscount) }}</span></div>
        <div v-if="pointDiscount > 0" class="summary-row discount"><span>蝦幣折抵</span><span>- NT$ {{ formatPrice(pointDiscount) }}</span></div>
        <div class="summary-row final"><span>訂單總計</span><span class="price">NT$ {{ formatPrice(finalAmount) }}</span></div>
        <el-button type="primary" size="large" class="submit-btn" @click="handleSubmit">確認下單</el-button>
      </div>
    </div>


    <!-- 新增地址彈窗 -->
    <AddressFormDialog v-model="addressDialogVisible" @submit="handleAddAddressSuccess" />

    <!-- 優惠券彈窗 -->
    <el-dialog v-model="showCouponModal" title="選擇優惠券" width="450px">
      <div v-if="availableCoupons.length === 0" class="empty-coupons">目前沒有可用的優惠券</div>
      <div v-else class="coupon-list">
        <div v-for="c in availableCoupons" :key="c.id" class="coupon-item" :class="{ selected: selectedCouponId === c.id }" @click="selectCoupon(c.id)">
          <div class="coupon-title">{{ c.title }}</div>
          <div class="coupon-desc">{{ c.couponType === 1 ? `現折 $${c.discountValue}` : `打 ${c.discountValue} 折` }}<span v-if="c.minimumSpend > 0">，滿 ${{ c.minimumSpend }} 可用</span></div>
        </div>
        <div class="coupon-item none" @click="selectCoupon(null)">不使用優惠券</div>
      </div>
    </el-dialog>
  </div>
</template>

<style scoped>
.checkout-page { background: #f8fafc; min-height: 100vh; padding: 40px 20px; }
.checkout-container { max-width: 850px; margin: 0 auto; }
.page-title { margin-bottom: 24px; font-size: 26px; font-weight: bold; color: #1e293b; }
.section-card { margin-bottom: 20px; border-radius: 12px; border: none; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
.card-header { display: flex; justify-content: space-between; align-items: center; font-weight: 600; font-size: 16px; }
.selection-hint { font-size: 14px; color: #64748b; margin-bottom: 12px; }
.address-selection { margin-bottom: 10px; }
.w-full { width: 100%; }
.item-row { display: flex; align-items: center; padding: 16px 0; border-bottom: 1px solid #f1f5f9; }
.item-row:last-child { border-bottom: none; }
.item-img { width: 64px; height: 64px; border-radius: 8px; margin-right: 16px; background: #f1f5f9; }
.item-info { flex: 1; }
.item-name { font-size: 15px; font-weight: 500; color: #334155; }
.item-spec { font-size: 13px; color: #94a3b8; margin-top: 4px; }
.item-price { font-size: 14px; color: #64748b; margin-top: 4px; }
.item-total { font-weight: 600; color: #1e293b; }
.discount-row { display: flex; justify-content: space-between; align-items: center; padding: 16px 0; border-bottom: 1px solid #f1f5f9; cursor: pointer; }
.discount-row:last-child { border-bottom: none; cursor: default; }
.value.clickable { color: #ee4d2d; display: flex; align-items: center; gap: 4px; }
.coupon-tag { background: #fee2e2; color: #ef4444; padding: 2px 8px; border-radius: 4px; font-size: 13px; }
.summary-section { background: white; padding: 24px; border-radius: 12px; margin-top: 20px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
.summary-row { display: flex; justify-content: space-between; margin-bottom: 12px; color: #64748b; }
.summary-row.discount { color: #ef4444; }
.summary-row.final { border-top: 1px solid #f1f5f9; padding-top: 16px; margin-top: 16px; color: #1e293b; font-weight: bold; }
.summary-row.final .price { font-size: 24px; color: #ee4d2d; }
.submit-btn { width: 100%; margin-top: 20px; height: 50px; font-size: 18px; font-weight: bold; border-radius: 8px; background: #ee4d2d; border-color: #ee4d2d; }
.coupon-item { padding: 16px; border: 1px solid #e2e8f0; border-radius: 8px; margin-bottom: 12px; cursor: pointer; transition: all 0.2s; }
.coupon-item:hover { border-color: #ee4d2d; background: #fff5f2; }
.coupon-item.selected { border-color: #ee4d2d; background: #fff5f2; position: relative; }
.coupon-title { font-weight: bold; color: #1e293b; margin-bottom: 4px; }
.coupon-desc { font-size: 13px; color: #64748b; }
.coupon-item.none { text-align: center; color: #94a3b8; }
</style>
