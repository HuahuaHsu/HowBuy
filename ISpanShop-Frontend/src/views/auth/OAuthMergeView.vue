<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { mergeOAuthAccount } from '../../api/auth';
import { useAuthStore } from '../../stores/auth';
import { ElMessage } from 'element-plus';
import { storage } from '../../utils/storage';
import { Lock } from '@element-plus/icons-vue';
import type { FormInstance, FormRules } from 'element-plus';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const mergeFormRef = ref<FormInstance>();

const email = route.query.email as string;
const provider = route.query.provider as string;
const providerId = route.query.providerId as string; // 這是真正的 Google ID

const mergeForm = reactive({
  password: ''
});

const rules = reactive<FormRules>({
  password: [
    { required: true, message: '請輸入原帳號密碼', trigger: 'blur' }
  ]
});

const handleMerge = async (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  const valid = await formEl.validate();
  if (!valid) return;

  try {
    const response = await mergeOAuthAccount({
        account: email,           // 既有帳號
        password: mergeForm.password, // 密碼
        provider: provider,        // Google
        oAuthProviderId: providerId // 真正的 Google ID
    });
    
    const { data } = response;
    authStore.token = data.token;
    storage.setToken(data.token);
    await authStore.fetchUserInfo();

    ElMessage.success('帳號綁定並登入成功');
    router.push('/');
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '驗證失敗，請檢查密碼');
  }
};
</script>

<template>
  <div class="merge-container">
    <el-card class="merge-card">
      <template #header>
        <div class="card-header">
          <h2>帳號綁定確認</h2>
          <p class="subtitle">偵測到您的 Email <b>{{ email }}</b> 已有既有帳號</p>
        </div>
      </template>

      <div class="info-alert">
        <el-alert
          title="請輸入您在 HowBuy 的原帳號密碼，以將此 Google 帳號與現有帳號綁定。"
          type="warning"
          :closable="false"
          show-icon
        />
      </div>

      <el-form ref="mergeFormRef" :model="mergeForm" :rules="rules" @submit.prevent="handleMerge(mergeFormRef)">
        <el-form-item label="登入帳號" label-width="80px">
          <el-input :value="email" disabled />
        </el-form-item>
        
        <el-form-item prop="password" label="原密碼" label-width="80px">
          <el-input
            v-model="mergeForm.password"
            type="password"
            placeholder="請輸入原帳號密碼"
            :prefix-icon="Lock"
            show-password
          />
        </el-form-item>

        <div class="actions">
          <el-button type="primary" class="w-full" @click="handleMerge(mergeFormRef)">
            確認綁定並登入
          </el-button>
          <el-button class="w-full" @click="router.push('/login')">
            取消
          </el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<style scoped>
.merge-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 70vh;
  padding: 20px;
}
.merge-card {
  width: 100%;
  max-width: 450px;
}
.card-header {
  text-align: center;
}
.subtitle {
  color: #666;
  font-size: 14px;
}
.info-alert {
  margin-bottom: 20px;
}
.actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 20px;
}
.w-full {
  width: 100%;
  margin-left: 0 !important;
}
</style>