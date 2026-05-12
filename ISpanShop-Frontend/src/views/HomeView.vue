<template>
  <div class="home">
    <!-- ── 輪播 + 右側 banner ── -->
    <section class="banner-section">
      <!-- 真實活動資料（API 有回傳時） -->
      <template v-if="promotions.length > 0">
        <div class="main-carousel">
          <el-carousel height="320px" arrow="always">
            <el-carousel-item v-for="promo in promotions" :key="promo.id">
              <div class="carousel-slide promo-slide" :style="getSlideBackground(promo)" @click="goToActivity(promo)">
                <!-- 模糊底圖（有商品圖時才渲染） -->
                <img
                  v-if="getBannerImage(promo)"
                  :src="getBannerImage(promo)"
                  class="slide-bg-blur"
                  aria-hidden="true"
                />
                <div class="slide-ambient" aria-hidden="true"></div>
                <div class="slide-ribbon" aria-hidden="true"></div>
                <!-- 漸層遮罩：保證文字可讀 -->
                <div class="slide-overlay"></div>
                <!-- 左側文字區 -->
                <div class="slide-content">
                  <div class="slide-header">
                    <span class="slide-tag" :class="promo.type">{{ promo.typeLabel }}</span>
                    <template v-if="countdowns[promo.id]">
                      <div v-if="!countdowns[promo.id]!.expired" class="slide-countdown-inline">
                        <template v-if="countdowns[promo.id]!.days > 0">
                          <span class="cd-num">{{ countdowns[promo.id]!.days }}</span>
                          <span class="cd-label">天</span>
                        </template>
                        <span class="cd-num">{{ countdowns[promo.id]!.hours }}</span>
                        <span class="cd-sep">:</span>
                        <span class="cd-num">{{ countdowns[promo.id]!.minutes }}</span>
                        <span class="cd-sep">:</span>
                        <span class="cd-num">{{ countdowns[promo.id]!.seconds }}</span>
                      </div>
                      <span v-else class="slide-countdown-inline expired">已結束</span>
                    </template>
                  </div>
                  <h2 class="slide-title">{{ promo.title }}</h2>
                  <p v-if="promo.subtitle" class="slide-desc">{{ promo.subtitle }}</p>
                  <el-button type="danger" round size="large" @click.stop="goToActivity(promo)">立即搶購</el-button>
                </div>
                <!-- 右側商品圖拼貼（最多 3 張） -->
                <div
                  v-if="promo.productImages && promo.productImages.length > 0"
                  class="slide-products"
                  :class="`count-${Math.min(promo.productImages.length, 3)}`"
                >
                  <div
                    v-for="(img, i) in promo.productImages.slice(0, 3)"
                    :key="i"
                    class="slide-product-card"
                  >
                    <img :src="formatImageUrl(img)" alt="" />
                  </div>
                </div>
                <!-- 沒有多張商品圖時，顯示單張主圖靠右 -->
                <img
                  v-else-if="getBannerImage(promo)"
                  :src="getBannerImage(promo)"
                  class="slide-main-img"
                />
              </div>
            </el-carousel-item>
          </el-carousel>
        </div>
        <div class="side-banners-grid" style="display: grid; grid-template-rows: repeat(2, 1fr); gap: 16px; height: 320px;">
          <!-- 真實活動副版塊 (第 2, 3 筆) -->
          <div
            v-for="promo in sideBanners"
            :key="promo.id"
            class="side-banner-dynamic"
            :style="!getBannerImage(promo) ? { background: typeGradients[promo.type] ?? typeGradients['other'] } : {}"
            @click="goToActivity(promo)"
          >
            <!-- 背景圖（有圖才渲染，破圖防呆） -->
            <img
              v-if="getBannerImage(promo)"
              :src="getBannerImage(promo)"
              alt=""
              class="sb-bg-img"
              @error="(e: any) => (e.target as HTMLImageElement).style.display = 'none'"
            />
            <div class="sb-ambient" aria-hidden="true"></div>
            <!-- 漸層遮罩 -->
            <div class="sb-overlay"></div>
            <!-- 文字內容 -->
            <div class="sb-content">
              <div class="sb-header">
                <span class="sb-tag" :class="promo.type">{{ promo.typeLabel }}</span>
              </div>
              <h3 class="sb-title">{{ promo.title }}</h3>
              <p v-if="promo.subtitle" class="sb-subtitle">{{ promo.subtitle }}</p>
              <div v-if="countdowns[promo.id] && !countdowns[promo.id]!.expired" class="sb-countdown-bar">
                <span class="sb-cd-icon">⏱</span>
                <template v-if="countdowns[promo.id]!.days > 0">
                  <span class="cd-num-sm">{{ countdowns[promo.id]!.days }}</span>
                  <span class="cd-label-sm">天</span>
                </template>
                <span class="cd-num-sm">{{ countdowns[promo.id]!.hours }}</span>
                <span class="cd-sep-sm">:</span>
                <span class="cd-num-sm">{{ countdowns[promo.id]!.minutes }}</span>
                <span class="cd-sep-sm">:</span>
                <span class="cd-num-sm">{{ countdowns[promo.id]!.seconds }}</span>
              </div>
            </div>
          </div>

          <!-- 補位用 (若資料不足 3 筆時顯示) -->
          <div
            v-for="n in Math.max(0, 2 - sideBanners.length)"
            :key="`placeholder-${n}`"
            class="side-banner side-banner-empty"
            style="border-radius: 12px; background: #1e293b; opacity: 0.3;"
          />
        </div>
      </template>

      <!-- 靜態預設內容（API 空時顯示） -->
      <template v-else>
        <div class="main-carousel">
          <el-carousel height="320px" arrow="always">
            <el-carousel-item v-for="(banner, i) in staticBanners" :key="i">
              <div class="carousel-slide" :style="getSlideBackground(banner)" @click="goToActivity(banner)">
                <div class="slide-ambient" aria-hidden="true"></div>
                <div class="slide-ribbon" aria-hidden="true"></div>
                <!-- 漸層遮罩 -->
                <div class="slide-overlay"></div>
                <!-- 文字與按鈕 -->
                <div class="slide-content">
                  <span class="slide-tag">{{ banner.tag }}</span>
                  <h2 class="slide-title">{{ banner.title }}</h2>
                  <p class="slide-desc">{{ banner.subtitle }}</p>
                  <el-button type="danger" round size="large" @click.stop="goToActivity(banner)">立即搶購</el-button>
                </div>
                <span class="slide-static-emoji">{{ banner.emoji }}</span>
              </div>
            </el-carousel-item>
          </el-carousel>
        </div>
        <div class="side-banners">
          <div class="side-banner" v-for="sb in staticSideBanners" :key="sb.title" :style="{ background: sb.bg }" @click="goToActivity(sb)">
            <div class="sb-tag">{{ sb.tag }}</div>
            <h3>{{ sb.title }}</h3>
            <p>{{ sb.desc }}</p>
            <span class="sb-emoji">{{ sb.emoji }}</span>
          </div>
        </div>
      </template>
    </section>

    <!-- ── 圓形快捷服務 ── -->
    <section class="quick-icons">
      <div v-for="item in quickItems" :key="item.label" class="quick-item" @click="item.url ? $router.push(item.url) : undefined">
        <div class="quick-circle">{{ item.icon }}</div>
        <div class="quick-label">{{ item.label }}</div>
      </div>
    </section>

    <!-- ── 商品分類網格 ── -->
    <section class="category-section">
      <div class="section-title">分類</div>

      <div v-if="categoriesLoading" class="category-grid">
        <el-skeleton v-for="n in 7" :key="n" animated class="cat-skeleton">
          <template #template>
            <el-skeleton-item variant="circle" style="width: 70px; height: 70px; margin: 0 auto 10px;" />
            <el-skeleton-item variant="p" style="width: 70%; margin: 0 auto 4px;" />
          </template>
        </el-skeleton>
      </div>

      <div v-else-if="apiCategories.length > 0" class="category-grid">
        <div
          v-for="cat in apiCategories"
          :key="cat.id"
          class="category-item"
          :class="{ 'category-item--active': selectedCategoryId === cat.id }"
          @click="onCategoryClick(cat.id)"
        >
          <div class="cat-image">
            <el-image
              v-if="cat.iconUrl"
              :src="cat.iconUrl"
              fit="cover"
              style="width: 100%; height: 100%; border-radius: 50%;"
            />
            <span v-else class="cat-emoji">{{ getCategoryIcon(cat.name) }}</span>
          </div>
          <div class="cat-name">{{ cat.name }}</div>
        </div>
      </div>

      <el-empty v-else description="目前沒有分類" :image-size="80" />
    </section>

    <!-- ── 每日新發現 ── -->
    <section ref="sectionRef" class="products-section">
      <div class="discovery-header">
        <h2 class="discovery-title">每日新發現</h2>
        <el-button
          v-if="selectedCategoryId !== null"
          size="small"
          plain
          style="margin-left: 12px; vertical-align: middle;"
          @click="onClearCategory"
        >顯示全部</el-button>
      </div>

      <!-- 手機：篩選按鈕 + 抽屜（選了分類且手機才顯示） -->
      <template v-if="selectedCategoryId !== null && isMobile">
        <div class="mobile-filter-bar">
          <el-button size="small" @click="drawerOpen = true">
            <el-icon><FilterIcon /></el-icon> 篩選條件
          </el-button>
        </div>
        <el-drawer
          v-model="drawerOpen"
          direction="ltr"
          size="280px"
          title="篩選條件"
        >
          <FilterSidebar
            :category-name="selectedCategoryName"
            :sub-categories="subCategories"
            :brands="brands"
            :sub-loading="subLoading"
            :brand-loading="brandLoading"
            :selected-sub-category-id="selectedSubCategoryId"
            :selected-brand-ids="selectedBrandIds"
            :min-price="minPrice"
            :max-price="maxPrice"
            :price-lower-bound="priceLowerBound"
            :price-upper-bound="priceUpperBound"
            :brand-keyword="isBrandSearchKeyword"
            :is-brand-expanded="isBrandListExpanded"
            @clear="onClearCategory"
            @filter-change="onFilterChange"
            @update:selected-sub-category-id="onSubCategoryChange($event)"
            @update:selected-brand-ids="selectedBrandIds = $event"
            @update:min-price="minPrice = $event"
            @update:max-price="maxPrice = $event"
            @update:brand-keyword="isBrandSearchKeyword = $event"
            @update:is-brand-expanded="isBrandListExpanded = $event"
          />
        </el-drawer>
      </template>

      <!-- 主體：邊欄 + 商品牆 -->
      <div
        class="products-layout"
        :class="{ 'with-sidebar': selectedCategoryId !== null && !isMobile }"
      >
        <!-- 左側邊欄（桌機，選了分類才顯示） -->
        <aside v-if="selectedCategoryId !== null && !isMobile" class="filter-aside">
          <FilterSidebar
            :category-name="selectedCategoryName"
            :sub-categories="subCategories"
            :brands="brands"
            :sub-loading="subLoading"
            :brand-loading="brandLoading"
            :selected-sub-category-id="selectedSubCategoryId"
            :selected-brand-ids="selectedBrandIds"
            :min-price="minPrice"
            :max-price="maxPrice"
            :price-lower-bound="priceLowerBound"
            :price-upper-bound="priceUpperBound"
            :brand-keyword="isBrandSearchKeyword"
            :is-brand-expanded="isBrandListExpanded"
            @clear="onClearCategory"
            @filter-change="onFilterChange"
            @update:selected-sub-category-id="onSubCategoryChange($event)"
            @update:selected-brand-ids="selectedBrandIds = $event"
            @update:min-price="minPrice = $event"
            @update:max-price="maxPrice = $event"
            @update:brand-keyword="isBrandSearchKeyword = $event"
            @update:is-brand-expanded="isBrandListExpanded = $event"
          />
        </aside>

        <!-- 右側商品牆 -->
        <div class="products-main">
          <!-- 排序列 -->
          <div class="sort-bar">
            <span class="sort-label">排序：</span>
            <div class="sort-btns">
              <button
                v-for="s in sortOptions"
                :key="s.value"
                class="sort-btn"
                :class="{ active: sortBy === s.value }"
                @click="setSort(s.value as SortBy)"
              >{{ s.label }}</button>

              <!-- 價格下拉選單 -->
              <el-dropdown trigger="click" @command="handleSortCommand">
                <button
                  class="sort-btn"
                  :class="{ active: sortBy === 'priceAsc' || sortBy === 'priceDesc' }"
                >
                  {{ priceLabel }}
                  <el-icon style="margin-left: 4px; vertical-align: middle;"><ArrowDown /></el-icon>
                </button>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item command="priceAsc">價格：低到高</el-dropdown-item>
                    <el-dropdown-item command="priceDesc">價格：高到低</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </div>
          </div>

          <!-- 骨架屏 -->
          <div v-if="loading" class="product-grid">
            <el-skeleton
              v-for="n in 20"
              :key="n"
              animated
              class="skeleton-card"
            >
              <template #template>
                <el-skeleton-item variant="image" class="skeleton-image" />
                <div class="skeleton-body">
                  <el-skeleton-item variant="p" style="width: 90%" />
                  <el-skeleton-item variant="p" style="width: 70%" />
                  <el-skeleton-item variant="p" style="width: 50%" />
                </div>
              </template>
            </el-skeleton>
          </div>

          <!-- 商品牆 -->
          <div v-else-if="products.length > 0" class="product-grid">
            <ProductCard
              v-for="product in products"
              :key="product.id"
              :product="product"
            />
          </div>

          <!-- 空狀態 -->
          <el-empty v-else description="目前沒有商品" :image-size="120" />

          <!-- 查看更多按鈕 -->
          <div v-if="products.length > 0" class="view-more-wrap">
            <el-button
              class="view-more-btn"
              @click="goToProductsPage"
            >
              查看更多
              <el-icon style="margin-left: 6px;"><ArrowRight /></el-icon>
            </el-button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Filter as FilterIcon, ArrowDown, ArrowRight } from '@element-plus/icons-vue'
