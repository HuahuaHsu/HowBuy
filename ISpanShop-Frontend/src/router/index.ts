import { createRouter, createWebHistory } from 'vue-router';
import { routes } from './routes';
import { useAuthStore } from '../stores/auth';
import { createRouter, createWebHistory } from 'vue-router'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import MemberLayout from '@/layouts/MemberLayout.vue'
import BlankLayout from '@/layouts/BlankLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
});

// 全域路由守衛
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isLoggedIn = authStore.isLoggedIn;

  // 1. 檢查頁面是否需要登入
  if (to.meta.requiresAuth && !isLoggedIn) {
    // 未登入且進入需要登入的頁面 -> 導向 /login
    return next({ name: 'login', query: { redirect: to.fullPath } });
  }

  // 2. 檢查是否為已登入不應進入的頁面 (例如登入後進 /login)
  if (to.meta.hideForAuth && isLoggedIn) {
    // 已登入但進入 /login 或 /register -> 導向首頁
    return next({ name: 'home' });
  }

  // 正常跳轉
  next();
});
  routes: [
    // 前台
    {
      path: '/',
      component: DefaultLayout,
      children: [
        { path: '', name: 'home', component: () => import('@/views/HomeView.vue') },
        { path: 'product/:id', name: 'ProductDetail', component: () => import('@/views/ProductDetailView.vue') },
        // 之後加:商品列表、購物車等
      ],
    },
    // 會員中心
    {
      path: '/member',
      component: MemberLayout,
      children: [
        { path: 'profile', component: () => import('@/views/HomeView.vue') },
        // 之後加各種會員頁
      ],
    },
    // 登入註冊
    {
      path: '/auth',
      component: BlankLayout,
      children: [
        { path: 'login', component: () => import('@/views/HomeView.vue') },
        { path: 'register', component: () => import('@/views/HomeView.vue') },
      ],
    },
  ],
})

export default router;
