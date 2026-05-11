<template>
  <div class="search-page">
    <!-- 麵包屑 + 標題 -->
    <div class="search-header">
      <el-breadcrumb separator="/" class="breadcrumb">
        <el-breadcrumb-item :to="{ path: '/' }">首頁</el-breadcrumb-item>
        <el-breadcrumb-item>搜尋結果</el-breadcrumb-item>
      </el-breadcrumb>
      <h2 class="search-title">
        <template v-if="keyword">「{{ keyword }}」的搜尋結果</template>
        <template v-else>所有商品</template>
        <span v-if="!loading" class="result-count">（共 {{ total }} 件）</span>
      </h2>
    </div>

    <div class="search-layout">
      <!-- ── 左側篩選欄 ───────────────────────────────────── -->
      <aside class="filter-aside">

        <!-- 商品分類 -->
        <div class="filter-block">
          <div class="filter-block-title">商品分類</div>
          <div v-if="catsLoading" class="filter-loading">
            <el-skeleton :rows="5" animated />
          </div>
          <ul v-else class="filter-list">
            <li
              class="filter-item"
              :class="{ active: selectedCategoryId === null }"
              @click="selectCategory(null)"
            >全部分類</li>
            <li
              v-for="cat in categories"
              :key="cat.id"
              class="filter-item"
              :class="{ active: selectedCategoryId === cat.id }"
              @click="selectCategory(cat.id)"
            >{{ cat.name }}</li>
          </ul>
        </div>

        <!-- 價格區間 -->
        <div class="filter-block">
          <div class="filter-block-title">價格區間</div>
          <div class="price-range-head">
            <span>NT$ {{ sliderMin.toLocaleString() }}</span>
            <span>NT$ {{ sliderMax.toLocaleString() }}</span>
          </div>
          <el-slider
            v-model="priceRangeDraft"
            range
            :min="sliderMin"
            :max="sliderMax"
            :step="priceSliderStep"
            :disabled="sliderMax <= sliderMin"
            :format-tooltip="formatSliderTooltip"
            class="price-slider"
            @change="syncPriceInputsFromSlider"
          />
          <div class="price-inputs">
            <el-input
              v-model="priceMinStr"
              placeholder="最低"
              size="small"
              type="number"
              style="flex:1"
              @input="syncSliderFromInputs"
            />
            <span class="price-sep">~</span>
            <el-input
              v-model="priceMaxStr"
              placeholder="最高"
              size="small"
              type="number"
              style="flex:1"
              @input="syncSliderFromInputs"
            />
          </div>
        </div>

        <!-- 預留擴充 -->
        <!-- <div class="filter-block">條件：評分 / 出貨速度 ...</div> -->

      </aside>

      <!-- ── 右側商品區 ────────────────────────────────────── -->
      <div class="search-main">

        <!-- 活動橫幅 (當 promoText 存在時顯示) -->
        <div v-if="promoText" class="promo-banner">
          <div class="promo-content">
            <span class="promo-badge">活動專區</span>
            <h3 class="promo-text">{{ promoText }}</h3>
          </div>
          <div class="promo-decoration">🔥</div>
        </div>

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
          
          <!-- 右側精簡分頁 -->
          <div v-if="total > 0" class="compact-pagination">
            <span class="page-indicator">{{ currentPage }}/{{ totalPages }}</span>
            <el-button
              size="small"
              :icon="ArrowLeft"
              circle
              :disabled="currentPage === 1"
              @click="onPageChange(currentPage - 1)"
            />
            <el-button
              size="small"
              :icon="ArrowRight"
              circle
              :disabled="currentPage === totalPages"
              @click="onPageChange(currentPage + 1)"
            />
          </div>
          
          <div v-if="hasActiveFilters && total === 0" class="clear-filters">
            <el-button link size="small" @click="clearFilters">清除篩選</el-button>
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
              <el-skeleton-item variant="image" style="width:100%;aspect-ratio:1/1;border-radius:8px" />
              <el-skeleton-item variant="p" style="width:90%;margin-top:8px" />
              <el-skeleton-item variant="p" style="width:55%;margin-top:4px" />
            </template>
          </el-skeleton>
        </div>

        <!-- 商品網格 -->
        <div v-else-if="products.length > 0" class="product-grid">
          <ProductCard
            v-for="p in products"
            :key="p.id"
            :product="p"
          />
        </div>

        <!-- 空狀態 -->
        <el-empty
          v-else
          :description="keyword ? `找不到「${keyword}」相關商品` : '目前沒有商品'"
          :image-size="120"
          style="padding: 60px 0"
        >
          <el-button @click="clearFilters">清除所有篩選條件</el-button>
        </el-empty>

        <!-- 分頁 -->
        <div v-if="total > 0" class="pagination-wrap">
          <el-pagination
            background
            layout="prev, pager, next, jumper, total"
            :total="total"
            :page-size="pageSize"
            :current-page="currentPage"
            :disabled="loading"
            @current-change="onPageChange"
          />
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { ArrowDown, ArrowLeft, ArrowRight } from '@element-plus/icons-vue'
import ProductCard from '@/components/product/ProductCard.vue'
import { fetchProductList } from '@/api/product'
import { fetchMainCategories } from '@/api/category'
import type { ProductListItem, FetchProductsParams } from '@/types/product'
import type { Category } from '@/types/category'