import ProductCard from '@/components/product/ProductCard.vue'
import FilterSidebar from '@/components/home/FilterSidebar.vue'
import { fetchProductList } from '@/api/product'
import { fetchMainCategories, fetchChildCategories } from '@/api/category'
import { fetchActivePromotions } from '@/api/promotion'
import { fetchBrands } from '@/api/brand'
import { getCategoryIcon } from '@/constants/categoryIcon'
import type { ProductListItem, FetchProductsParams } from '@/types/product'
import type { Category, SubCategory } from '@/types/category'
import type { Promotion } from '@/types/promotion'
import type { Brand } from '@/types/brand'

type SortBy = 'latest' | 'priceAsc' | 'priceDesc' | 'soldCount'

// ── 路由 ─────────────────────────────────────────────────────────
const route = useRoute()
const router = useRouter()

// ── 響應式斷點 ────────────────────────────────────────────────────
const isMobile = ref<boolean>(typeof window !== 'undefined' && window.innerWidth < 768)
function handleResize(): void {
  isMobile.value = window.innerWidth < 768
}

// ── 每日新發現：商品牆 ────────────────────────────────────────────
const products = ref<ProductListItem[]>([])
const loading = ref<boolean>(false)
const currentPage = ref<number>(1)
const pageSize = ref<number>(20) // 固定 20 筆
const total = ref<number>(0)
const sectionRef = ref<HTMLElement | null>(null)
const keyword = ref<string>('')
const sortBy = ref<SortBy>('latest')
let productRequestSeq = 0

// ── 排序選項 ──────────────────────────────────────────────────────
const sortOptions = [
  { value: 'latest',    label: '最新' },
  { value: 'soldCount', label: '銷量' },
]

// 價格排序下拉文字
const priceLabel = computed<string>(() => {
  if (sortBy.value === 'priceAsc') return '價格：低到高'
  if (sortBy.value === 'priceDesc') return '價格：高到低'
  return '價格'
})

