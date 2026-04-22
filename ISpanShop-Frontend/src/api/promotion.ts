import request from './request'
import type { ApiPromotionsResponse } from '@/types/promotion'

export async function fetchActivePromotions(): Promise<ApiPromotionsResponse> {
  const response = await request.get<ApiPromotionsResponse>('/api/promotions/active')
  return response.data
}

// ─── 賣家促銷活動管理 ────────────────────────────────────────────

/** 取得賣家自己的活動列表 */
export async function fetchSellerPromotions(params: { status?: string; page?: number; pageSize?: number } = {}) {
  const response = await request.get('/api/seller/promotions', { params })
  return response.data
}

/** 新增活動（送審） */
export async function createSellerPromotion(data: unknown) {
  const response = await request.post('/api/seller/promotions', data)
  return response.data
}

/** 編輯活動 */
export async function updateSellerPromotion(id: number, data: unknown) {
  const response = await request.put(`/api/seller/promotions/${id}`, data)
  return response.data
}

/** 刪除活動 */
export async function deleteSellerPromotion(id: number) {
  const response = await request.delete(`/api/seller/promotions/${id}`)
  return response.data
}
