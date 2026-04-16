import { defineStore } from 'pinia'
import { ref, computed, watch } from 'vue'

export interface CartItem {
  productId: number
  variantId: number | null
  name: string
  image: string
  price: number
  quantity: number
  /** 規格描述，例如 "顏色: 紅、尺寸: M"，無規格時為空字串 */
  specLabel: string
}

const CART_KEY = 'cart_items'

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>(
    (() => {
      try {
        return JSON.parse(localStorage.getItem(CART_KEY) ?? '[]') as CartItem[]
      } catch {
        return []
      }
    })(),
  )

  /** 商品種類數（幾種不同商品/規格），用於 Header 徽章 */
  const totalCount = computed(() => items.value.length)

  /** 所有商品數量加總，用於「共 X 件」文字 */
  const totalQuantity = computed(() =>
    items.value.reduce((sum, item) => sum + item.quantity, 0),
  )

  const totalPrice = computed(() =>
    items.value.reduce((sum, item) => sum + item.price * item.quantity, 0),
  )

  function addItem(newItem: Omit<CartItem, 'quantity'> & { quantity?: number }): void {
    const qty = newItem.quantity ?? 1
    const existing = items.value.find(
      (i) => i.productId === newItem.productId && i.variantId === newItem.variantId,
    )
    if (existing) {
      existing.quantity += qty
    } else {
      items.value.push({ ...newItem, quantity: qty })
    }
  }

  function removeItem(productId: number, variantId: number | null): void {
    items.value = items.value.filter(
      (i) => !(i.productId === productId && i.variantId === variantId),
    )
      }

  function updateQty(productId: number, variantId: number | null, qty: number): void {
    const item = items.value.find(
      (i) => i.productId === productId && i.variantId === variantId,
    )
    if (!item) return
    if (qty <= 0) {
      removeItem(productId, variantId)
    } else {
      item.quantity = qty
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

  function clearCart(): void {
    items.value = []
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

  // 持久化到 localStorage
  watch(items, (val) => {
    localStorage.setItem(CART_KEY, JSON.stringify(val))
  }, { deep: true })

  return { items, totalCount, totalQuantity, totalPrice, addItem, removeItem, updateQty, clearCart }
})