// ── 主分類篩選 ───────────────────────────────────────────────────
const selectedCategoryId = ref<number | null>(null)
const apiCategories = ref<Category[]>([])
const categoriesLoading = ref<boolean>(false)

const selectedCategoryName = computed<string>(() => {
  if (selectedCategoryId.value === null) return ''
  return apiCategories.value.find(c => c.id === selectedCategoryId.value)?.name ?? ''
})

// ── 左側篩選邊欄狀態 ─────────────────────────────────────────────
const subCategories = ref<SubCategory[]>([])
const brands = ref<Brand[]>([])
const subLoading = ref<boolean>(false)
const brandLoading = ref<boolean>(false)
const selectedSubCategoryId = ref<number | null>(null)
const selectedBrandIds = ref<number[]>([])
const minPrice = ref<number | null>(null)
const maxPrice = ref<number | null>(null)
const isBrandSearchKeyword = ref<string>('')
const isBrandListExpanded = ref<boolean>(false)
const drawerOpen = ref<boolean>(false)

const priceLowerBound = ref<number>(0)
const priceUpperBound = ref<number>(0)
const priceBoundsLocked = ref<boolean>(false)

// ── 活動/輪播 ────────────────────────────────────────────────────
const promotions = ref<Promotion[]>([])

/** 側邊小 Banner 資料：拿取第 2、3 筆活動 */
const sideBanners = computed(() => {
  if (promotions.value.length <= 1) return []
  return promotions.value.slice(1, 3)
})

// ── 倒數計時 ──────────────────────────────────────────────────────
interface CountdownValue {
  expired: boolean
  days: number
  hours: string
  minutes: string
  seconds: string
}

const countdowns = ref<Record<number, CountdownValue>>({})
let countdownTimer: ReturnType<typeof setInterval> | null = null

const updateCountdowns = (): void => {
  const now = new Date().getTime()
  const result: Record<number, CountdownValue> = {}
  const pad = (n: number): string => String(n).padStart(2, '0')
  for (const item of promotions.value) {
    const end = new Date(item.endDate).getTime()
    const diff = end - now
    if (diff <= 0) {
      result[item.id] = { expired: true, days: 0, hours: '00', minutes: '00', seconds: '00' }
      continue
    }
    const days = Math.floor(diff / (1000 * 60 * 60 * 24))
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    const seconds = Math.floor((diff % (1000 * 60)) / 1000)
    result[item.id] = { expired: false, days, hours: pad(hours), minutes: pad(minutes), seconds: pad(seconds) }
  }
  countdowns.value = result
}

const startCountdown = (): void => {
  if (countdownTimer) clearInterval(countdownTimer)
  console.log('[倒數] 活動數量:', promotions.value.length, '活動 IDs:', promotions.value.map(p => p.id))
  updateCountdowns()
  countdownTimer = setInterval(updateCountdowns, 1000)
}

// ── API 呼叫 ─────────────────────────────────────────────────────

function goToActivity(banner: any): void {
  if (!banner) return
  const target = (banner.linkUrl as string | undefined) || `/promotion/${banner.id as number}`
  void router.push(target)
}

const API_BASE = (import.meta.env['VITE_API_BASE_URL'] as string) || 'https://localhost:7125'

function formatImageUrl(url: string | null | undefined): string {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return API_BASE + url
}

const typeGradients: Record<string, string> = {
  flashSale:  'linear-gradient(135deg, #ff6b35 0%, #f7c948 100%)',
  discount:   'linear-gradient(135deg, #ee4d2d 0%, #ff7849 100%)',
  limitedBuy: 'linear-gradient(135deg, #7c3aed 0%, #a78bfa 100%)',
  other:      'linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)',
}

function getBannerImage(banner: any): string {
  if (!banner) return ''
  if (banner.bannerImageUrl) return formatImageUrl(banner.bannerImageUrl as string)
  if ((banner.productImages as string[] | undefined)?.length) return formatImageUrl(banner.productImages[0] as string)
  return ''
}

function getSlideBackground(banner: any): Record<string, string> {
  if (getBannerImage(banner)) return {}
  if (banner.bg as string | undefined) return { background: banner.bg as string }
  return { background: typeGradients[banner.type as string] ?? typeGradients['other'] ?? '#1a1a2e' }
}

async function loadProducts(): Promise<void> {
  const requestSeq = ++productRequestSeq
  loading.value = true
  try {
    const params: FetchProductsParams = {
      page: currentPage.value,
      pageSize: pageSize.value,
      sortBy: sortBy.value,
    }
    if (selectedCategoryId.value !== null) params.categoryId = selectedCategoryId.value
    if (selectedSubCategoryId.value !== null) params.subCategoryId = selectedSubCategoryId.value
    if (selectedBrandIds.value.length > 0) params.brandIds = selectedBrandIds.value
    if (minPrice.value !== null) params.minPrice = minPrice.value
    if (maxPrice.value !== null) params.maxPrice = maxPrice.value
    if (keyword.value.trim()) params.keyword = keyword.value.trim()

    const res = await fetchProductList(params)
    if (requestSeq !== productRequestSeq) return
    if (res.success) {
      products.value = res.data.items
      total.value = res.data.totalCount ?? res.data.total ?? 0
      lockInitialPriceBounds(res.data.items)
    } else {
      ElMessage.error(res.message || '載入失敗')
    }
  } catch {
    if (requestSeq !== productRequestSeq) return
    ElMessage.error('載入失敗，請稍後再試')
  } finally {
    if (requestSeq === productRequestSeq) loading.value = false
  }
}

function resetPriceBounds(): void {
  priceLowerBound.value = 0
  priceUpperBound.value = 0
  priceBoundsLocked.value = false
}

function lockInitialPriceBounds(items: ProductListItem[]): void {
  if (priceBoundsLocked.value) return

  const prices = items
    .map(p => p.price)
    .filter((price): price is number => typeof price === 'number' && Number.isFinite(price))

  if (prices.length === 0) return

  priceLowerBound.value = Math.max(0, Math.floor(Math.min(...prices)))
  priceUpperBound.value = Math.ceil(Math.max(...prices))
  priceBoundsLocked.value = true
}

async function loadCategories(): Promise<void> {
  categoriesLoading.value = true
  try {
    const res = await fetchMainCategories()
    if (res.success) {
      // 過濾髒資料：純數字或名稱長度 < 2 的分類
      const dirty = res.data.filter(c => /^\d+$/.test(c.name) || c.name.length < 2)
      if (dirty.length > 0) {
        console.warn('⚠️ 偵測到髒分類資料，已過濾（請至後端清理）:', dirty.map(c => `[id:${c.id}] "${c.name}"`))
      }
      apiCategories.value = res.data.filter(c => !/^\d+$/.test(c.name) && c.name.length >= 2)
    } else {
      ElMessage.error(res.message || '分類載入失敗')
    }
  } catch {
    ElMessage.error('分類載入失敗，請稍後再試')
  } finally {
    categoriesLoading.value = false
  }
}

async function loadSubCategories(parentId: number): Promise<void> {
  subLoading.value = true
  subCategories.value = []
  try {
    const res = await fetchChildCategories(parentId)
    if (res.success) subCategories.value = res.data
  } catch {
    // 靜默失敗：沒有子分類就不顯示
  } finally {
    subLoading.value = false
  }
}

async function loadBrands(params: { categoryId: number } | { subCategoryId: number }): Promise<void> {
  brandLoading.value = true
  brands.value = []
  try {
    const res = await fetchBrands(params)
    if (res.success) {
      brands.value = res.data
      // 清理已選品牌中不再存在於新清單的 id
      const validIds = new Set(res.data.map(b => b.id))
      const cleaned = selectedBrandIds.value.filter(id => validIds.has(id))
      if (cleaned.length !== selectedBrandIds.value.length) {
        selectedBrandIds.value = cleaned
      }
    }
  } catch {
    // 靜默失敗：沒有品牌就不顯示
  } finally {
    brandLoading.value = false
  }
}

async function loadPromotions(): Promise<void> {
  try {
    const res = await fetchActivePromotions()
    console.log('[首頁活動] API 回傳數量:', res.data?.length ?? 0)
    console.log('[首頁活動] 活動標題:', res.data?.map(p => `[${p.id}] ${p.title}`))
    if (res.success) {
      promotions.value = res.data
      startCountdown()
    }
  } catch {
    // 靜默失敗，fallback 到靜態內容
  }
}