type SortBy = 'latest' | 'priceAsc' | 'priceDesc' | 'soldCount'

const route = useRoute()
const router = useRouter()

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

// 總頁數
const totalPages = computed<number>(() => Math.ceil(total.value / pageSize.value))

// ── 從 route.query 讀取當前狀態（computed = 唯一資料來源）──────────
const keyword = computed<string>(() =>
  typeof route.query['keyword'] === 'string' ? route.query['keyword'] : '',
)
const promoText = computed<string>(() =>
  typeof route.query['promoText'] === 'string' ? route.query['promoText'] : '',
)
const selectedCategoryId = computed<number | null>(() => {
  const v = route.query['categoryId']
  if (!v) return null
  const n = Number(v)
  return Number.isNaN(n) ? null : n
})
const selectedSubCategoryId = computed<number | null>(() => {
  const v = route.query['subCategoryId']
  if (!v) return null
  const n = Number(v)
  return Number.isNaN(n) ? null : n
})
const selectedBrandIds = computed<number[]>(() => {
  const v = route.query['brandIds']
  if (!v) return []
  const str = typeof v === 'string' ? v : ''
  return str.split(',').map(Number).filter(n => !Number.isNaN(n))
})
const sortBy = computed<SortBy>(() => {
  const v = route.query['sortBy'] as string
  return (['latest', 'priceAsc', 'priceDesc', 'soldCount'] as const).includes(v as SortBy)
    ? (v as SortBy)
    : 'latest'
})
const currentPage = computed<number>(() => {
  const v = route.query['page']
  const n = v ? Number(v) : 1
  return Number.isNaN(n) ? 1 : Math.max(1, n)
})
const routeMinPrice = computed<number | undefined>(() => {
  const v = route.query['minPrice']
  const n = v ? Number(v) : undefined
  return n !== undefined && !Number.isNaN(n) ? n : undefined
})
const routeMaxPrice = computed<number | undefined>(() => {
  const v = route.query['maxPrice']
  const n = v ? Number(v) : undefined
  return n !== undefined && !Number.isNaN(n) ? n : undefined
})

// ── 本地價格輸入（使用者打字但未套用時的暫存）───────────────────
const priceMinStr = ref<string>(routeMinPrice.value !== undefined ? String(routeMinPrice.value) : '')
const priceMaxStr = ref<string>(routeMaxPrice.value !== undefined ? String(routeMaxPrice.value) : '')
const priceRangeDraft = ref<[number, number]>([0, 0])
let priceFilterTimer: ReturnType<typeof setTimeout> | null = null

// 當 URL 的價格 query 變化時同步輸入框
watch([routeMinPrice, routeMaxPrice], ([min, max]) => {
  priceMinStr.value = min !== undefined ? String(min) : ''
  priceMaxStr.value = max !== undefined ? String(max) : ''
  syncSliderFromRoute()
})

// ── API 狀態 ─────────────────────────────────────────────────────
const products = ref<ProductListItem[]>([])
const total    = ref<number>(0)
const pageSize = ref<number>(20)
const loading  = ref<boolean>(false)
let productRequestSeq = 0

const productPriceBounds = ref<[number, number]>([0, 0])
const priceBoundsLocked = ref<boolean>(false)

