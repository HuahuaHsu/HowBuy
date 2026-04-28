<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { googleLogin, bindOAuthApi } from '../../api/auth';
import { useAuthStore } from '../../stores/auth';
import { ElMessage } from 'element-plus';
import { storage } from '../../utils/storage';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const loading = ref(true);

onMounted(async () => {
  const code = route.query.code as string;
  const redirectUri = `${window.location.origin}/auth/callback`;

  if (!code) {
    ElMessage.error('無效的登入請求');
    router.push('/login');
    return;
  }

  try {
    if (authStore.isLoggedIn) {
      // ────────────────────────────────────────────────────────
      // 情況 A：使用者已登入 -> 這是「綁定」流程
      // ────────────────────────────────────────────────────────
      const res = await bindOAuthApi(code, redirectUri);
      if (res.data.success) {
        ElMessage.success('帳號綁定成功');
        await authStore.fetchUserInfo(); // 刷新 store 中的 provider 資訊
        router.push('/member/profile');
      }
    } else {
      // ────────────────────────────────────────────────────────
      // 情況 B：使用者未登入 -> 這是「登入/註冊」流程
      // ────────────────────────────────────────────────────────
      const response = await googleLogin(code, redirectUri);
      const result = response.data;

      if (result.status === 'Success' && result.token) {
        authStore.token = result.token;
        storage.setToken(result.token);
        await authStore.fetchUserInfo();
        
        ElMessage.success('登入成功');
        router.push('/');
      } else if (result.status === 'MergeRequired') {
        ElMessage.warning('偵測到相同 Email，請進行帳號綁定');
        router.push({
          path: '/auth/oauth-merge',
          query: { 
            email: result.email,
            provider: 'Google',
            providerId: result.providerId // 傳遞正確的 Google ID
          }
        });
      }
    }
  } catch (error: any) {
    console.error('OAuth 處理失敗', error);
    const msg = error.response?.data?.message || '處理失敗，請稍後再試';
    ElMessage.error(msg);
    router.push(authStore.isLoggedIn ? '/member/profile' : '/login');
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="callback-container" v-loading="loading">
    <div v-if="loading" class="loading-box">
      <p>正在與 Google 驗證身分，請稍候...</p>
    </div>
  </div>
</template>

<style scoped>
.callback-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}
.loading-box {
  text-align: center;
}
</style>