// ── 篩選操作 ─────────────────────────────────────────────────────

function clearSidebarState(): void {
  selectedSubCategoryId.value = null
  selectedBrandIds.value = []
  minPrice.value = null
  maxPrice.value = null
  resetPriceBounds()
  isBrandSearchKeyword.value = ''
  isBrandListExpanded.value = false
  subCategories.value = []
  brands.value = []
}

function onCategoryClick(categoryId: number): void {
  if (selectedCategoryId.value === categoryId) {
    // 再點同一個分類 → 取消選取
    selectedCategoryId.value = null
    clearSidebarState()
  } else {
    selectedCategoryId.value = categoryId
    clearSidebarState()
    void loadSubCategories(categoryId)
    void loadBrands({ categoryId })
  }
  currentPage.value = 1
  void loadProducts()
}

function onClearCategory(): void {
  selectedCategoryId.value = null
  clearSidebarState()
  drawerOpen.value = false
  currentPage.value = 1
  void loadProducts()
}

function clearHomeFilters(): void {
  selectedCategoryId.value = null
  clearSidebarState()
  drawerOpen.value = false
  currentPage.value = 1
  if (Object.keys(route.query).length > 0) {
    void router.replace({ path: '/', query: {} })
  }
  void loadProducts()
}

function onFilterChange(): void {
  currentPage.value = 1
  void loadProducts()
}

async function onSubCategoryChange(subCategoryId: number | null): Promise<void> {
  selectedSubCategoryId.value = subCategoryId
  minPrice.value = null
  maxPrice.value = null
  resetPriceBounds()
  currentPage.value = 1
  const brandsParams =
    subCategoryId !== null
      ? { subCategoryId }
      : { categoryId: selectedCategoryId.value! }
  await loadBrands(brandsParams)
  void loadProducts()
}

/**
 * 從 route.query 讀取 categoryId / subCategoryId，
 * 套用篩選狀態並重新載入商品。
 * 供 onMounted 和 watch(route.query) 共用。
 */
async function applyQueryFilter(): Promise<void> {
  const rawCatId = route.query['categoryId']
  const rawSubCatId = route.query['subCategoryId']
  const rawKeyword = route.query['keyword']

  const catId = rawCatId ? Number(rawCatId) : null
  const subCatId = rawSubCatId ? Number(rawSubCatId) : null

  // 關鍵字
  keyword.value = typeof rawKeyword === 'string' ? rawKeyword : ''

  if (catId !== null && !Number.isNaN(catId)) {
    clearSidebarState()
    selectedCategoryId.value = catId

    void loadSubCategories(catId)
    const brandsParam =
      subCatId !== null && !Number.isNaN(subCatId)
        ? { subCategoryId: subCatId }
        : { categoryId: catId }
    void loadBrands(brandsParam)

    if (subCatId !== null && !Number.isNaN(subCatId)) {
      selectedSubCategoryId.value = subCatId
    }
  } else {
    selectedCategoryId.value = null
    clearSidebarState()
  }

  currentPage.value = 1
  void loadProducts()
}

function setSort(value: SortBy): void {
  sortBy.value = value
  currentPage.value = 1
  void loadProducts()
}

function handleSortCommand(command: string): void {
  if (command === 'priceAsc' || command === 'priceDesc') {
    sortBy.value = command as SortBy
    currentPage.value = 1
    void loadProducts()
  }
}

function goToProductsPage(): void {
  const query: Record<string, string> = {}
  if (selectedCategoryId.value !== null) {
    query['categoryId'] = String(selectedCategoryId.value)
  }
  if (sortBy.value !== 'latest') {
    query['sortBy'] = sortBy.value
  }
  if (minPrice.value !== null) {
    query['minPrice'] = String(minPrice.value)
  }
  if (maxPrice.value !== null) {
    query['maxPrice'] = String(maxPrice.value)
  }
  void router.push({ path: '/products', query })
}

// ── 生命週期 ─────────────────────────────────────────────────────

onMounted(() => {
  void applyQueryFilter()   // 讀 query 決定是否套分類；沒有 query 就直接載入全部
  void loadCategories()
  void loadPromotions()
  window.addEventListener('resize', handleResize)
  window.addEventListener('howbuy:clear-home-filters', clearHomeFilters)
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
  window.removeEventListener('howbuy:clear-home-filters', clearHomeFilters)
  if (countdownTimer) {
    clearInterval(countdownTimer)
    countdownTimer = null
  }
})

// 若已在首頁時 query 改變（例如連點麵包屑不同分類），重新套用篩選
watch(
  () => route.query,
  () => {
    void applyQueryFilter()
  },
)

// ── 靜態預設輪播資料（API 空時顯示） ────────────────────────────

const staticBanners = [
  { tag: '🎉 會員專屬', title: '購物節送 8 折券', subtitle: '全站 $49 起免運', bg: 'linear-gradient(135deg, #1e293b 0%, #1e1b4b 100%)', emoji: '🚚' },
  { tag: '🔥 限時搶購', title: '3C 家電季', subtitle: '滿萬折千 再送好禮', bg: 'linear-gradient(135deg, #064e3b 0%, #022c22 100%)', emoji: '📱' },
  { tag: '💚 新品上架', title: '春夏新品', subtitle: '時尚穿搭一次擁有', bg: 'linear-gradient(135deg, #1e1b4b 0%, #312e81 100%)', emoji: '👗' },
]

const staticSideBanners = [
  { tag: '商城', title: '新品喇叭上市', desc: '領券現折 $100', bg: 'linear-gradient(135deg, #0f172a 0%, #1e293b 100%)', emoji: '🔊' },
  { tag: '商城', title: '幫你換新機', desc: 'AI 筆電專區', bg: 'linear-gradient(135deg, #1e293b 0%, #334155 100%)', emoji: '💻' },
]

const quickItems = [
  { icon: '🚚', label: '全站大免運' },
  { icon: '📦', label: '免運$99起' },
  { icon: '🎫', label: '領券中心', url: '/coupons' },
  { icon: '💰', label: '全額$49免運' },
  { icon: '👑', label: 'VIP 獨享 18%' },
  { icon: '🛍️', label: 'HowBuy 商城' },
  { icon: '✈️', label: '海外直送' },
  { icon: '💳', label: '銀行刷卡優惠' },
  { icon: '💻', label: 'HowBuy 3C' },
  { icon: '🔥', label: '天天超划算' },
]
</script>

<style scoped>
.home {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px 30px;
}

/* ── 輪播 ── */
.banner-section {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 16px;
  margin-bottom: 24px;
  min-width: 0;
}

.banner-section > *,
.main-carousel,
.side-banners-grid,
.side-banners {
  min-width: 0;
  width: 100%;
}

