<template>
  <div class="seller-apply-container" v-loading="initialLoading">
    <template v-if="!initialLoading">
      <!-- 1. 審核中狀態 -->
      <el-card v-if="status === 'Pending'" class="status-card">
        <el-result
          icon="info"
          title="申請審核中"
          sub-title="您的賣場申請已提交，管理員正在審核中，請耐心等候。"
        >
          <template #extra>
            <div class="status-actions">
              <el-button type="primary" @click="router.push('/member/mystore')">查看賣場狀態</el-button>
              <el-button @click="router.push('/')">回首頁</el-button>
            </div>
          </template>
        </el-result>
      </el-card>

      <!-- 2. 申請表單 (未申請或已駁回) -->
      <el-card v-else-if="status === 'NotApplied' || status === 'Rejected'" class="apply-card">
        <template #header>
          <div class="card-header">
            <span>{{ status === 'Rejected' ? '重新申請成為賣家' : '申請成為賣家' }}</span>
          </div>
        </template>

        <div v-if="status === 'Rejected'" class="reject-alert">
          <el-alert
            title="先前的申請已被駁回"
            type="error"
            description="請根據管理員的建議修改資料後再次提交。"
            show-icon
            :closable="false"
          />
        </div>
        <!-- ... 其餘表單內容 ... -->

      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="100px"
        label-position="top"
      >
        <el-form-item label="賣場標誌 (Logo)" prop="logoUrl">
          <el-upload
            class="logo-uploader"
            action="#"
            :show-file-list="false"
            :auto-upload="false"
            :on-change="handleLogoChange"
          >
            <img v-if="form.logoUrl" :src="baseUrl + form.logoUrl" class="logo-preview" />
            <el-icon v-else class="logo-uploader-icon"><Plus /></el-icon>
            <template #tip>
              <div class="el-upload__tip">建議比例 1:1，檔案大小不超過 2MB</div>
            </template>
          </el-upload>
        </el-form-item>

        <el-form-item label="賣場名稱" prop="storeName">
          <el-input v-model="form.storeName" placeholder="請輸入賣場名稱 (例: 我的選物店)" />
        </el-form-item>

        <el-form-item label="賣場介紹" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            :rows="4"
            placeholder="請簡單介紹您的賣場，讓顧客更認識您..."
          />
        </el-form-item>

        <div class="form-actions">
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            提交申請
          </el-button>
          <el-button @click="router.push('/member/mystore')">取消</el-button>
        </div>
      </el-form>
    </el-card>
    </template>
  </div>
</template>


<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { FormInstance, FormRules, UploadFile } from 'element-plus'
import { applyStoreApi, getStoreStatusApi, uploadStoreLogoApi } from '@/api/store'
import type { StoreStatus } from '@/types/store'

const router = useRouter()
const formRef = ref<FormInstance>()
const submitting = ref(false)
const initialLoading = ref(true)
const status = ref<StoreStatus | ''>('')
const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7125'

const form = reactive({
  storeName: '',
  description: '',
  logoUrl: ''
})

const rules = reactive<FormRules>({
  storeName: [
    { required: true, message: '請輸入賣場名稱', trigger: 'blur' },
    { min: 2, max: 50, message: '長度需在 2 到 50 個字之間', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: '長度不能超過 500 個字', trigger: 'blur' }
  ]
})

const checkStatus = async () => {
  initialLoading.value = true
  try {
    const res = await getStoreStatusApi()
    status.value = res.data.status

    // 如果已經是賣家，直接去數據中心
    if (status.value === 'Approved') {
      router.replace('/seller')
    }
  } catch (error) {
    console.error('檢查狀態失敗', error)
  } finally {
    initialLoading.value = false
  }
}

const handleLogoChange = async (uploadFile: UploadFile) => {
  const file = uploadFile.raw
  if (!file) return

  if (file.size / 1024 / 1024 > 2) {
    ElMessage.error('圖片大小不能超過 2MB!')
    return
  }

  try {
    const res = await uploadStoreLogoApi(file as File)
    form.logoUrl = res.data.url
    ElMessage.success('圖片上傳成功')
  } catch (error) {
    ElMessage.error('圖片上傳失敗')
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        submitting.value = true
        await applyStoreApi(form)
        ElMessage.success('申請已成功提交，請靜候管理員審核')
        // 提交成功後，重新整理狀態並顯示「審核中」
        await checkStatus()
      } catch (error: any) {
        ElMessage.error(error.response?.data?.message || '提交失敗，請稍後再試')
      } finally {
        submitting.value = false
      }
    }
  })
}

onMounted(() => {
  checkStatus()
})
</script>

<style scoped>
.seller-apply-container {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.status-card, .apply-card {
  margin-top: 20px;
  border-radius: 8px;
}

.card-header {
  font-size: 1.25rem;
  font-weight: bold;
}

.reject-alert {
  margin-bottom: 25px;
}

.logo-uploader {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  width: 120px;
  height: 120px;
  transition: border-color 0.3s;
}

.logo-uploader:hover {
  border-color: #409eff;
}

.logo-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 120px;
  height: 120px;
  text-align: center;
  line-height: 120px;
}

.logo-preview {
  width: 120px;
  height: 120px;
  display: block;
  object-fit: cover;
}

.form-actions {
  margin-top: 40px;
  display: flex;
  justify-content: center;
  gap: 20px;
}

.status-actions {
  display: flex;
  gap: 10px;
  justify-content: center;
}
</style>
