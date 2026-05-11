<template>
  <div class="member-level-container" v-loading="loading">
    <!-- 上半部：個人等級概況 -->
    <el-card class="status-card" shadow="hover" v-if="!loading">
      <div class="user-level-info">
        <div class="level-badge-section">
          <div class="level-icon" :style="{ backgroundColor: currentLevelStyles.color }">
            <el-icon :size="40"><Trophy /></el-icon>
          </div>
          <div class="level-names">
            <span class="current-label">目前等級</span>
            <!-- 將按鈕移到這裡，並使用 flex 讓它們水平排列 -->
            <div class="level-name-row">
              <h2 class="level-name">{{ currentLevel.levelName }}</h2>
              <el-button
                type="primary"
                size="small"
                plain
                round
                @click="generateDemoOrder"
                :loading="demoLoading"
              >
                🎁 生成 Demo 訂單 (NT$ 4,499)
              </el-button>
            </div>
            <!-- 提示訊息移到名稱下方 -->
            <span class="demo-tip-inline" v-if="demoMessage">{{ demoMessage }}</span>
          </div>
        </div>

        <div class="stats-grid">
          <div class="stat-item">
            <span class="stat-label">累積消費金額</span>
            <span class="stat-value">NT$ {{ formatNumber(realTotalSpending) }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">計算區間</span>
            <span class="stat-value text-small">{{ calculationPeriod }}</span>
          </div>
        </div>
      </div>

      <div class="progress-section">
        <div class="progress-header">
          <span>等級進度</span>
          <!-- ↓ 修改：依據 periodStatus 顯示對應提示 -->
          <span v-if="nextLevel && periodStatus === 'active'" class="next-level-tip">
            再消費 <strong>NT$ {{ formatNumber(neededForNext) }}</strong> 即可升級至 <strong>{{ nextLevel.levelName }}</strong>
          </span>
          <span v-else-if="nextLevel && periodStatus === 'lastday'" class="next-level-tip next-level-tip--urgent">
            ⚠️ 今日截止，再消費 <strong>NT$ {{ formatNumber(neededForNext) }}</strong> 即可升級至 <strong>{{ nextLevel.levelName }}</strong>
          </span>
          <span v-else-if="periodStatus === 'expired'" class="next-level-tip next-level-tip--expired">
            計算區間已結束，等待系統重新結算等級…
          </span>
          <span v-else class="next-level-tip">您已達到最高等級！</span>
          <!-- ↑ 修改結束 -->
        </div>
        <el-progress
          :percentage="progressPercentage"
          :stroke-width="16"
          :format="progressFormat"
          :color="currentLevelStyles.color"
        />
        <div class="progress-footer">
          <span>NT$ {{ formatNumber(realTotalSpending) }}</span>
          <span v-if="nextLevel">NT$ {{ formatNumber(Number(nextLevel.minSpending)) }}</span>
        </div>
      </div>

      <div class="update-info" v-if="statsInfo.updatedAt">
        最後更新時間：{{ statsInfo.updatedAt }} (數據每 24 小時同步一次)
      </div>
    </el-card>

    <!-- 下半部：等級說明與權益 -->
    <el-card class="rules-card" shadow="never">
      <template #header>
        <div class="card-header">
          <el-icon><InfoFilled /></el-icon>
          <span>會員等級說明</span>
        </div>
      </template>

      <el-table :data="levelRules" style="width: 100%" border stripe>
        <el-table-column prop="levelName" label="等級名稱" width="150" align="center">
          <template #default="scope">
            <span class="custom-level-tag" :style="getLevelTagStyle(scope.row)">
              {{ scope.row.levelName }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="minSpending" label="升級門檻 (累積消費)" align="right">
          <template #default="scope">
            NT$ {{ formatNumber(Number(scope.row.minSpending)) }}
          </template>
        </el-table-column>
        <el-table-column prop="discountRate" label="專屬權益" align="center">
          <template #default="scope">
            <span v-if="Number(scope.row.discountRate) < 1" class="highlight-text">
              {{ (Number(scope.row.discountRate) * 10).toFixed(1) }} 折優惠
            </span>
            <span v-else>—</span>
          </template>
        </el-table-column>

        <!-- 更新：有效期動態判斷 -->
        <el-table-column label="有效期" align="center">
          <template #default="scope">
            <span v-if="Number(scope.row.minSpending) === 0">永久有效</span>
            <span v-else class="highlight-text">12 個月</span>
          </template>
        </el-table-column>
      </el-table>

      <!-- 更新：等級計算規則說明微調 -->
      <div class="rules-footer">
        <h3>等級計算規則：</h3>
        <ul>
          <li>系統將根據您在過去 12 個月內的「已完成」訂單總額進行計算。</li>
          <li>達成升級門檻後，系統將自動為您升級，<strong>新等級有效期為 12 個月</strong>。</li>
          <li>有效期屆滿時，系統將重新結算過去 12 個月的消費總額，以決定您的新等級。</li>
          <li>若發生退貨導致累積金額低於門檻，系統將保留調整等級之權利。</li>
        </ul>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Trophy, InfoFilled } from '@element-plus/icons-vue'
import { getLevelDetail } from '@/api/member'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
interface MembershipLevel {
  id: number;
  levelName: string;
  minSpending: number | string;
  discountRate: number | string;
  color?: string;
}

const loading = ref(true)
const demoLoading = ref(false)
const demoMessage = ref('')
const realTotalSpending = ref(0)
const levelRules = ref<MembershipLevel[]>([])
const progressPercentage = ref(0)
const neededForNext = ref(0)
const nextLevelName = ref('')
const currentLevelName = ref('')

const levelColors: Record<number, string> = {
  1: '#EE4D2D',
  2: '#64748b',
  3: '#f59e0b'
}

const statsInfo = ref({
  startDate: '',
  endDate: '',
  updatedAt: ''
})

const fetchLevelData = async () => {
  try {
    loading.value = true
    const response = await getLevelDetail()
    const data = response.data

    realTotalSpending.value = data.currentTotalSpending
    currentLevelName.value = data.currentLevelName
    nextLevelName.value = data.nextLevelName
    progressPercentage.value = data.progressPercent
    neededForNext.value = data.nextLevelThreshold - data.currentTotalSpending

    levelRules.value = data.allLevels.map((l: any) => ({
      id: l.levelId,
      levelName: l.name,
      minSpending: l.minSpending,
      discountRate: l.discountRate,
      color: levelColors[l.levelId] || '#94a3b8'
    }))

    const formatDate = (dateStr: string) => dateStr.split('T')[0]

    statsInfo.value = {
      startDate: formatDate(data.calculationStartDate),
      endDate: formatDate(data.calculationEndDate),
      updatedAt: new Date().toLocaleString()
    }
  } catch (error) {
    console.error('獲取等級資訊失敗:', error)
    ElMessage.error('無法取得會員等級數據')
  } finally {
    loading.value = false
  }
}

const calculationPeriod = computed(() => {
  return statsInfo.value.startDate ? `${statsInfo.value.startDate} ～ ${statsInfo.value.endDate}` : '載入中...'
})

// ↓ 新增：判斷計算區間狀態
const periodStatus = computed((): 'active' | 'lastday' | 'expired' => {
  if (!statsInfo.value.endDate) return 'active'

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const endDate = new Date(statsInfo.value.endDate)
  endDate.setHours(0, 0, 0, 0)

  const diffMs = endDate.getTime() - today.getTime()
  const diffDays = Math.round(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays < 0) return 'expired'
  if (diffDays === 0) return 'lastday'
  return 'active'
})
// ↑ 新增結束

const currentLevel = computed(() => {
  if (levelRules.value.length === 0) return { levelName: '載入中...', color: '#94a3b8' }
  const spending = Number(realTotalSpending.value)
  const sorted = [...levelRules.value].sort((a, b) => Number(b.minSpending) - Number(a.minSpending))
  const level = sorted.find(l => spending >= Number(l.minSpending)) || levelRules.value[0]

  if (level.levelName && level.levelName !== '載入中...') {
    authStore.updateLevel(level.levelName)
  }

  return level
})

const nextLevel = computed(() => {
  if (levelRules.value.length === 0) return null
  return levelRules.value.find(l => l.levelName === nextLevelName.value) || null
})

const currentLevelStyles = computed(() => {
  return {
    color: currentLevel.value.color || '#94a3b8'
  }
})

const formatNumber = (num: number) => {
  return Math.max(0, Math.floor(num)).toLocaleString()
}

const progressFormat = () => {
  return nextLevel.value ? `${progressPercentage.value.toFixed(0)}%` : 'MAX'
}

const getLevelTagStyle = (row: MembershipLevel) => {
  const baseColor = row.color || '#94a3b8'
  return {
    backgroundColor: baseColor + '15',
    color: baseColor,
    border: `1px solid ${baseColor}30`,
    padding: '4px 12px',
    borderRadius: '6px',
    fontSize: '13px',
    fontWeight: '600'
  }
}

onMounted(() => {
  fetchLevelData()
})

const generateDemoOrder = async () => {
  demoLoading.value = true
  demoMessage.value = ''
  try {
    const response = await fetch('/api/front/demo/create-order', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify({
        productId: 500,
        quantity: 1,
        recipientName: '示範客戶',
        recipientPhone: '0900000000',
        recipientAddress: '示範地址'
      })
    })

    if (!response.ok) {
      throw new Error('建立訂單失敗')
    }

    const data = await response.json()
    demoMessage.value = `✅ 成功建立訂單！訂單號：${data.orderNumber}`

    setTimeout(() => {
      fetchLevelData()
      demoMessage.value = ''
    }, 2000)

  } catch (error) {
    console.error('Demo 訂單建立失敗:', error)
    demoMessage.value = `❌ 建立失敗：${error instanceof Error ? error.message : '未知錯誤'}`
  } finally {
    demoLoading.value = false
  }
}
</script>