/* 響應式：中等螢幕 (1024px ~ 768px) - Banner 佔滿，右側卡片改橫排 */
@media (max-width: 1024px) {
  .home {
    padding: 16px 18px;
  }

  .banner-section {
    grid-template-columns: 1fr !important;
    gap: 12px;
    margin-bottom: 18px;
  }

  :deep(.el-carousel__container) {
    height: 240px !important;
  }

  .carousel-slide {
    height: 240px !important;
    padding: 22px 26px;
  }

  .slide-content {
    max-width: min(66%, 520px);
    padding: 0 !important;
  }

  .slide-header {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: 8px;
    margin-bottom: 10px;
  }

  .slide-tag {
    margin-bottom: 0;
    padding: 5px 11px;
    border-radius: 8px;
    font-size: 12px;
  }

  .slide-title {
    max-width: 100%;
    font-size: 25px;
    line-height: 1.22;
    margin-bottom: 10px;
  }

  .slide-desc {
    max-width: 92%;
    font-size: 14px;
    line-height: 1.45;
    margin-bottom: 14px;
  }

  .slide-countdown-inline {
    padding: 4px 8px;
    gap: 2px;
  }

  .slide-countdown-inline .cd-num {
    min-width: 20px;
    padding: 2px 5px;
    font-size: 12px;
  }

  .slide-countdown-inline .cd-label,
  .slide-countdown-inline .cd-sep {
    font-size: 11px;
  }

  .slide-content :deep(.el-button--large) {
    min-height: 36px;
    padding: 8px 18px;
    font-size: 14px;
  }

  .slide-products {
    right: 22px;
    max-width: 34%;
    padding: 10px;
  }

  .slide-product-card,
  .slide-products.count-1 .slide-product-card,
  .slide-products.count-2 .slide-product-card,
  .slide-products.count-3 .slide-product-card {
    width: 78px;
    height: 78px;
  }

  .slide-products.count-3 {
    width: auto;
    padding: 12px;
    gap: 8px;
  }

  .slide-main-img {
    right: 28px;
    max-width: 190px;
    max-height: 170px;
  }

  .side-banners-grid, .side-banners {
    height: auto !important;
    grid-template-columns: repeat(2, 1fr) !important;
    grid-template-rows: 1fr !important;
    display: grid !important;
    gap: 12px;
  }

  .side-banner-dynamic,
  .side-banner {
    min-height: 126px;
  }

  .sb-content {
    padding: 13px 16px 12px;
  }

  .sb-tag {
    padding: 3px 9px;
    border-radius: 7px;
    font-size: 10px;
    margin-bottom: 6px;
  }

  .sb-title {
    max-width: 92%;
    font-size: 15px;
    line-height: 1.28;
    margin-bottom: 4px;
  }

  .sb-subtitle {
    max-width: 88%;
    font-size: 12px;
    line-height: 1.35;
  }

  .sb-countdown-bar {
    padding: 3px 8px;
    margin-top: auto;
  }

  .sb-countdown-bar .cd-num-sm {
    min-width: 17px;
    padding: 1px 4px;
    font-size: 10px;
  }

  .quick-icons { 
    grid-template-columns: repeat(5, 1fr); 
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }
  .quick-icons::-webkit-scrollbar {
    height: 4px;
  }
  .quick-icons::-webkit-scrollbar-thumb {
    background: #cbd5e1;
    border-radius: 2px;
  }
}

@media (min-width: 1025px) and (max-width: 1280px) {
  .slide-content {
    max-width: 43%;
    padding-left: 44px;
  }

  .slide-title {
    max-width: 360px;
    font-size: 30px;
  }

  .slide-desc {
    max-width: 370px;
    font-size: 15px;
  }

  .slide-products {
    right: 28px;
    max-width: 48%;
    padding: 12px;
    border-radius: 24px;
  }

  .slide-products.count-3 {
    width: 56%;
    padding: 22px;
  }

  .slide-products.count-3 .slide-product-card {
    width: 92px;
    height: 92px;
  }

  .slide-product-card {
    width: 104px;
    height: 104px;
    border-radius: 16px;
  }

  .slide-products.count-1 .slide-product-card {
    width: 124px;
    height: 124px;
  }

  .slide-products.count-2 .slide-product-card {
    width: 112px;
    height: 112px;
  }
}

/* 響應式：小螢幕 (平板直向/手機) */
@media (max-width: 900px) {
  .home {
    padding: 12px 15px;
    overflow-x: hidden;
  }
  
  .banner-section {
    display: flex !important;
    flex-direction: column !important;
    gap: 10px;
    margin-bottom: 16px;
  }
  
  /* 主輪播改成簡潔卡片風格 */
  :deep(.el-carousel__container) {
    height: 200px !important;
    width: 100% !important;
    overflow: hidden !important;
  }
  
  .carousel-slide {
    height: 200px !important;
    padding: 16px 20px;
    width: 100% !important;
    min-width: 0 !important;
  }
  
  /* 文字區域精簡 */
  .slide-content {
    padding: 0 !important;
    max-width: 100%;
    width: 100%;
    height: auto;
    justify-content: flex-start;
    align-items: flex-start !important;
    text-align: left;
  }
  
  .slide-header {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 8px !important;
  }
  
  .slide-tag {
    font-size: 11px !important;
    padding: 3px 10px !important;
    margin-bottom: 0 !important;
  }
  
  .slide-title {
    font-size: clamp(18px, 5vw, 22px) !important;
    line-height: 1.3 !important;
    margin: 6px 0 !important;
    /* 最多兩行 */
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }
  
  .slide-desc {
    font-size: 13px !important;
    margin-bottom: 10px !important;
    line-height: 1.4;
    /* 最多兩行 */
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }
  
  /* 倒數計時精簡 */
  .slide-countdown-inline {
    padding: 3px 8px !important;
    font-size: 11px !important;
  }
  .slide-countdown-inline .cd-num {
    font-size: 11px !important;
    min-width: 18px !important;
    padding: 2px 4px !important;
  }
  .slide-countdown-inline .cd-label,
  .slide-countdown-inline .cd-sep {
    font-size: 10px !important;
  }
  
  /* 按鈕縮小 */
  .slide-content :deep(.el-button--large) {
    padding: 6px 16px !important;
    font-size: 13px !important;
    min-height: 34px !important;
  }
  
  /* 隱藏右側商品圖片和主圖 */
  .slide-products,
  .slide-main-img,
  .slide-static-emoji {
    display: none !important;
  }
  
  /* 遮罩加深讓文字更清晰 */
  .slide-overlay {
    background: linear-gradient(180deg, rgba(0,0,0,0.55) 0%, rgba(0,0,0,0.75) 100%) !important;
  }
  
  /* 右側小卡片：改成垂直排列 */
  .side-banners-grid, 
  .side-banners {
    display: flex !important;
    flex-direction: column !important;
    grid-template-columns: unset !important;
    grid-template-rows: unset !important;
    gap: 10px;
    overflow-x: visible !important;
    padding-bottom: 0;
    height: auto !important;
    width: 100% !important;
  }
  
  .side-banner-dynamic,
  .side-banner {
    flex: none !important;
    min-width: unset !important;
    max-width: none !important;
    width: 100% !important;
    height: 132px !important;
    min-height: 132px !important;
  }
  
  .sb-content {
    padding: 14px 16px !important;
    justify-content: center;
  }
  
  .sb-tag {
    font-size: 10px !important;
    padding: 2px 8px !important;
    margin-bottom: 6px !important;
  }
  
  .sb-title {
    font-size: 16px !important;
    margin-bottom: 4px !important;
    line-height: 1.32;
    max-width: 88% !important;
    -webkit-line-clamp: 1;
  }
  
  .sb-subtitle {
    font-size: 12px !important;
    line-height: 1.35;
    -webkit-line-clamp: 1;
    max-width: 88% !important;
  }
  
  .sb-countdown-bar {
    font-size: 10px !important;
    margin-top: 6px !important;
  }
  
  .sb-countdown-bar .cd-num-sm {
    font-size: 10px !important;
    min-width: 16px !important;
  }
  
  /* 快捷功能列：自動換行，不用拖拉 */
  .quick-icons { 
    display: flex !important;
    grid-template-columns: unset !important;
    flex-wrap: wrap !important;
    justify-content: center;
    overflow-x: visible !important;
    padding: 12px 8px; 
    gap: 8px 10px;
  }
  
  .quick-item {
    flex: 0 0 auto;
    width: calc(20% - 8px);
    min-width: 60px;
    max-width: 70px;
  }
  
  .quick-circle { 
    width: 42px; 
    height: 42px; 
    font-size: 18px; 
    margin: 0 auto 6px;
  }
  
  .quick-label { 
    font-size: 10px;
    line-height: 1.2;
  }
}