const sliderMin = computed<number>(() => {
  const [min] = productPriceBounds.value
  const activeMin = routeMinPrice.value
  return Math.max(0, Math.floor(Math.min(min, activeMin ?? min)))
})

const sliderMax = computed<number>(() => {
  const [, max] = productPriceBounds.value
  const activeMax = routeMaxPrice.value
  return Math.ceil(Math.max(max, activeMax ?? max))
})

const priceSliderStep = computed<number>(() => {
  const span = sliderMax.value - sliderMin.value
  if (span >= 100000) return 1000
  if (span >= 10000) return 100
  return 10
})

// ── 分類清單 ─────────────────────────────────────────────────────
const categories  = ref<Category[]>([])
const catsLoading = ref<boolean>(false)

// ── 是否有啟用的篩選條件 ────────────────────────────────────────
const hasActiveFilters = computed<boolean>(() =>
  !!keyword.value ||
  selectedCategoryId.value !== null ||
  routeMinPrice.value !== undefined ||
  routeMaxPrice.value !== undefined ||
  sortBy.value !== 'latest',
)

// ── URL 更新工具 ────────────────────────────────────────────────
function buildQuery(
  overrides: Partial<{
    keyword: string
    categoryId: number | null
    minPrice: number | null | undefined
    maxPrice: number | null | undefined
    sortBy: SortBy
    page: number
  }> = {},
): Record<string, string> {
  // 🌟 核心修正：以當前 route.query 為基底，確保 promoText 等參數不遺失
  const q: Record<string, string> = { ...route.query } as Record<string, string>

  const merged = {
    keyword:    overrides.keyword    !== undefined ? overrides.keyword    : keyword.value,
    categoryId: overrides.categoryId !== undefined ? overrides.categoryId : selectedCategoryId.value,
    minPrice:   Object.prototype.hasOwnProperty.call(overrides, 'minPrice') ? overrides.minPrice : routeMinPrice.value,
    maxPrice:   Object.prototype.hasOwnProperty.call(overrides, 'maxPrice') ? overrides.maxPrice : routeMaxPrice.value,
    sortBy:     overrides.sortBy     !== undefined ? overrides.sortBy     : sortBy.value,
    page:       overrides.page       !== undefined ? overrides.page       : currentPage.value,
  }

  // 覆蓋/更新篩選條件
  if (merged.keyword)                     q['keyword']    = merged.keyword
  else                                    delete q['keyword']

  if (merged.categoryId !== null)         q['categoryId'] = String(merged.categoryId)
  else                                    delete q['categoryId']

  if (merged.minPrice != null)            q['minPrice']   = String(merged.minPrice)
  else                                    delete q['minPrice']

  if (merged.maxPrice != null)            q['maxPrice']   = String(merged.maxPrice)
  else                                    delete q['maxPrice']

  if (merged.sortBy !== 'latest')         q['sortBy']     = merged.sortBy
  else                                    delete q['sortBy']

  if (merged.page > 1)                    q['page']       = String(merged.page)
  else                                    delete q['page']

  return q
}

function pushQuery(overrides: Parameters<typeof buildQuery>[0]): void {
  void router.push({ path: '/products', query: buildQuery({ ...overrides, page: 1 }) })
}

// ── 篩選操作 ────────────────────────────────────────────────────
function selectCategory(id: number | null): void {
  pushQuery({ categoryId: id })
}

function setSort(value: SortBy): void {
  pushQuery({ sortBy: value })
}

function handleSortCommand(command: string): void {
  if (command === 'priceAsc' || command === 'priceDesc') {
    pushQuery({ sortBy: command })
  }
}

function applyPriceFilter(): void {
  const min = priceMinStr.value ? Number(priceMinStr.value) : undefined
  const max = priceMaxStr.value ? Number(priceMaxStr.value) : undefined
  if (min !== undefined && max !== undefined && min > max) return

  const isFullRange =
    min !== undefined &&
    max !== undefined &&
    min <= sliderMin.value &&
    max >= sliderMax.value

  const isUninitializedRange =
    min === 0 &&
    max === 0 &&
    sliderMin.value === 0 &&
    sliderMax.value === 0

  pushQuery({
    minPrice: isFullRange || isUninitializedRange ? null : min,
    maxPrice: isFullRange || isUninitializedRange ? null : max,
  })
}