<style scoped>
.member-level-container {
  max-width: 900px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.status-card {
  background: white;
  border-radius: 16px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.user-level-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.level-badge-section {
  display: flex;
  align-items: center;
  gap: 20px;
}

.level-icon {
  color: white;
  width: 72px;
  height: 72px;
  border-radius: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.level-names {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.current-label {
  font-size: 13px;
  color: #94a3b8;
  margin-bottom: 4px;
}

.level-name-row {
  display: flex;
  align-items: center;
  gap: 12px;
}

.level-name {
  font-size: 26px;
  font-weight: 800;
  color: #1e293b;
  margin: 0;
}

.demo-tip-inline {
  margin-top: 6px;
  font-size: 12px;
  font-weight: 600;
  color: #059669;
}

.stats-grid {
  display: flex;
  gap: 40px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.stat-label {
  font-size: 13px;
  color: #94a3b8;
  margin-bottom: 6px;
}

.stat-value {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
}

.text-small {
  font-size: 13px;
}

.progress-section {
  padding: 10px 0;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 14px;
  color: #64748b;
}

.next-level-tip strong {
  color: #ee4d2d;
}

/* ↓ 新增兩個狀態樣式 */
.next-level-tip--urgent {
  color: #e6a23c;
}

.next-level-tip--urgent strong {
  color: #e6a23c;
}

.next-level-tip--expired {
  color: #909399;
  font-style: italic;
}
/* ↑ 新增結束 */

.progress-footer {
  display: flex;
  justify-content: space-between;
  margin-top: 8px;
  font-size: 12px;
  color: #94a3b8;
  font-weight: 600;
}

.update-info {
  margin-top: 20px;
  font-size: 12px;
  color: #94a3b8;
  text-align: right;
  font-style: italic;
}

.rules-card {
  border-radius: 16px;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 17px;
  font-weight: 700;
  color: #1e293b;
}

.custom-level-tag {
  display: inline-block;
  white-space: nowrap;
}

.highlight-text {
  color: #ee4d2d;
  font-weight: 700;
}

.rules-footer {
  margin-top: 30px;
  background-color: #f8fafc;
  padding: 20px;
  border-radius: 12px;
}

.rules-footer h3 {
  font-size: 15px;
  color: #1e293b;
  margin-top: 0;
  margin-bottom: 10px;
}

.rules-footer ul {
  margin: 0;
  padding-left: 20px;
  color: #64748b;
  font-size: 13px;
  line-height: 1.8;
}

:deep(.el-progress-bar__outer) {
  background-color: #f1f5f9;
}
</style>