@media (max-width: 520px) {
  .banner-section {
    gap: 10px;
  }

  :deep(.el-carousel__container) {
    height: 220px !important;
  }

  .carousel-slide {
    height: 220px !important;
    padding: 16px !important;
  }

  .slide-content {
    justify-content: center !important;
  }

  .slide-header {
    flex-wrap: wrap;
    gap: 6px !important;
  }

  .slide-title {
    font-size: clamp(20px, 7vw, 24px) !important;
    line-height: 1.22 !important;
    max-width: 92% !important;
  }

  .slide-desc {
    font-size: 12px !important;
    max-width: 92% !important;
    -webkit-line-clamp: 2;
  }

  .slide-countdown-inline {
    max-width: 100%;
    overflow: hidden;
  }

  .slide-countdown-inline .cd-num {
    min-width: 17px !important;
    padding: 2px 3px !important;
  }

  .side-banner-dynamic,
  .side-banner {
    height: 120px !important;
    min-height: 120px !important;
  }

  .sb-content {
    padding: 12px 16px !important;
  }

  .sb-title {
    font-size: 15px !important;
    max-width: 92% !important;
    -webkit-line-clamp: 2;
  }

  .sb-subtitle {
    max-width: 92% !important;
  }

  .sb-countdown-bar {
    padding: 3px 8px !important;
    max-width: 100%;
  }
}

.main-carousel {
  border-radius: 14px;
  overflow: hidden;
  box-shadow: 0 18px 45px rgba(15, 23, 42, 0.16);
}

/* 每個 slide */
.carousel-slide {
  position: relative;
  height: 320px;
  overflow: hidden;
  display: flex;
  align-items: center;
  isolation: isolate;
  background:
    radial-gradient(circle at 78% 18%, rgba(255, 153, 89, 0.24), transparent 30%),
    radial-gradient(circle at 18% 70%, rgba(132, 77, 255, 0.28), transparent 36%),
    linear-gradient(135deg, #090b18 0%, #17172c 48%, #222234 100%);
  cursor: pointer;
}

.carousel-slide::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: 1;
  pointer-events: none;
  opacity: 0.46;
  background-image:
    linear-gradient(rgba(255,255,255,0.055) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 42px 42px;
  mask-image: linear-gradient(90deg, rgba(0,0,0,0.85), rgba(0,0,0,0.12));
}

.carousel-slide::after {
  content: '';
  position: absolute;
  inset: 0;
  z-index: 2;
  pointer-events: none;
  background:
    linear-gradient(115deg, rgba(255,255,255,0.16) 0%, rgba(255,255,255,0.035) 18%, transparent 38%),
    radial-gradient(circle at 48% 100%, rgba(238, 77, 45, 0.32), transparent 32%);
  mix-blend-mode: screen;
}

/* 模糊底圖 */
.slide-bg-blur {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: blur(26px) saturate(1.35) brightness(0.42);
  transform: scale(1.16);
  z-index: 0;
  opacity: 0.78;
}

.slide-ambient {
  position: absolute;
  inset: -24%;
  z-index: 1;
  pointer-events: none;
  background:
    radial-gradient(circle at 74% 42%, rgba(255, 255, 255, 0.22), transparent 18%),
    radial-gradient(circle at 22% 18%, rgba(238, 77, 45, 0.28), transparent 26%),
    radial-gradient(circle at 42% 88%, rgba(20, 184, 166, 0.2), transparent 28%);
  filter: blur(4px);
}

.slide-ribbon {
  position: absolute;
  right: -86px;
  top: -120px;
  width: 440px;
  height: 520px;
  z-index: 2;
  pointer-events: none;
  background: linear-gradient(140deg, rgba(255,255,255,0.28), rgba(255,255,255,0.04) 42%, transparent 70%);
  transform: rotate(18deg);
  border-left: 1px solid rgba(255,255,255,0.18);
}

/* 漸層遮罩：左深右透，保障文字可讀 */
.slide-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(3, 7, 18, 0.9) 0%, rgba(15, 23, 42, 0.62) 43%, rgba(15, 23, 42, 0.18) 100%),
    linear-gradient(0deg, rgba(0,0,0,0.42), transparent 48%);
  z-index: 3;
  pointer-events: none;
}

/* 左側文字區 */
.slide-content {
  position: relative;
  z-index: 10;
  padding: 40px 40px 40px 60px;
  max-width: min(46%, 440px);
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  color: white;
}