function syncPriceInputsFromSlider(value: number | number[]): void {
  if (!Array.isArray(value)) return
  if (sliderMax.value <= sliderMin.value) return
  priceMinStr.value = String(value[0])
  priceMaxStr.value = String(value[1])
  applyPriceFilter()
}

function syncSliderFromRoute(): void {
  const min = routeMinPrice.value ?? sliderMin.value
  const max = routeMaxPrice.value ?? sliderMax.value
  priceRangeDraft.value = [Math.max(sliderMin.value, min), Math.min(sliderMax.value, max)]
}

function resetPriceBounds(): void {
  productPriceBounds.value = [0, 0]
  priceBoundsLocked.value = false
}

function lockInitialPriceBounds(items: ProductListItem[]): void {
  if (priceBoundsLocked.value) return

  const prices = items
    .map(p => p.price)
    .filter((price): price is number => typeof price === 'number' && Number.isFinite(price))

  if (prices.length === 0) return

  productPriceBounds.value = [
    Math.max(0, Math.floor(Math.min(...prices))),
    Math.ceil(Math.max(...prices)),
  ]
  priceBoundsLocked.value = true
}

function formatSliderTooltip(value: number): string {
  return `NT$ ${value.toLocaleString()}`
}

function syncSliderFromInputs(): void {
  const min = priceMinStr.value ? Number(priceMinStr.value) : sliderMin.value
  const max = priceMaxStr.value ? Number(priceMaxStr.value) : sliderMax.value
  if (sliderMax.value <= sliderMin.value && !priceMinStr.value && !priceMaxStr.value) return
  if (!Number.isFinite(min) || !Number.isFinite(max)) return
  const clampedMin = Math.max(sliderMin.value, Math.min(min, sliderMax.value))
  const clampedMax = Math.max(sliderMin.value, Math.min(max, sliderMax.value))
  priceRangeDraft.value = [
    Math.min(clampedMin, clampedMax),
    Math.max(clampedMin, clampedMax),
  ]
  schedulePriceFilter()
}

function schedulePriceFilter(): void {
  if (priceFilterTimer) clearTimeout(priceFilterTimer)
  priceFilterTimer = setTimeout(() => {
    priceFilterTimer = null
    applyPriceFilter()
  }, 350)
}

function clearFilters(): void {
  priceMinStr.value = ''
  priceMaxStr.value = ''
  priceRangeDraft.value = [sliderMin.value, sliderMax.value]
  void router.push({ path: '/products', query: keyword.value ? { keyword: keyword.value } : {} })
}

