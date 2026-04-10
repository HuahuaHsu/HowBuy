<template>
  <div style="padding: 40px; max-width: 800px; margin: 0 auto;">

    <h1>Element Plus 強大功能展示</h1>

    <!-- 1. 訊息提示 -->
    <el-divider content-position="left">1. 訊息提示(Bootstrap 要寫 50 行)</el-divider>
    <el-button @click="showSuccess">成功訊息</el-button>
    <el-button @click="showError">錯誤訊息</el-button>
    <el-button @click="showConfirm">確認彈窗</el-button>

    <!-- 2. 商品數量 -->
    <el-divider content-position="left">2. 商品數量加減(Bootstrap 沒有)</el-divider>
    <el-input-number v-model="quantity" :min="1" :max="10" />
    <p>目前數量:{{ quantity }}</p>

    <!-- 3. 商品評分 -->
    <el-divider content-position="left">3. 商品評分(Bootstrap 沒有)</el-divider>
    <el-rate v-model="rating" show-score />

    <!-- 4. 表單驗證 -->
    <el-divider content-position="left">4. 表單自動驗證(Bootstrap 要寫 100 行)</el-divider>
    <el-form :model="form" :rules="rules" ref="formRef" label-width="80px">
      <el-form-item label="Email" prop="email">
        <el-input v-model="form.email" placeholder="請輸入 Email" />
      </el-form-item>
      <el-form-item label="密碼" prop="password">
        <el-input v-model="form.password" type="password" show-password />
      </el-form-item>
      <el-form-item label="手機" prop="phone">
        <el-input v-model="form.phone" placeholder="09xxxxxxxx" />
      </el-form-item>
      <el-button type="primary" @click="submitForm">註冊</el-button>
    </el-form>

    <!-- 5. 表格 -->
    <el-divider content-position="left">5. 訂單表格(可排序)(Bootstrap 要寫 200 行)</el-divider>
    <el-table :data="orders" stripe border>
      <el-table-column prop="orderNo" label="訂單編號" sortable />
      <el-table-column prop="customer" label="顧客" />
      <el-table-column prop="amount" label="金額" sortable>
        <template #default="{ row }">
          NT$ {{ row.amount.toLocaleString() }}
        </template>
      </el-table-column>
      <el-table-column label="狀態">
        <template #default="{ row }">
          <el-tag :type="row.status === '已付款' ? 'success' : 'warning'">
            {{ row.status }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>

  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance } from 'element-plus'

// 數量
const quantity = ref(1)

// 評分
const rating = ref(4)

// 表單
const formRef = ref<FormInstance>()
const form = reactive({
  email: '',
  password: '',
  phone: '',
})
const rules = {
  email: [
    { required: true, message: 'Email 必填', trigger: 'blur' },
    { type: 'email', message: 'Email 格式不正確', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '密碼必填', trigger: 'blur' },
    { min: 8, message: '密碼至少 8 個字', trigger: 'blur' },
  ],
  phone: [
    { pattern: /^09\d{8}$/, message: '請輸入正確的手機格式(09 開頭)', trigger: 'blur' },
  ],
}

// 訂單資料
const orders = [
  { orderNo: 'A001', customer: '王小明', amount: 1280, status: '已付款' },
  { orderNo: 'A002', customer: '陳大華', amount: 3590, status: '待付款' },
  { orderNo: 'A003', customer: '林美麗', amount: 890, status: '已付款' },
  { orderNo: 'A004', customer: '張小強', amount: 5200, status: '待付款' },
]

// 函式
function showSuccess() {
  ElMessage.success('已成功加入購物車!')
}

function showError() {
  ElMessage.error('庫存不足!')
}

function showConfirm() {
  ElMessageBox.confirm('確定要刪除這項商品嗎?', '提示', {
    confirmButtonText: '確定',
    cancelButtonText: '取消',
    type: 'warning',
  }).then(() => {
    ElMessage.success('已刪除')
  }).catch(() => {
    ElMessage.info('已取消')
  })
}

function submitForm() {
  formRef.value?.validate((valid) => {
    if (valid) {
      ElMessage.success('註冊成功!')
    } else {
      ElMessage.error('請檢查表單錯誤')
    }
  })
}
</script>