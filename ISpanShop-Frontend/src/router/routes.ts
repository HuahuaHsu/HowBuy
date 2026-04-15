import type { RouteRecordRaw } from 'vue-router';

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/auth/LoginView.vue'),
    meta: { requiresAuth: false, hideForAuth: true }
  },
  {
    path: '/register',
    name: 'register',
    component: () => import('../views/auth/RegisterView.vue'),
    meta: { requiresAuth: false, hideForAuth: true }
  },
  {
    path: '/member',
    name: 'member',
    component: () => import('../views/member/MemberCenterView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/member/orders',
    name: 'member-orders',
    component: () => import('../views/member/OrdersView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/member/settings',
    name: 'member-settings',
    component: () => import('../views/member/SettingsView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/member/wallet',
    name: 'member-wallet',
    component: () => import('../views/member/WalletView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/member/mystore',
    name: 'member-mystore',
    component: () => import('../views/member/MyStoreView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/cart',
    name: 'cart',
    component: () => import('../views/cart/CartView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/wip',
    name: 'wip',
    component: () => import('../views/error/WipView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('../views/error/NotFoundView.vue')
  }
];
