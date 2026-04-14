import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { loginApi } from '../api/auth';
import type { LoginRequest } from '../types/auth';
import { storage } from '../utils/storage';

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(storage.getToken());
  const memberInfo = ref<{
    memberId: number | null;
    email: string | null;
    memberName: string | null;
  }>(storage.getUser() || {
    memberId: null,
    email: null,
    memberName: null
  });

  // Getters
  const isLoggedIn = computed(() => !!token.value);

  // Actions
  async function login(loginData: LoginRequest) {
    try {
      const response = await loginApi(loginData);
      const { data } = response;
      
      // 1. 存入 Token 與 使用者資訊 (由後端 DTO 回傳)
      token.value = data.token;
      memberInfo.value = {
        memberId: data.memberId,
        email: data.email,
        memberName: data.memberName
      };

      // 2. 持久化到 localStorage
      storage.setToken(data.token);
      storage.setUser(memberInfo.value);
      
      return true;
    } catch (error) {
      console.error('登入失敗:', error);
      throw error;
    }
  }

  function logout() {
    // 1. 重置 State
    token.value = null;
    memberInfo.value = {
      memberId: null,
      email: null,
      memberName: null
    };

    // 2. 清除 localStorage
    storage.clearAll();
  }

  return {
    token,
    memberInfo,
    isLoggedIn,
    login,
    logout
  };
});