function onPageChange(page: number): void {
  void router.push({ path: '/products', query: buildQuery({ page }) })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// ── API 呼叫 ────────────────────────────────────────────────────
async function loadProducts(): Promise<void> {
  const requestSeq = ++productRequestSeq
  loading.value = true
  try {
    const params: FetchProductsParams = {
      page:     currentPage.value,
      pageSize: pageSize.value,
      sortBy:   sortBy.value,
    }
    if (keyword.value)                        params.keyword       = keyword.value
    if (selectedCategoryId.value !== null)    params.categoryId    = selectedCategoryId.value
    if (selectedSubCategoryId.value !== null) params.subCategoryId = selectedSubCategoryId.value
    if (selectedBrandIds.value.length > 0)    params.brandIds      = selectedBrandIds.value
    if (routeMinPrice.value !== undefined)    params.minPrice      = routeMinPrice.value
    if (routeMaxPrice.value !== undefined)    params.maxPrice      = routeMaxPrice.value

    const res = await fetchProductList(params)
    if (requestSeq !== productRequestSeq) return
    if (res.success) {
      products.value = res.data.items
      total.value    = res.data.totalCount ?? res.data.total ?? 0
      lockInitialPriceBounds(res.data.items)
      syncSliderFromRoute()
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

async function loadCategories(): Promise<void> {
  catsLoading.value = true
  try {
    const res = await fetchMainCategories()
    if (res.success) {
      categories.value = res.data.filter(c => !/^\d+$/.test(c.name) && c.name.length >= 2)
    }
  } catch {
    // 靜默失敗，不顯示分類也沒關係
  } finally {
    catsLoading.value = false
  }
}

// ── 生命週期 ─────────────────────────────────────────────────────
onMounted(() => {
  void loadProducts()
  void loadCategories()
})

// URL query 變化時重新載入
watch(
  () => route.query,
  () => void loadProducts(),
)

watch(
  () => [
    keyword.value,
    selectedCategoryId.value,
    selectedSubCategoryId.value,
    selectedBrandIds.value.join(','),
  ] as const,
  () => resetPriceBounds(),
  { flush: 'sync' },
)
</script>

<style scoped>
.search-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 24px 30px 60px;
}

/* 標題區 */
.search-header {
  margin-bottom: 20px;
}
.breadcrumb {
  margin-bottom: 10px;
}
.search-title {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}
.result-count {
  font-size: 14px;
  font-weight: 400;
  color: #909399;
  margin-left: 6px;
}

/* 左右分欄 */
.search-layout {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}

/* ── 左側篩選欄 ── */
.filter-aside {
  flex: 0 0 220px;
  width: 220px;
  background: white;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.07);
  position: sticky;
  top: 16px;
}
.filter-block {
  margin-bottom: 24px;
}
.filter-block:last-child {
  margin-bottom: 0;
}
.filter-block-title {
  font-size: 14px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 10px;
  padding-bottom: 8px;
  border-bottom: 1px solid #f0f0f0;
}
.filter-loading {
  padding: 8px 0;
}
.filter-list {
  list-style: none;
  padding: 0;
  margin: 0;
}
.filter-item {
  padding: 7px 10px;
  border-radius: 6px;
  font-size: 13px;
  color: #606266;
  cursor: pointer;
  transition: all 0.15s;
  margin-bottom: 2px;
}
.filter-item:hover {
  background: #fef2f2;
  color: #EE4D2D;
}
.filter-item.active {
  background: #fef2f2;
  color: #EE4D2D;
  font-weight: 600;
}
.price-range-head {
  display: flex;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 2px;
  font-size: 12px;
  color: #94a3b8;
}
.price-slider {
  padding: 0 3px;
  margin-bottom: 6px;
}
:deep(.price-slider .el-slider__bar) {
  background-color: #EE4D2D;
}
:deep(.price-slider .el-slider__button) {
  border-color: #EE4D2D;
}
.price-inputs {
  display: flex;
  align-items: center;
  gap: 6px;
}
.price-sep {
  font-size: 13px;
  color: #909399;
  flex-shrink: 0;
}

/* ── 右側商品區 ── */
.search-main {
  flex: 1;
  min-width: 0;
}

/* 活動橫幅 */
.promo-banner {
  background: linear-gradient(135deg, #ff7e5f 0%, #feb47b 100%);
  border-radius: 12px;
  padding: 16px 24px;
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
  box-shadow: 0 4px 12px rgba(255, 126, 95, 0.25);
  position: relative;
  overflow: hidden;
}
.promo-content {
  z-index: 1;
}
.promo-badge {
  display: inline-block;
  background: rgba(255, 255, 255, 0.25);
  backdrop-filter: blur(4px);
  padding: 2px 10px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 6px;
  border: 1px solid rgba(255, 255, 255, 0.4);
}
.promo-text {
  font-size: 20px;
  font-weight: 800;
  margin: 0;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
.promo-decoration {
  font-size: 48px;
  opacity: 0.3;
  transform: rotate(15deg);
  user-select: none;
}

.sort-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  background: white;
  border-radius: 8px;
  padding: 10px 16px;
  margin-bottom: 16px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.07);
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
.clear-filters {
  margin-left: 12px;
}

/* 精簡分頁（排序列右側） */
.compact-pagination {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-left: auto;
}
.page-indicator {
  font-size: 14px;
  font-weight: 600;
  color: #EE4D2D;
  min-width: 50px;
  text-align: center;
}

/* 商品網格 — 響應式 4 欄 */
.product-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}
@media (max-width: 1100px) {
  .product-grid { grid-template-columns: repeat(3, 1fr); }
}
@media (max-width: 768px) {
  .search-layout { flex-direction: column; }
  .filter-aside { width: 100%; position: static; }
  .product-grid { grid-template-columns: repeat(2, 1fr); }
}
.skeleton-card {
  background: white;
  border-radius: 8px;
  padding: 12px;
}

/* 分頁 */
.pagination-wrap {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}
</style>
