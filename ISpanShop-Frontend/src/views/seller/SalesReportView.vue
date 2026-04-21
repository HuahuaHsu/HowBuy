<template>
  <div class="sales-report-container">
    <div class="page-header">
      <h2 class="title">銷售報表</h2>
      <div class="header-actions">
        <el-tag type="info">數據更新時間：{{ lastUpdateTime }}</el-tag>
      </div>
    </div>

    <div v-loading="loading" class="dashboard-content">
      <!-- KPI Cards -->
      <el-row :gutter="20" class="kpi-row">
        <el-col :xs="24" :sm="12" :md="6">
          <el-card shadow="hover" class="kpi-card">
            <div class="kpi-icon revenue"><el-icon><Money /></el-icon></div>
            <div class="kpi-info">
              <div class="kpi-label">總營收</div>
              <div class="kpi-value">${{ formatPrice(data?.kpis.totalRevenue || 0) }}</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6">
          <el-card shadow="hover" class="kpi-card">
            <div class="kpi-icon orders"><el-icon><Document /></el-icon></div>
            <div class="kpi-info">
              <div class="kpi-label">總訂單數</div>
              <div class="kpi-value">{{ data?.kpis.totalOrders || 0 }}</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6">
          <el-card shadow="hover" class="kpi-card">
            <div class="kpi-icon products"><el-icon><Goods /></el-icon></div>
            <div class="kpi-info">
              <div class="kpi-label">在架商品</div>
              <div class="kpi-value">{{ data?.kpis.totalProducts || 0 }}</div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6">
          <el-card shadow="hover" class="kpi-card">
            <div class="kpi-icon warning"><el-icon><Warning /></el-icon></div>
            <div class="kpi-info">
              <div class="kpi-label">低庫存警告</div>
              <div class="kpi-value warning-text">{{ data?.kpis.lowStockCount || 0 }}</div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- Charts & Tables -->
      <el-row :gutter="20" class="main-row">
        <el-col :xs="24" :lg="16">
          <el-card shadow="never" class="chart-card">
            <template #header>
              <div class="card-header">
                <span>近七日銷售趨勢</span>
              </div>
            </template>
            <div class="chart-wrapper">
              <apexchart
                v-if="data"
                type="line"
                height="350"
                :options="chartOptions"
                :series="data.salesTrend.series"
              ></apexchart>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :lg="8">
          <el-card shadow="never" class="top-products-card">
            <template #header>
              <div class="card-header">
                <span>熱銷商品排行</span>
              </div>
            </template>
            <el-table :data="data?.topProducts" style="width: 100%" size="small">
              <el-table-column type="index" label="排名" width="50" align="center" />
              <el-table-column prop="productName" label="商品名稱" show-overflow-tooltip />
              <el-table-column prop="salesVolume" label="銷量" width="60" align="right" />
              <el-table-column label="營收" width="90" align="right">
                <template #default="{ row }">
                  ${{ formatPrice(row.salesRevenue) }}
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { Money, Document, Goods, Warning } from '@element-plus/icons-vue';
import { getSellerDashboardApi } from '@/api/store';
import type { SellerDashboardData } from '@/types/store';
import { ElMessage } from 'element-plus';
import VueApexCharts from 'vue3-apexcharts';

const apexchart = VueApexCharts;
const loading = ref(false);
const data = ref<SellerDashboardData | null>(null);

const lastUpdateTime = computed(() => {
  return new Date().toLocaleString();
});

const chartOptions = ref({
  chart: {
    id: 'sales-trend',
    toolbar: { show: false }
  },
  xaxis: {
    categories: [] as string[]
  },
  colors: ['#ee4d2d'],
  stroke: {
    curve: 'smooth',
    width: 3
  },
  markers: {
    size: 4
  },
  yaxis: {
    labels: {
      formatter: (val: number) => `$${val.toLocaleString()}`
    }
  }
});

const fetchDashboardData = async () => {
  loading.value = true;
  try {
    const res = await getSellerDashboardApi();
    data.value = res.data;
    chartOptions.value.xaxis.categories = res.data.salesTrend.labels;
  } catch (error: any) {
    console.error('獲取賣場數據失敗', error);
    ElMessage.error(error.response?.data?.message || '獲取賣場數據失敗');
  } finally {
    loading.value = false;
  }
};

const formatPrice = (price: number) => {
  return new Intl.NumberFormat('zh-TW').format(price);
};

onMounted(() => {
  fetchDashboardData();
});
</script>

<style scoped lang="scss">
.sales-report-container {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  
  .title { 
    margin: 0; 
    font-size: 22px; 
    font-weight: 700;
    color: #1e293b;
  }
}

.kpi-row {
  margin-bottom: 24px;
}

.kpi-card {
  border: 1px solid #e8eaf0;
  border-radius: 12px;
  margin-bottom: 16px;

  :deep(.el-card__body) {
    display: flex;
    align-items: center;
    gap: 20px;
    padding: 24px;
  }

  .kpi-icon {
    width: 52px;
    height: 52px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 24px;
    flex-shrink: 0;
    
    &.revenue { background-color: #fff1f0; color: #ff4d4f; }
    &.orders { background-color: #e6f7ff; color: #1890ff; }
    &.products { background-color: #f6ffed; color: #52c41a; }
    &.warning { background-color: #fffbe6; color: #faad14; }
  }

  .kpi-info {
    .kpi-label { font-size: 13px; color: #64748b; margin-bottom: 4px; }
    .kpi-value { font-size: 24px; font-weight: bold; color: #1e293b; }
    .warning-text { color: #faad14; }
  }
}

.main-row {
  .chart-card, .top-products-card {
    height: 100%;
    margin-bottom: 24px;
    border: 1px solid #e8eaf0;
    border-radius: 12px;
  }
}

.card-header {
  font-weight: bold;
  color: #1e293b;
}

.chart-wrapper {
  padding: 10px 0;
}
</style>