.slide-content :deep(.el-button) {
  border: 0;
  background: linear-gradient(135deg, #ff765f 0%, #ee4d2d 52%, #ff9f43 100%);
  box-shadow: 0 12px 24px rgba(238, 77, 45, 0.34);
  font-weight: 700;
}

/* 活動類型標籤 */
.slide-tag {
  display: inline-block;
  align-self: flex-start;
  padding: 7px 14px;
  border-radius: 9px;
  font-size: 13px;
  font-weight: 800;
  margin-bottom: 12px;
  color: #fff;
  background: rgba(238,77,45,0.92);
  box-shadow: 0 8px 18px rgba(238, 77, 45, 0.24);
}
.slide-tag.flashSale  { background: linear-gradient(135deg, #ff7a45, #ee4d2d); }
.slide-tag.discount   { background: linear-gradient(135deg, #f97316, #dc2626); }
.slide-tag.limitedBuy { background: linear-gradient(135deg, #9333ea, #4f46e5); }
.slide-tag.other      { background: linear-gradient(135deg, #334155, #0f172a); }

.slide-title {
  max-width: 420px;
  font-size: clamp(28px, 3.1vw, 34px);
  font-weight: 850;
  margin: 0 0 14px;
  text-shadow: 0 4px 20px rgba(0,0,0,0.48);
  line-height: 1.18;
  letter-spacing: 0;
  word-break: break-word;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.slide-desc {
  max-width: 430px;
  font-size: clamp(15px, 1.45vw, 17px);
  opacity: 0.92;
  margin-bottom: 20px;
  text-shadow: 0 1px 8px rgba(0,0,0,0.36);
  line-height: 1.55;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* 右側商品圖拼貼 */
.slide-products {
  position: absolute;
  right: 42px;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  gap: clamp(8px, 1vw, 14px);
  z-index: 5;
  width: auto;
  max-width: min(47%, 500px);
  justify-content: center;
  padding: clamp(12px, 1.3vw, 18px);
  border-radius: 28px;
  background: linear-gradient(135deg, rgba(255,255,255,0.2), rgba(255,255,255,0.08));
  border: 1px solid rgba(255,255,255,0.18);
  box-shadow: inset 0 1px 0 rgba(255,255,255,0.2), 0 24px 50px rgba(0,0,0,0.28);
  backdrop-filter: blur(10px);
}
.slide-products.count-1 {
  padding: 18px;
  border-radius: 26px;
}
.slide-products.count-2 {
  padding: 16px;
  border-radius: 26px;
}
.slide-products.count-3 {
  width: min(54%, 620px);
  gap: clamp(12px, 1.4vw, 20px);
  padding: clamp(24px, 2.2vw, 30px);
}
.slide-products.count-3 .slide-product-card {
  width: clamp(84px, 8vw, 112px);
  height: clamp(84px, 8vw, 112px);
}
.slide-product-card {
  width: clamp(96px, 9.2vw, 130px);
  height: clamp(96px, 9.2vw, 130px);
  border-radius: 18px;
  overflow: hidden;
  background: linear-gradient(180deg, #fff, #f8fafc);
  border: 1px solid rgba(255,255,255,0.9);
  box-shadow: 0 14px 30px rgba(0,0,0,0.26);
  flex-shrink: 0;
  transition: transform 0.3s;
}
.slide-products.count-1 .slide-product-card {
  width: clamp(118px, 11vw, 150px);
  height: clamp(118px, 11vw, 150px);
}
.slide-products.count-2 .slide-product-card {
  width: clamp(108px, 10vw, 140px);
  height: clamp(108px, 10vw, 140px);
}
.slide-product-card:nth-child(2) { transform: translateY(12px); }
.slide-product-card:nth-child(3) { transform: translateY(-10px); }
.slide-products.count-1 .slide-product-card,
.slide-products.count-2 .slide-product-card:nth-child(2) {
  transform: none;
}
.slide-product-card:hover { transform: translateY(-6px) scale(1.03); }
.slide-product-card:nth-child(2):hover { transform: translateY(6px) scale(1.03); }
.slide-product-card:nth-child(3):hover { transform: translateY(-16px) scale(1.03); }
.slide-products.count-1 .slide-product-card:hover,
.slide-products.count-2 .slide-product-card:nth-child(2):hover {
  transform: translateY(-4px) scale(1.03);
}
.slide-product-card img { width: 100%; height: 100%; object-fit: contain; padding: 8px; }

/* 單張主圖（無多張商品圖時） */
.slide-main-img {
  position: absolute;
  right: 60px;
  top: 50%;
  transform: translateY(-50%);
  max-height: 240px;
  max-width: 280px;
  object-fit: contain;
  z-index: 5;
  filter: drop-shadow(0 24px 30px rgba(0,0,0,0.4));
}

/* 靜態 fallback 用 emoji */
.slide-static-emoji {
  position: absolute;
  right: 60px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 140px;
  opacity: 0.32;
  z-index: 5;
  pointer-events: none;
  filter: drop-shadow(0 20px 28px rgba(0,0,0,0.28));
}

/* 右側小 Banner */
.side-banner-dynamic {
  position: relative;
  border-radius: 14px;
  overflow: hidden;
  cursor: pointer;
  isolation: isolate;
  background:
    radial-gradient(circle at 82% 18%, rgba(255, 190, 92, 0.28), transparent 34%),
    linear-gradient(135deg, #111827 0%, #25213a 100%);
  box-shadow: 0 14px 32px rgba(15, 23, 42, 0.15);
  transition: transform 0.3s, box-shadow 0.3s;
}
.side-banner-dynamic::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: 1;
  pointer-events: none;
  opacity: 0.38;
  background-image:
    linear-gradient(rgba(255,255,255,0.055) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.045) 1px, transparent 1px);
  background-size: 30px 30px;
}
.side-banner-dynamic:hover {
  transform: translateY(-4px);
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.2);
}

/* 靜態 side-banner 補位與 fallback */
.side-banner {
  position: relative;
  padding: 16px 20px;
  color: white;
  border-radius: 14px;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  justify-content: center;
  transition: transform 0.3s, box-shadow 0.3s;
  overflow: hidden;
  isolation: isolate;
  box-shadow: 0 14px 32px rgba(15, 23, 42, 0.15);
}
.side-banner::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: 1;
  pointer-events: none;
  background:
    radial-gradient(circle at 78% 22%, rgba(255,255,255,0.18), transparent 28%),
    linear-gradient(115deg, rgba(255,255,255,0.12), transparent 34%);
}
.side-banner:hover {
  transform: translateY(-4px);
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.2);
}
.side-banner h3 { margin: 0 0 4px; font-size: 18px; font-weight: 700; position: relative; z-index: 2; }
.side-banner p { margin: 0; font-size: 13px; opacity: 0.85; position: relative; z-index: 2; }
.sb-emoji { position: absolute; right: 10px; bottom: -10px; font-size: 60px; opacity: 0.2; z-index: 1; }

.sb-bg-img {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  z-index: 0;
  filter: saturate(1.08) contrast(1.04);
}
.sb-ambient {
  position: absolute;
  inset: -30%;
  z-index: 1;
  pointer-events: none;
  background:
    radial-gradient(circle at 28% 24%, rgba(238, 77, 45, 0.28), transparent 28%),
    radial-gradient(circle at 80% 78%, rgba(99, 102, 241, 0.24), transparent 30%);
  filter: blur(2px);
}
.sb-overlay {
  position: absolute;
  inset: 0;
  background:
    linear-gradient(90deg, rgba(3, 7, 18, 0.88) 0%, rgba(15, 23, 42, 0.58) 52%, rgba(15, 23, 42, 0.18) 100%),
    linear-gradient(0deg, rgba(0,0,0,0.35), transparent 54%);
  z-index: 2;
}
.sb-content {
  position: relative;
  z-index: 3;
  padding: 18px 22px 16px;
  color: white;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: flex-start;
}
.sb-tag {
  display: inline-block;
  align-self: flex-start;
  padding: 5px 11px;
  border-radius: 8px;
  font-size: 11px;
  font-weight: 800;
  margin-bottom: 8px;
  background: rgba(238,77,45,0.85);
  box-shadow: 0 8px 16px rgba(0,0,0,0.2);
}
.sb-tag.flashSale  { background: linear-gradient(135deg, #ff7a45, #ee4d2d); }
.sb-tag.discount   { background: linear-gradient(135deg, #f97316, #dc2626); }
.sb-tag.limitedBuy { background: linear-gradient(135deg, #9333ea, #4f46e5); }
.sb-tag.other      { background: linear-gradient(135deg, #334155, #0f172a); }
.sb-title {
  margin: 0 0 4px;
  max-width: 88%;
  font-size: clamp(16px, 1.28vw, 18px);
  font-weight: 850;
  line-height: 1.32;
  letter-spacing: 0;
  text-shadow: 0 3px 14px rgba(0,0,0,0.42);
  word-break: break-word;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.sb-subtitle {
  margin: 0;
  max-width: 82%;
  font-size: clamp(12px, 0.92vw, 13px);
  opacity: 0.9;
  text-shadow: 0 2px 10px rgba(0,0,0,0.36);
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* ── 快捷服務 ── */
.quick-icons {
  background: white;
  border-radius: 12px;
  padding: 24px;
  display: grid;
  grid-template-columns: repeat(10, 1fr);
  gap: 12px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}
.quick-item { text-align: center; cursor: pointer; transition: transform 0.2s; }
.quick-item:hover { transform: translateY(-4px); }
.quick-circle {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: linear-gradient(135deg, #FFF0ED 0%, #FCDBD5 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  margin: 0 auto 8px;
  box-shadow: 0 2px 6px rgba(238,77,45,0.15);
}
.quick-label { font-size: 12px; color: #475569; }

/* ── 商品分類 ── */
.category-section {
  background: white;
  border-radius: 12px;
  padding: 24px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}
.section-title {
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 20px;
  padding-left: 12px;
  border-left: 4px solid #EE4D2D;
}
.category-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 12px;
}

/* 響應式：中等螢幕 */
@media (max-width: 1024px) { 
  .category-grid { 
    grid-template-columns: repeat(6, 1fr); 
  } 
}

@media (max-width: 768px) { 
  .category-grid { 
    grid-template-columns: repeat(4, 1fr);
    gap: 8px;
  }
  .category-section {
    padding: 16px;
  }
  .cat-image {
    width: 56px;
    height: 56px;
  }
  .cat-emoji {
    font-size: 24px;
  }
  .cat-name {
    font-size: 12px;
  }
}

@media (max-width: 480px) { 
  .category-grid { 
    grid-template-columns: repeat(3, 1fr);
    gap: 6px;
  }
}

.category-item {
  text-align: center;
  padding: 16px 8px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  border: 2px solid transparent;
}
.category-item:hover { background: #FFF5F3; transform: translateY(-3px); }
.category-item--active { background: #FFF5F3; border-color: #EE4D2D; }
.cat-image {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 10px;
  overflow: hidden;
}
.cat-emoji { font-size: 32px; line-height: 1; }
.cat-name { font-size: 13px; color: #334155; font-weight: 500; }
.cat-skeleton { text-align: center; padding: 16px 8px; }

@media (min-width: 769px) and (max-width: 1280px) {
  .quick-icons {
    grid-template-columns: repeat(5, minmax(0, 1fr)) !important;
    gap: 16px 10px;
    padding: 20px 28px;
    overflow: visible;
  }

  .quick-circle {
    width: 52px;
    height: 52px;
    font-size: 25px;
  }

  .quick-label {
    min-height: 32px;
    line-height: 1.28;
  }

  .category-section {
    padding: 22px 26px;
  }

  .category-grid {
    grid-template-columns: repeat(7, minmax(0, 1fr));
    gap: 10px;
  }

  .category-item {
    padding: 14px 4px;
  }

  .cat-image {
    width: 62px;
    height: 62px;
    margin-bottom: 9px;
  }

  .cat-emoji {
    font-size: 29px;
  }

  .cat-name {
    font-size: 12px;
    line-height: 1.28;
  }
}

@media (min-width: 901px) and (max-width: 1280px) {
  .quick-icons {
    display: grid !important;
    grid-template-columns: repeat(5, minmax(120px, 1fr)) !important;
    justify-items: center;
  }

  .quick-item {
    width: 100%;
    max-width: 132px;
  }
}

@media (max-width: 768px) {
  .quick-icons {
    display: grid !important;
    grid-template-columns: repeat(5, minmax(0, 1fr)) !important;
    gap: 14px 8px;
    padding: 18px 14px;
    overflow: visible !important;
  }

  .quick-item {
    width: auto;
    min-width: 0;
    max-width: none;
  }

  .quick-circle {
    width: 48px;
    height: 48px;
    font-size: 22px;
  }

  .quick-label {
    font-size: 11px;
    line-height: 1.25;
  }

  .category-grid {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 12px 10px;
  }

  .category-item,
  .cat-skeleton {
    flex: 0 0 calc(25% - 8px);
    max-width: 116px;
    min-width: 82px;
    padding: 14px 6px;
  }

  .cat-image {
    width: 58px;
    height: 58px;
    margin-bottom: 8px;
  }

  .cat-emoji {
    font-size: 26px;
  }

  .cat-name {
    font-size: 12px;
    line-height: 1.28;
  }
}

@media (max-width: 520px) {
  .quick-icons {
    grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
  }

  .category-item,
  .cat-skeleton {
    flex-basis: calc(33.333% - 8px);
    min-width: 78px;
  }
}

/* ── 每日新發現 ── */
.products-section {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}
.discovery-header {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
  position: relative;
}
.discovery-title {
  display: inline-block;
  font-size: 22px;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
  padding-bottom: 8px;
  border-bottom: 3px solid #EE4D2D;
}
.discovery-header::before,
.discovery-header::after {
  content: '';
  position: absolute;
  top: 50%;
  width: calc(50% - 90px);
  height: 1px;
  background: #e2e8f0;
  transform: translateY(-6px);
}
.discovery-header::before { left: 0; }
.discovery-header::after  { right: 0; }

/* 手機篩選按鈕列 */
.mobile-filter-bar {
  margin-bottom: 16px;
}

/* ── 雙欄 layout（Flexbox） ── */
.products-layout {
  /* 無邊欄：products-main 自動撐滿 */
}
.products-layout.with-sidebar {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}
.filter-aside {
  width: 240px;
  flex-shrink: 0;
  /* 黏性定位，讓邊欄跟著捲動時固定 */
  position: sticky;
  top: 16px;
}
.products-main {
  flex: 1;
  min-width: 0; /* 防止 flex 子元素撐破容器 */
}

/* 商品格線（無邊欄：6 欄；有邊欄：5 欄） */
.product-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 12px;
  margin-bottom: 20px;
}
.with-sidebar .product-grid {
  grid-template-columns: repeat(5, 1fr);
}

/* 響應式：大螢幕筆電 (1200px ~ 1024px) */
@media (max-width: 1200px) {
  .product-grid { 
    grid-template-columns: repeat(5, 1fr); 
  }
  .with-sidebar .product-grid { 
    grid-template-columns: repeat(4, 1fr); 
  }
}

/* 響應式：中等筆電/平板橫向 (1024px ~ 768px) */
@media (max-width: 1024px) {
  .product-grid { 
    grid-template-columns: repeat(4, 1fr); 
  }
  .with-sidebar .product-grid { 
    grid-template-columns: repeat(3, 1fr); 
  }
}

/* 響應式：平板直向 (768px ~ 576px) */
@media (max-width: 768px) {
  .product-grid { 
    grid-template-columns: repeat(3, 1fr); 
    gap: 8px;
  }
  .with-sidebar .product-grid { 
    grid-template-columns: repeat(2, 1fr); 
    gap: 8px;
  }
  
  /* 邊欄在小螢幕隱藏，使用抽屜 */
  .filter-aside {
    display: none;
  }
}

/* 響應式：手機 (< 576px) */
@media (max-width: 576px) {
  .product-grid,
  .with-sidebar .product-grid { 
    grid-template-columns: repeat(2, 1fr); 
    gap: 8px;
  }
}

/* 排序列 */
.sort-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  background: #fafafa;
  border-radius: 8px;
  padding: 10px 16px;
  margin-bottom: 16px;
}
.sort-label {
  font-size: 13px;
  color: #606266;
  flex-shrink: 0;
}
.sort-btns {
  display: flex;
  gap: 6px;
}
.sort-btn {
  padding: 5px 16px;
  border: 1px solid #dcdfe6;
  background: white;
  border-radius: 4px;
  font-size: 13px;
  color: #606266;
  cursor: pointer;
  transition: all 0.15s;
}
.sort-btn:hover {
  border-color: #EE4D2D;
  color: #EE4D2D;
}
.sort-btn.active {
  background: #EE4D2D;
  border-color: #EE4D2D;
  color: white;
  font-weight: 600;
}

/* 查看更多按鈕 */
.view-more-wrap {
  display: flex;
  justify-content: center;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 1px solid #f1f5f9;
}
.view-more-btn {
  width: 280px;
  height: 44px;
  font-size: 15px;
  font-weight: 600;
  color: #EE4D2D;
  border-color: #EE4D2D;
  background: white;
  transition: all 0.3s;
}
.view-more-btn:hover {
  background: #EE4D2D;
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(238, 77, 45, 0.3);
}

/* 骨架屏卡片 */
.skeleton-card { border-radius: 4px; overflow: hidden; border: 1px solid #f1f5f9; }
.skeleton-image { width: 100%; aspect-ratio: 1 / 1; }
.skeleton-body { padding: 10px 12px 12px; display: flex; flex-direction: column; gap: 8px; }

/* ── 倒數計時 ── */
.slide-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 10px;
}
.slide-countdown-inline {
  display: inline-flex;
  align-items: center;
  gap: 2px;
  background: rgba(255, 255, 255, 0.12);
  backdrop-filter: blur(8px);
  padding: 4px 10px;
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.15);
}
.slide-countdown-inline .cd-num {
  display: inline-block;
  background: rgba(255, 255, 255, 0.2);
  color: #fff;
  font-size: 13px;
  font-weight: 700;
  min-width: 24px;
  text-align: center;
  padding: 2px 4px;
  border-radius: 4px;
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
}
.slide-countdown-inline .cd-sep {
  color: rgba(255, 255, 255, 0.5);
  font-size: 12px;
  font-weight: 700;
  margin: 0;
}
.slide-countdown-inline .cd-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 11px;
  margin: 0 3px 0 1px;
}
.slide-countdown-inline.expired {
  background: rgba(255, 255, 255, 0.1);
  padding: 4px 12px;
  border-radius: 20px;
  color: rgba(255, 255, 255, 0.5);
  font-size: 12px;
}
.sb-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 6px;
}
.sb-countdown-bar {
  display: inline-flex;
  align-items: center;
  gap: 2px;
  margin-top: auto;
  background: rgba(0, 0, 0, 0.25);
  padding: 4px 10px;
  border-radius: 14px;
  max-width: 100%;
}
.sb-cd-icon {
  font-size: 11px;
  margin-right: 4px;
  opacity: 0.8;
}
.sb-countdown-bar .cd-num-sm {
  display: inline-block;
  background: rgba(255, 255, 255, 0.2);
  color: #fff;
  font-size: 11px;
  font-weight: 700;
  min-width: 20px;
  text-align: center;
  padding: 1px 3px;
  border-radius: 3px;
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
}
.sb-countdown-bar .cd-sep-sm {
  color: rgba(255, 255, 255, 0.5);
  font-size: 10px;
  font-weight: 700;
}
.sb-countdown-bar .cd-label-sm {
  color: rgba(255, 255, 255, 0.7);
  font-size: 10px;
  margin: 0 2px 0 1px;
}

/* 輪播圓點 */
:deep(.el-carousel__indicator--horizontal .el-carousel__button) {
  background: rgba(255,255,255,0.5); width: 30px; height: 4px; border-radius: 2px;
}
:deep(.el-carousel__indicator--horizontal.is-active .el-carousel__button) { background: #EE4D2D; }
:deep(.el-carousel__arrow) { background: rgba(0,0,0,0.4); }
:deep(.el-carousel__arrow:hover) { background: #EE4D2D; }
</style>
