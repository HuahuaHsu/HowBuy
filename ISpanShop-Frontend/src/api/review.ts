import axios from './axios'

export interface SubmitReviewParams {
  orderId: number
  rating: number
  comment: string
  imageUrls?: string[]
}

/**
 * 取得特定商品的評論列表 (與後端 OrdersController 路由對接)
 */
export async function fetchProductReviews(productId: number): Promise<any[]> {
  const response = await axios.get<any[]>(`/api/front/orders/product/${productId}`)
  return response.data
}

/**
 * 一鍵生成測試評論 (指定商品)
 */
export async function generateMockReviews(productId: number, count = 5): Promise<any> {
  const response = await axios.post(`/api/front/orders/product/${productId}/mock?count=${count}`)
  return response.data
}
