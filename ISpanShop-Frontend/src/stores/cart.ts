import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import type { CartItem } from '@/types/cart';

export const useCartStore = defineStore('cart', () => {
  // --- State ---
  // 初始化時嘗試從 localStorage 讀取 cart_data
  const savedCart = localStorage.getItem('cart_data');
  const items = ref<CartItem[]>(savedCart ? JSON.parse(savedCart) : []);

  // --- Getters ---
  /**
   * 總金額：僅計算已勾選 (selected === true) 的項目
   */
  const totalPrice = computed(() => {
    return items.value
      .filter((item) => item.selected)
      .reduce((total, item) => total + item.price * item.quantity, 0);
  });

  /**
   * 已勾選的商品種類數量
   */
  const selectedCount = computed(() => {
    return items.value.filter((item) => item.selected).length;
  });

  /**
   * 購物車內商品總數 (所有項目的數量加總，可選用)
   */
  const totalQuantity = computed(() => {
    return items.value.reduce((total, item) => total + item.quantity, 0);
  });

  // --- Actions ---
  /**
   * 加入購物車
   * 若 productId 與 variantId 皆相同則增加數量 (不可超過 stock)，否則新增
   */
  const addToCart = (newItem: CartItem) => {
    const existingIndex = items.value.findIndex(
      (item) => item.productId === newItem.productId && item.variantId === newItem.variantId
    );

    if (existingIndex > -1) {
      const existingItem = items.value[existingIndex];
      if (existingItem) {
        const targetQty = existingItem.quantity + newItem.quantity;
        // 確保不超過庫存
        existingItem.quantity = Math.min(targetQty, existingItem.stock);
      }
    } else {
      // 確保初始選中狀態與基本屬性
      items.value.push({
        ...newItem,
        selected: newItem.selected ?? true,
      });
    }
  };

  /**
   * 更新指定變體的數量
   */
  const updateQuantity = (variantId: number, qty: number) => {
    const item = items.value.find((i) => i.variantId === variantId);
    if (item) {
      // 數量區間為 1 到 庫存上限
      item.quantity = Math.max(1, Math.min(qty, item.stock));
    }
  };

  /**
   * 移除商品
   */
  const removeFromCart = (variantId: number) => {
    items.value = items.value.filter((item) => item.variantId !== variantId);
  };

  /**
   * 切換勾選狀態
   */
  const toggleSelect = (variantId: number) => {
    const item = items.value.find((i) => i.variantId === variantId);
    if (item) {
      item.selected = !item.selected;
    }
  };

  /**
   * 切換全選狀態 (輔助功能)
   */
  const toggleAll = (selected: boolean) => {
    items.value.forEach((item) => {
      item.selected = selected;
    });
  };

  /**
   * 清空購物車 (通常結帳後使用)
   */
  const clearCart = () => {
    items.value = [];
  };

  // --- Persistence ---
  // 使用 watch 監控 items 變化，即時將資料存回 localStorage
  watch(
    items,
    (newItems) => {
      localStorage.setItem('cart_data', JSON.stringify(newItems));
    },
    { deep: true }
  );

  return {
    items,
    totalPrice,
    selectedCount,
    totalQuantity,
    addToCart,
    updateQuantity,
    removeFromCart,
    toggleSelect,
    toggleAll,
    clearCart,
  };
});
