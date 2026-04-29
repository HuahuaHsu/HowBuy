<template>
  <div class="preview-page">
    <!-- 預覽模式橫幅 -->
    <div class="preview-banner">
      <el-icon><View /></el-icon>
      <span>這是商品預覽畫面，僅您本人可見。審核通過上架後，買家才能看到此商品。</span>
      <el-button size="small" plain @click="goBack">← 回到我的商品</el-button>
    </div>

    <!-- 載入中 -->
    <div v-if="loading" class="loading-wrap">
      <el-skeleton :rows="8" animated />
    </div>

    <!-- 錯誤 -->
    <div v-else-if="error" class="error-wrap">
      <el-result icon="error" :title="error">
        <template #extra>
          <el-button type="primary" @click="goBack">回到我的商品</el-button>
        </template>
      </el-result>
    </div>

    <!-- 商品內容 -->
    <div v-else-if="product" class="product-wrap">
      <!-- 狀態標籤 -->
      <div class="status-row">
        <el-tag :type="statusTagType" size="large">{{ product.statusText }}</el-tag>
        <el-tag v-if="product.rejectReason" type="danger" size="large" style="margin-left:8px">
          退回原因：{{ product.rejectReason }}
        </el-tag>
      </div>

      <div class="product-main">
        <!-- 左：圖片 -->
        <div class="image-col">
          <el-carousel
            v-if="product.images.length > 1"
            height="420px"
            :interval="4000"
            indicator-position="outside"
          >
            <el-carousel-item v-for="(img, i) in product.images" :key="i">
              <img :src="getFullImageUrl(img)" class="product-img" :alt="`商品圖 ${i + 1}`" />
            </el-carousel-item>
          </el-carousel>
          <div v-else-if="product.images.length === 1" class="single-image">
            <img :src="getFullImageUrl(product.images[0]!)" class="product-img" alt="商品圖" />
          </div>
          <div v-else class="no-image">
            <el-icon :size="64"><Picture /></el-icon>
            <p>尚無圖片</p>
          </div>
        </div>

        <!-- 右：資訊 -->
        <div class="info-col">
          <h1 class="product-name">{{ product.name }}</h1>

          <div class="price-row">
            <span v-if="product.minPrice !== null && product.maxPrice !== null">
              <span v-if="product.minPrice === product.maxPrice" class="price">
                NT$ {{ product.minPrice.toLocaleString() }}
              </span>
              <span v-else class="price">
                NT$ {{ product.minPrice.toLocaleString() }} ～ {{ product.maxPrice.toLocaleString() }}
              </span>
            </span>
            <span v-else class="price price--unset">（尚未設定價格）</span>
          </div>

          <el-divider />

          <div class="meta-grid">
            <div class="meta-item">
              <span class="meta-label">分類</span>
              <span>{{ product.categoryName || '—' }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">品牌</span>
              <span>{{ product.brandName || '—' }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">商品 ID</span>
              <span>{{ product.id }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">建立時間</span>
              <span>{{ formatDate(product.createdAt) }}</span>
            </div>
          </div>

          <!-- 規格 -->
          <div v-if="product.variants.length > 0" class="variants-section">
            <el-divider content-position="left">規格明細</el-divider>
            <el-table :data="product.variants" size="small" stripe>
              <el-table-column prop="variantName" label="規格名稱" />
              <el-table-column prop="skuCode" label="SKU" width="140" />
              <el-table-column label="售價" width="110">
                <template #default="{ row }">NT$ {{ row.price.toLocaleString() }}</template>
              </el-table-column>
              <el-table-column prop="stock" label="庫存" width="80" />
            </el-table>
          </div>
        </div>
      </div>

      <!-- 商品描述 -->
      <el-divider content-position="left">商品描述</el-divider>
      <div
        class="description-wrap"
        v-html="product.description || '<p style=\'color:#999\'>（尚無描述）</p>'"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { View, Picture } from '@element-plus/icons-vue'
import { getSellerProductDetail } from '@/api/product'
import type { SellerProductDetail } from '@/types/product'

const route = useRoute()
const router = useRouter()

const loading = ref(true)
const error = ref<string | null>(null)
const product = ref<SellerProductDetail | null>(null)

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? ''

function getFullImageUrl(url: string): string {
  if (!url) return ''
  if (url.startsWith('http://') || url.startsWith('https://') || url.startsWith('blob:')) return url
  return `${BASE_URL}${url.startsWith('/') ? '' : '/'}${url}`
}

function formatDate(dateStr: string | null): string {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('zh-TW', { year: 'numeric', month: '2-digit', day: '2-digit' })
}

const statusTagType = computed(() => {
  switch (product.value?.statusText) {
    case '已上架': return 'success'
    case '審核中': return 'warning'
    case '已退回': return 'danger'
    default: return 'info'
  }
})

function goBack(): void {
  router.push('/seller/products')
}

onMounted(async () => {
  const id = Number(route.params.id)
  if (!id || isNaN(id)) {
    error.value = '無效的商品 ID'
    loading.value = false
    return
  }
  try {
    product.value = await getSellerProductDetail(id)
  } catch (e: any) {
    const status = e?.response?.status
    if (status === 403) error.value = '您沒有權限預覽此商品'
    else if (status === 404) error.value = '商品不存在'
    else error.value = '載入失敗，請稍後再試'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.preview-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.preview-banner {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 24px;
  background: #fdf6ec;
  border-bottom: 1px solid #f5dab1;
  color: #e6a23c;
  font-size: 14px;
  position: sticky;
  top: 0;
  z-index: 100;
}

.preview-banner span {
  flex: 1;
}

.loading-wrap,
.error-wrap {
  max-width: 900px;
  margin: 40px auto;
  padding: 0 20px;
}

.product-wrap {
  max-width: 1100px;
  margin: 24px auto;
  padding: 0 20px 40px;
}

.status-row {
  margin-bottom: 16px;
}

.product-main {
  display: flex;
  gap: 40px;
  background: #fff;
  border-radius: 8px;
  padding: 28px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, .06);
}

.image-col {
  width: 420px;
  flex-shrink: 0;
}

.product-img {
  width: 100%;
  height: 420px;
  object-fit: contain;
  background: #fafafa;
  border-radius: 6px;
}

.no-image {
  width: 100%;
  height: 420px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #f5f7fa;
  border-radius: 6px;
  color: #bbb;
}

.info-col {
  flex: 1;
  min-width: 0;
}

.product-name {
  font-size: 22px;
  font-weight: 600;
  margin: 0 0 16px;
  line-height: 1.4;
}

.price-row {
  margin-bottom: 4px;
}

.price {
  font-size: 28px;
  font-weight: 700;
  color: #e4393c;
}

.price--unset {
  font-size: 16px;
  color: #999;
}

.meta-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px 24px;
  margin-bottom: 8px;
}

.meta-item {
  display: flex;
  gap: 8px;
  font-size: 14px;
  color: #555;
}

.meta-label {
  color: #999;
  white-space: nowrap;
}

.variants-section {
  margin-top: 8px;
}

.description-wrap {
  background: #fff;
  border-radius: 8px;
  padding: 24px 28px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, .06);
  line-height: 1.8;
  font-size: 15px;
  color: #333;
}

.description-wrap :deep(img) {
  max-width: 100%;
  height: auto;
}

@media (max-width: 768px) {
  .product-main {
    flex-direction: column;
  }

  .image-col {
    width: 100%;
  }

  .product-img {
    height: 260px;
  }

  .meta-grid {
    grid-template-columns: 1fr;
  }
}
</style>
